using AutoMapper;
using ERP.Transport.Application.DTOs.Dispatch;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Dispatch management — tracks when jobs are physically dispatched.
/// Uses TransportRequest status transitions and movement tracking.
/// </summary>
public class DispatchService : IDispatchService
{
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IRepository<TransportMovement> _movementRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DispatchService> _logger;

    public DispatchService(
        IRepository<TransportRequest> jobRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IRepository<TransportMovement> movementRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DispatchService> logger)
    {
        _jobRepo = jobRepo;
        _vehicleRepo = vehicleRepo;
        _movementRepo = movementRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<DispatchDto>> GetPendingDispatchAsync(CancellationToken ct = default)
    {
        // Jobs with vehicle assigned but not yet dispatched (InTransit)
        var pendingJobs = await _jobRepo.FindAsync(j =>
            (j.Status == TransportStatus.VehicleAssigned ||
             j.Status == TransportStatus.RateEntered ||
             j.Status == TransportStatus.RateApproval) &&
            !j.IsConsolidated);

        return await MapToDispatchDtos(pendingJobs);
    }

    public async Task<List<DispatchDto>> GetDispatchedTodayAsync(CancellationToken ct = default)
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        // Find movements created today with Dispatched milestone
        var todayMovements = await _movementRepo.FindAsync(m =>
            m.Milestone == MovementMilestone.Dispatched &&
            m.Timestamp >= todayStart &&
            m.Timestamp < todayEnd);

        var jobIds = todayMovements.Select(m => m.TransportRequestId).Distinct().ToList();
        if (!jobIds.Any())
            return new List<DispatchDto>();

        var jobs = await _jobRepo.FindAsync(j => jobIds.Contains(j.Id));
        var dtos = await MapToDispatchDtos(jobs);

        // Enrich with dispatch timestamps
        foreach (var dto in dtos)
        {
            var movement = todayMovements.FirstOrDefault(m => m.TransportRequestId == dto.JobId);
            if (movement != null)
            {
                dto.DispatchDate = movement.Timestamp;
                dto.Remarks = movement.Remarks;
            }
        }

        return dtos;
    }

    public async Task<DispatchDto> DispatchJobAsync(
        Guid requestId, CreateDispatchRequest request, Guid userId, CancellationToken ct = default)
    {
        var job = await _jobRepo.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException($"Transport job {requestId} not found");

        if (job.Status == TransportStatus.InTransit)
            throw new InvalidOperationException("Job is already dispatched / in transit");

        if (job.Status == TransportStatus.Delivered || job.Status == TransportStatus.Cleared ||
            job.Status == TransportStatus.Cancelled)
            throw new InvalidOperationException($"Cannot dispatch a job with status {job.Status}");

        if (job.Status < TransportStatus.VehicleAssigned)
            throw new InvalidOperationException("Vehicle must be assigned before dispatching");

        // ── Update job status ────────────────────────────────────
        job.Status = TransportStatus.InTransit;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        // ── Record dispatch movement ─────────────────────────────
        var dispatchDate = request.DispatchDate ?? DateTime.UtcNow;
        var movement = new TransportMovement
        {
            TransportRequestId = job.Id,
            Milestone = MovementMilestone.Dispatched,
            LocationName = job.PickupCity ?? job.OriginLocationName,
            Timestamp = dispatchDate,
            Remarks = request.Remarks ?? "Job dispatched",
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };
        await _movementRepo.AddAsync(movement);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {JobNumber} dispatched at {DispatchDate}",
            job.RequestNumber, dispatchDate);

        // Build response
        var dto = await BuildDispatchDto(job);
        dto.DispatchDate = dispatchDate;
        dto.DispatchedBy = userId;
        dto.Remarks = request.Remarks;

        return dto;
    }

    public async Task<DispatchSummaryDto> GetSummaryAsync(DateTime? date, CancellationToken ct = default)
    {
        var targetDate = (date ?? DateTime.UtcNow).Date;
        var dayStart = targetDate;
        var dayEnd = targetDate.AddDays(1);

        // Count dispatched today (movements with Dispatched milestone)
        var dispatchedToday = await _movementRepo.CountAsync(m =>
            m.Milestone == MovementMilestone.Dispatched &&
            m.Timestamp >= dayStart &&
            m.Timestamp < dayEnd);

        // Count pending dispatch (vehicle assigned but not yet in transit)
        var pendingDispatch = await _jobRepo.CountAsync(j =>
            (j.Status == TransportStatus.VehicleAssigned ||
             j.Status == TransportStatus.RateEntered ||
             j.Status == TransportStatus.RateApproval) &&
            !j.IsConsolidated);

        // Count currently in transit
        var inTransit = await _jobRepo.CountAsync(j =>
            j.Status == TransportStatus.InTransit);

        // Count delivered today
        var deliveredToday = await _jobRepo.CountAsync(j =>
            j.Status == TransportStatus.Delivered &&
            j.UpdatedDate.HasValue &&
            j.UpdatedDate.Value >= dayStart &&
            j.UpdatedDate.Value < dayEnd);

        return new DispatchSummaryDto
        {
            Date = targetDate,
            TotalDispatched = dispatchedToday,
            PendingDispatch = pendingDispatch,
            InTransit = inTransit,
            DeliveredToday = deliveredToday
        };
    }

    // ── Private Helpers ──────────────────────────────────────────

    private async Task<List<DispatchDto>> MapToDispatchDtos(IEnumerable<TransportRequest> jobs)
    {
        var dtos = new List<DispatchDto>();
        foreach (var job in jobs)
        {
            dtos.Add(await BuildDispatchDto(job));
        }
        return dtos.OrderByDescending(d => d.RequiredDeliveryDate).ToList();
    }

    private async Task<DispatchDto> BuildDispatchDto(TransportRequest job)
    {
        var dto = new DispatchDto
        {
            JobId = job.Id,
            RequestNumber = job.RequestNumber,
            Status = job.Status,
            CustomerName = job.CustomerName,
            PickupCity = job.PickupCity,
            DropCity = job.DropCity,
            GrossWeightKg = job.GrossWeightKg,
            NumberOfPackages = job.NumberOfPackages,
            RequiredDeliveryDate = job.RequiredDeliveryDate
        };

        // Get the latest vehicle assignment
        var vehicles = await _vehicleRepo.FindAsync(v => v.TransportRequestId == job.Id);
        var latestVehicle = vehicles.OrderByDescending(v => v.CreatedDate).FirstOrDefault();
        if (latestVehicle != null)
        {
            dto.VehicleNumber = latestVehicle.VehicleNumber;
            dto.DriverName = latestVehicle.DriverName;
            dto.TransporterName = latestVehicle.TransporterName;
        }

        return dto;
    }
}
