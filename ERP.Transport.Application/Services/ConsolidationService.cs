using AutoMapper;
using ERP.Transport.Application.DTOs.Consolidation;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Interfaces.Clients;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Consolidation trip management — dedicated lifecycle operations.
/// </summary>
public class ConsolidationService : IConsolidationService
{
    private readonly IRepository<ConsolidatedTrip> _tripRepo;
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<ConsolidatedStopDelivery> _stopDeliveryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFreightClient _freightClient;
    private readonly ILogger<ConsolidationService> _logger;

    public ConsolidationService(
        IRepository<ConsolidatedTrip> tripRepo,
        IRepository<TransportRequest> jobRepo,
        IRepository<ConsolidatedStopDelivery> stopDeliveryRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFreightClient freightClient,
        ILogger<ConsolidationService> logger)
    {
        _tripRepo = tripRepo;
        _jobRepo = jobRepo;
        _stopDeliveryRepo = stopDeliveryRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _freightClient = freightClient;
        _logger = logger;
    }

    public async Task<List<ConsolidationSummaryDto>> GetActiveConsolidationsAsync(CancellationToken ct = default)
    {
        var activeTrips = await _tripRepo.FindAsync(t =>
            t.Status != ConsolidationStatus.Completed &&
            t.Status != ConsolidationStatus.Cancelled);

        var summaries = new List<ConsolidationSummaryDto>();
        foreach (var trip in activeTrips)
        {
            var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == trip.Id);
            var jobList = jobs.ToList();

            summaries.Add(new ConsolidationSummaryDto
            {
                Id = trip.Id,
                ReferenceNumber = trip.ReferenceNumber,
                DestinationLocationName = trip.DestinationLocationName,
                SharedVehicleNumber = trip.SharedVehicleNumber,
                Status = trip.Status,
                JobCount = trip.JobCount,
                TotalWeight = jobList.Sum(j => j.GrossWeightKg),
                TotalPackages = jobList.Sum(j => j.NumberOfPackages),
                CountryCode = trip.CountryCode,
                BranchId = trip.BranchId,
                CreatedDate = trip.CreatedDate
            });
        }

        return summaries.OrderByDescending(s => s.CreatedDate).ToList();
    }

    public async Task<ConsolidatedTripDto?> GetByIdAsync(Guid tripId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId);
        if (trip == null) return null;

        var dto = _mapper.Map<ConsolidatedTripDto>(trip);

        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        dto.Jobs = _mapper.Map<ICollection<TransportJobListDto>>(jobs.ToList());

        var stops = await _stopDeliveryRepo.FindAsync(s => s.ConsolidatedTripId == tripId);
        dto.StopDeliveries = _mapper.Map<ICollection<ConsolidatedStopDeliveryDto>>(stops.ToList());

        return dto;
    }

    public async Task<ConsolidatedTripDto> CreateAsync(
        CreateConsolidationRequest request, Guid userId, CancellationToken ct = default)
    {
        if (request.JobIds.Count < 2)
            throw new InvalidOperationException("At least 2 jobs are required for consolidation");

        // ── Load and validate all jobs ──────────────────────────
        var jobs = new List<TransportRequest>();
        foreach (var jobId in request.JobIds)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

            if (job.Status != TransportStatus.RequestCreated && job.Status != TransportStatus.RequestReceived)
                throw new InvalidOperationException($"Job {job.RequestNumber} is not in a consolidatable state");

            if (job.IsConsolidated)
                throw new InvalidOperationException($"Job {job.RequestNumber} is already part of a consolidated trip");

            jobs.Add(job);
        }

        // ── Create trip ─────────────────────────────────────────
        var firstJob = jobs.First();
        var year = DateTime.UtcNow.Year;
        var count = await _tripRepo.CountAsync(c => c.CountryCode == firstJob.CountryCode);

        var trip = new ConsolidatedTrip
        {
            ReferenceNumber = $"CT-{year}-{firstJob.CountryCode}-{(count + 1):D4}",
            DestinationLocationId = firstJob.DestinationLocationId,
            DestinationLocationName = firstJob.DestinationLocationName,
            SharedVehicleNumber = request.VehicleNumber,
            Status = ConsolidationStatus.Draft,
            Remarks = request.Remarks,
            JobCount = jobs.Count,
            CountryCode = firstJob.CountryCode,
            BranchId = firstJob.BranchId,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _tripRepo.AddAsync(trip);

        // ── Link jobs + create stop deliveries ──────────────────
        var stopSeq = 1;
        foreach (var job in jobs)
        {
            job.ConsolidatedTripId = trip.Id;
            job.IsConsolidated = true;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);

            var stop = new ConsolidatedStopDelivery
            {
                ConsolidatedTripId = trip.Id,
                TransportRequestId = job.Id,
                StopSequence = stopSeq++,
                LocationName = job.DestinationLocationName,
                Address = job.DropAddress,
                City = job.DropCity,
                Pincode = job.DropPincode,
                EstimatedArrival = request.PlannedDate ?? job.RequiredDeliveryDate,
                DeliveryStatus = DeliveryStatus.Pending,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };
            await _stopDeliveryRepo.AddAsync(stop);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Consolidation {TripRef} created with {Count} jobs",
            trip.ReferenceNumber, jobs.Count);

        return await GetByIdAsync(trip.Id, ct)
               ?? throw new InvalidOperationException("Failed to load newly created consolidation");
    }

    public async Task<ConsolidatedTripDto> AddJobAsync(
        Guid tripId, AddJobToConsolidationRequest request, Guid userId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        if (trip.Status != ConsolidationStatus.Draft && trip.Status != ConsolidationStatus.Confirmed)
            throw new InvalidOperationException("Cannot add jobs to a trip that is already dispatched or completed");

        var job = await _jobRepo.GetByIdAsync(request.TransportRequestId)
            ?? throw new KeyNotFoundException($"Transport job {request.TransportRequestId} not found");

        if (job.IsConsolidated)
            throw new InvalidOperationException($"Job {job.RequestNumber} is already part of a consolidated trip");

        if (job.Status != TransportStatus.RequestCreated && job.Status != TransportStatus.RequestReceived)
            throw new InvalidOperationException($"Job {job.RequestNumber} is not in a consolidatable state");

        // Link job
        job.ConsolidatedTripId = trip.Id;
        job.IsConsolidated = true;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        // Update trip job count
        trip.JobCount += 1;
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _tripRepo.Update(trip);

        // Create stop delivery
        var existingStops = await _stopDeliveryRepo.FindAsync(s => s.ConsolidatedTripId == tripId);
        var maxSeq = existingStops.Any() ? existingStops.Max(s => s.StopSequence) : 0;

        var stop = new ConsolidatedStopDelivery
        {
            ConsolidatedTripId = trip.Id,
            TransportRequestId = job.Id,
            StopSequence = maxSeq + 1,
            LocationName = job.DestinationLocationName,
            Address = job.DropAddress,
            City = job.DropCity,
            Pincode = job.DropPincode,
            EstimatedArrival = job.RequiredDeliveryDate,
            DeliveryStatus = DeliveryStatus.Pending,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };
        await _stopDeliveryRepo.AddAsync(stop);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {JobId} added to consolidation {TripRef}",
            job.Id, trip.ReferenceNumber);

        return await GetByIdAsync(tripId, ct)
               ?? throw new InvalidOperationException("Failed to reload consolidation");
    }

    public async Task RemoveJobAsync(
        Guid tripId, Guid transportRequestId, Guid userId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        if (trip.Status != ConsolidationStatus.Draft && trip.Status != ConsolidationStatus.Confirmed)
            throw new InvalidOperationException("Cannot remove jobs from a dispatched or completed trip");

        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Transport job {transportRequestId} not found");

        if (job.ConsolidatedTripId != tripId)
            throw new InvalidOperationException("Job is not part of this consolidated trip");

        // Unlink job
        job.ConsolidatedTripId = null;
        job.IsConsolidated = false;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        // Remove stop delivery
        var stop = await _stopDeliveryRepo.FirstOrDefaultAsync(
            s => s.ConsolidatedTripId == tripId && s.TransportRequestId == transportRequestId);
        if (stop != null)
        {
            _stopDeliveryRepo.Delete(stop);
        }

        // Update trip job count
        trip.JobCount = Math.Max(0, trip.JobCount - 1);
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _tripRepo.Update(trip);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {JobId} removed from consolidation {TripRef}",
            transportRequestId, trip.ReferenceNumber);
    }

    public async Task<ConsolidatedTripDto> DispatchAsync(
        Guid tripId, Guid userId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        if (trip.Status == ConsolidationStatus.InTransit)
            throw new InvalidOperationException("Trip is already dispatched");

        if (trip.Status == ConsolidationStatus.Completed || trip.Status == ConsolidationStatus.Cancelled)
            throw new InvalidOperationException("Cannot dispatch a completed or cancelled trip");

        trip.Status = ConsolidationStatus.InTransit;
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _tripRepo.Update(trip);

        // Update all linked jobs to InTransit
        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        foreach (var job in jobs)
        {
            job.Status = TransportStatus.InTransit;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Consolidation {TripRef} dispatched with {Count} jobs",
            trip.ReferenceNumber, trip.JobCount);

        return await GetByIdAsync(tripId, ct)
               ?? throw new InvalidOperationException("Failed to reload consolidation");
    }

    public async Task<ConsolidatedTripDto> CompleteAsync(
        Guid tripId, Guid userId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        if (trip.Status != ConsolidationStatus.InTransit)
            throw new InvalidOperationException("Only in-transit trips can be completed");

        trip.Status = ConsolidationStatus.Completed;
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _tripRepo.Update(trip);

        // Update all linked jobs to Delivered
        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        foreach (var job in jobs)
        {
            job.Status = TransportStatus.Delivered;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);

            // Send freight callback for linked freight jobs
            if (job.Source == JobSource.FreightJob && job.SourceReferenceId.HasValue)
            {
                try
                {
                    await _freightClient.SendCallbackAsync(job.SourceReferenceId.Value,
                        new TransportCallbackPayload
                        {
                            Event = "TRANSPORT_COMPLETED",
                            TransportJobNumber = job.RequestNumber,
                            Status = "Completed",
                            Remarks = $"Consolidated trip {trip.ReferenceNumber} completed"
                        }, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send freight callback for job {JobId}", job.Id);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Consolidation {TripRef} completed", trip.ReferenceNumber);

        return await GetByIdAsync(tripId, ct)
               ?? throw new InvalidOperationException("Failed to reload consolidation");
    }

    public async Task CancelAsync(
        Guid tripId, string reason, Guid userId, CancellationToken ct = default)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        if (trip.Status == ConsolidationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed trip");

        trip.Status = ConsolidationStatus.Cancelled;
        trip.Remarks = $"{trip.Remarks} | Cancelled: {reason}".TrimStart(' ', '|');
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _tripRepo.Update(trip);

        // Unlink all jobs
        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        foreach (var job in jobs)
        {
            job.ConsolidatedTripId = null;
            job.IsConsolidated = false;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Consolidation {TripRef} cancelled: {Reason}",
            trip.ReferenceNumber, reason);
    }
}
