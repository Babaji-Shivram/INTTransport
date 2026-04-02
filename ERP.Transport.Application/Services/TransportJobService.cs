using AutoMapper;
using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Warehouse;
using ERP.Transport.Application.DTOs.Workflow;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Interfaces.Clients;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using ERP.Transport.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Core transport job service — CRUD + lifecycle + workflow integration.
/// </summary>
public class TransportJobService : ITransportJobService
{
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IRepository<VehicleRate> _rateRepo;
    private readonly IRepository<TransportMovement> _movementRepo;
    private readonly IRepository<TransportDelivery> _deliveryRepo;
    private readonly IRepository<TransportDocument> _documentRepo;
    private readonly IRepository<TransportExpense> _expenseRepo;
    private readonly IRepository<VehicleFundRequest> _fundRequestRepo;
    private readonly IRepository<ConsolidatedTrip> _consolidatedTripRepo;
    private readonly IRepository<ConsolidatedVehicle> _consolidatedVehicleRepo;
    private readonly IRepository<ConsolidatedExpense> _consolidatedExpenseRepo;
    private readonly IRepository<ConsolidatedStopDelivery> _stopDeliveryRepo;
    private readonly IRepository<TransitWarehouse> _transitWarehouseRepo;
    private readonly IRepository<ExpenseApproval> _expenseApprovalRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWorkflowClient _workflowClient;
    private readonly IDataScopeService _dataScopeService;
    private readonly IConfiguration _configuration;
    private readonly IFreightClient _freightClient;
    private readonly ILogger<TransportJobService> _logger;

    public TransportJobService(
        IRepository<TransportRequest> jobRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IRepository<VehicleRate> rateRepo,
        IRepository<TransportMovement> movementRepo,
        IRepository<TransportDelivery> deliveryRepo,
        IRepository<TransportDocument> documentRepo,
        IRepository<TransportExpense> expenseRepo,
        IRepository<VehicleFundRequest> fundRequestRepo,
        IRepository<ConsolidatedTrip> consolidatedTripRepo,
        IRepository<ConsolidatedVehicle> consolidatedVehicleRepo,
        IRepository<ConsolidatedExpense> consolidatedExpenseRepo,
        IRepository<ConsolidatedStopDelivery> stopDeliveryRepo,
        IRepository<TransitWarehouse> transitWarehouseRepo,
        IRepository<ExpenseApproval> expenseApprovalRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IWorkflowClient workflowClient,
        IDataScopeService dataScopeService,
        IFreightClient freightClient,
        IConfiguration configuration,
        ILogger<TransportJobService> logger)
    {
        _jobRepo = jobRepo;
        _vehicleRepo = vehicleRepo;
        _rateRepo = rateRepo;
        _movementRepo = movementRepo;
        _deliveryRepo = deliveryRepo;
        _documentRepo = documentRepo;
        _expenseRepo = expenseRepo;
        _fundRequestRepo = fundRequestRepo;
        _consolidatedTripRepo = consolidatedTripRepo;
        _consolidatedVehicleRepo = consolidatedVehicleRepo;
        _consolidatedExpenseRepo = consolidatedExpenseRepo;
        _stopDeliveryRepo = stopDeliveryRepo;
        _transitWarehouseRepo = transitWarehouseRepo;
        _expenseApprovalRepo = expenseApprovalRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _workflowClient = workflowClient;
        _dataScopeService = dataScopeService;
        _freightClient = freightClient;
        _configuration = configuration;
        _logger = logger;
    }

    // ════════════════════════════════════════════════════════════
    //  CREATE
    // ════════════════════════════════════════════════════════════

    public async Task<TransportJobDto> CreateJobAsync(
        CreateTransportJobDto dto, Guid userId, string countryCode, Guid branchId)
    {
        // ── Data Scope: ensure user can create in this branch ──
        await _dataScopeService.EnsureBranchAccessAsync(branchId, "Transport", "Create");

        var entity = _mapper.Map<TransportRequest>(dto);
        entity.Source = JobSource.Standalone;
        entity.RequestDate = DateTime.UtcNow;
        entity.RequestNumber = await GenerateRequestNumberAsync(countryCode);
        entity.CountryCode = countryCode;
        entity.BranchId = branchId;
        entity.Status = TransportStatus.RequestCreated;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _jobRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // Start workflow (non-fatal)
        await StartWorkflowAsync(entity, userId);

        return _mapper.Map<TransportJobDto>(entity);
    }

    public async Task<TransportJobDto> CreateJobFromEnquiryAsync(CreateJobFromEnquiryDto dto, Guid userId)
    {
        var entity = new TransportRequest
        {
            Source = JobSource.CRMEnquiry,
            SourceReferenceId = dto.EnquiryId,
            SourceReferenceNumber = dto.EnquiryReferenceNumber,
            RequestDate = DateTime.UtcNow,
            RequestNumber = await GenerateRequestNumberAsync(dto.CountryCode),
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            GSTNumber = dto.GSTNumber,
            OriginLocationId = dto.OriginLocationId,
            OriginLocationName = dto.OriginLocationName,
            DestinationLocationId = dto.DestinationLocationId,
            DestinationLocationName = dto.DestinationLocationName,
            PickupAddress = dto.PickupAddress,
            DropAddress = dto.DropAddress,
            CargoType = dto.CargoType,
            CargoDescription = dto.CargoDescription,
            GrossWeightKg = dto.GrossWeightKg,
            NumberOfPackages = dto.NumberOfPackages,
            Container20Count = dto.Container20Count,
            Container40Count = dto.Container40Count,
            SpecialInstructions = dto.SpecialInstructions,
            CountryCode = dto.CountryCode,
            BranchId = dto.BranchId,
            BranchName = dto.BranchName,
            Status = TransportStatus.RequestCreated,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _jobRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        await StartWorkflowAsync(entity, userId);

        return _mapper.Map<TransportJobDto>(entity);
    }

    public async Task<TransportJobDto> CreateJobFromFreightAsync(CreateJobFromFreightDto dto, Guid userId)
    {
        var entity = new TransportRequest
        {
            Source = JobSource.FreightJob,
            SourceReferenceId = dto.FreightJobId,
            SourceReferenceNumber = dto.FreightJobReference,
            RequestDate = DateTime.UtcNow,
            RequestNumber = await GenerateRequestNumberAsync(dto.CountryCode),
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            OriginLocationId = dto.PickupLocationId,
            OriginLocationName = dto.PickupLocationName,
            PickupAddress = dto.PickupAddress,
            DestinationLocationId = dto.DeliveryLocationId,
            DestinationLocationName = dto.DeliveryLocationName,
            DropAddress = dto.DropAddress,
            CargoDescription = dto.CargoDescription,
            GrossWeightKg = dto.GrossWeightKg,
            Container20Count = dto.Container20Count,
            Container40Count = dto.Container40Count,
            RequiredDeliveryDate = dto.RequiredDeliveryDate,
            CountryCode = dto.CountryCode,
            BranchId = dto.BranchId,
            BranchName = dto.BranchName,
            Status = TransportStatus.RequestCreated,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _jobRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        await StartWorkflowAsync(entity, userId);

        return _mapper.Map<TransportJobDto>(entity);
    }

    // ════════════════════════════════════════════════════════════
    //  READ
    // ════════════════════════════════════════════════════════════

    public async Task<TransportJobDto?> GetJobByIdAsync(Guid id)
    {
        var entity = await _jobRepo.GetByIdAsync(id);
        if (entity == null) return null;

        // ── Data Scope: ensure user can view this entity ───────
        await _dataScopeService.EnsureEntityAccessAsync(entity.BranchId, entity.CreatedBy, "Transport", "Read");

        return _mapper.Map<TransportJobDto>(entity);
    }

    public async Task<PagedResultDto<TransportJobListDto>> GetJobsAsync(TransportJobFilterDto filter)
    {
        var (items, totalCount) = await _jobRepo.GetPagedAsync(
            predicate: BuildJobPredicate(filter),
            orderBy: q => filter.SortDescending
                ? q.OrderByDescending(x => x.CreatedDate)
                : q.OrderBy(x => x.CreatedDate),
            page: filter.Page,
            pageSize: filter.PageSize);

        // ── Data Scope: filter items by user's access level ────
        var scoped = await _dataScopeService.ApplyScopeFilterAsync(
            items, j => j.BranchId, j => j.CreatedBy, "Transport", "Read");

        return new PagedResultDto<TransportJobListDto>
        {
            Items = _mapper.Map<IEnumerable<TransportJobListDto>>(scoped),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<TransportJobListDto>> GetQueueAsync(TransportJobFilterDto filter)
    {
        var items = await _jobRepo.FindAsync(j =>
            j.Status == TransportStatus.RequestCreated &&
            (filter.BranchId == null || j.BranchId == filter.BranchId) &&
            (filter.CountryCode == null || j.CountryCode == filter.CountryCode));

        return _mapper.Map<IEnumerable<TransportJobListDto>>(items);
    }

    public async Task<IEnumerable<TransportJobListDto>> GetPendingApprovalAsync(Guid userId)
    {
        var items = await _jobRepo.FindAsync(j => j.Status == TransportStatus.RateApproval);
        return _mapper.Map<IEnumerable<TransportJobListDto>>(items);
    }

    public async Task<JobTimelineDto> GetJobTimelineAsync(Guid jobId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        var movements = await _movementRepo.FindAsync(m => m.TransportRequestId == jobId);

        var timeline = new JobTimelineDto
        {
            JobId = jobId,
            RequestNumber = job.RequestNumber
        };

        // Add creation event
        timeline.Entries.Add(new TimelineEntryDto
        {
            Timestamp = job.CreatedDate,
            Event = "Job Created",
            Description = $"Source: {job.Source}"
        });

        // Add movement entries
        foreach (var m in movements.OrderBy(m => m.Timestamp))
        {
            timeline.Entries.Add(new TimelineEntryDto
            {
                Timestamp = m.Timestamp,
                Event = m.Milestone.ToString(),
                Description = m.LocationName,
                PerformedBy = m.Remarks
            });
        }

        // Add delivery
        if (job.Delivery != null)
        {
            timeline.Entries.Add(new TimelineEntryDto
            {
                Timestamp = job.Delivery.DeliveryDate,
                Event = "Delivered",
                Description = $"Received by: {job.Delivery.ReceivedBy}"
            });
        }

        return timeline;
    }

    // ════════════════════════════════════════════════════════════
    //  UPDATE
    // ════════════════════════════════════════════════════════════

    public async Task<TransportJobDto> UpdateJobAsync(Guid id, UpdateTransportJobDto dto, Guid userId)
    {
        var entity = await _jobRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Transport job {id} not found");

        // ── Data Scope: ensure user can update this entity ─────
        await _dataScopeService.EnsureEntityAccessAsync(entity.BranchId, entity.CreatedBy, "Transport", "Update");

        // Only allow edits before Vehicle Assigned, and not on Cleared/Cancelled
        if (entity.Status > TransportStatus.RequestReceived)
            throw new InvalidOperationException("Job cannot be edited after vehicle assignment");

        if (entity.Status == TransportStatus.Cleared || entity.Status == TransportStatus.Cancelled)
            throw new InvalidOperationException("Job is closed and cannot be edited");

        // Apply non-null fields
        if (dto.OriginLocationId.HasValue) entity.OriginLocationId = dto.OriginLocationId;
        if (dto.OriginLocationName != null) entity.OriginLocationName = dto.OriginLocationName;
        if (dto.PickupAddress != null) entity.PickupAddress = dto.PickupAddress;
        if (dto.PickupCity != null) entity.PickupCity = dto.PickupCity;
        if (dto.PickupState != null) entity.PickupState = dto.PickupState;
        if (dto.PickupPincode != null) entity.PickupPincode = dto.PickupPincode;
        if (dto.DestinationLocationId.HasValue) entity.DestinationLocationId = dto.DestinationLocationId;
        if (dto.DestinationLocationName != null) entity.DestinationLocationName = dto.DestinationLocationName;
        if (dto.DropAddress != null) entity.DropAddress = dto.DropAddress;
        if (dto.DropCity != null) entity.DropCity = dto.DropCity;
        if (dto.DropState != null) entity.DropState = dto.DropState;
        if (dto.DropPincode != null) entity.DropPincode = dto.DropPincode;
        if (dto.CargoType.HasValue) entity.CargoType = dto.CargoType.Value;
        if (dto.CargoDescription != null) entity.CargoDescription = dto.CargoDescription;
        if (dto.GrossWeightKg.HasValue) entity.GrossWeightKg = dto.GrossWeightKg.Value;
        if (dto.NumberOfPackages.HasValue) entity.NumberOfPackages = dto.NumberOfPackages.Value;
        if (dto.Container20Count.HasValue) entity.Container20Count = dto.Container20Count.Value;
        if (dto.Container40Count.HasValue) entity.Container40Count = dto.Container40Count.Value;
        if (dto.VehicleTypeRequired.HasValue) entity.VehicleTypeRequired = dto.VehicleTypeRequired.Value;
        if (dto.DeliveryType.HasValue) entity.DeliveryType = dto.DeliveryType.Value;
        if (dto.RequiredDeliveryDate.HasValue) entity.RequiredDeliveryDate = dto.RequiredDeliveryDate;
        if (dto.Priority.HasValue) entity.Priority = dto.Priority.Value;
        if (dto.SpecialInstructions != null) entity.SpecialInstructions = dto.SpecialInstructions;
        if (dto.Division != null) entity.Division = dto.Division;
        if (dto.Plant != null) entity.Plant = dto.Plant;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _jobRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportJobDto>(entity);
    }

    public async Task DeleteJobAsync(Guid id, Guid userId)
    {
        var entity = await _jobRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Transport job {id} not found");

        if (entity.Status > TransportStatus.RequestReceived)
            throw new InvalidOperationException("Job cannot be cancelled after vehicle assignment");

        entity.Status = TransportStatus.Cancelled;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        // Cancel workflow if running
        if (entity.WorkflowInstanceId.HasValue)
        {
            try { await _workflowClient.CancelAsync(entity.WorkflowInstanceId.Value, userId, new CancelWorkflowDto { Reason = "Job cancelled" }); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to cancel workflow for job {JobId}", id); }
        }
    }

    // ════════════════════════════════════════════════════════════
    //  LIFECYCLE
    // ════════════════════════════════════════════════════════════

    public async Task<TransportJobDto> ReceiveJobAsync(Guid id, Guid userId)
    {
        var entity = await _jobRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Transport job {id} not found");

        if (entity.Status != TransportStatus.RequestCreated)
            throw new InvalidOperationException("Only jobs with status 'Request Created' can be received");

        entity.Status = TransportStatus.RequestReceived;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _jobRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        await AdvanceWorkflowAsync(entity, userId, "Request received by operations");

        return _mapper.Map<TransportJobDto>(entity);
    }

    public async Task<TransportJobDto> AssignVehicleAsync(Guid jobId, AssignVehicleDto dto, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        if (job.Status != TransportStatus.RequestReceived && job.Status != TransportStatus.VehicleAssigned)
            throw new InvalidOperationException("Vehicle can only be assigned after request is received");

        // ── Vehicle type validation ─────────────────────────────
        if (job.VehicleTypeRequired != default && dto.VehicleType != job.VehicleTypeRequired)
            throw new InvalidOperationException(
                $"Vehicle type mismatch: job requires {job.VehicleTypeRequired} but {dto.VehicleType} was assigned");

        var vehicle = _mapper.Map<TransportVehicle>(dto);
        vehicle.TransportRequestId = jobId;
        vehicle.CreatedBy = userId;
        vehicle.CreatedDate = DateTime.UtcNow;

        await _vehicleRepo.AddAsync(vehicle);

        if (job.Status == TransportStatus.RequestReceived)
        {
            job.Status = TransportStatus.VehicleAssigned;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
        }

        await _unitOfWork.SaveChangesAsync();
        await AdvanceWorkflowAsync(job, userId, "Vehicle assigned");

        return _mapper.Map<TransportJobDto>(job);
    }

    public async Task<TransportJobDto> UpdateVehicleAssignmentAsync(
        Guid jobId, Guid vehicleId, UpdateVehicleAssignmentDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Vehicle assignment {vehicleId} not found");

        if (vehicle.TransportRequestId != jobId)
            throw new InvalidOperationException("Vehicle does not belong to this job");

        if (dto.VehicleNumber != null) vehicle.VehicleNumber = dto.VehicleNumber;
        if (dto.VehicleType.HasValue) vehicle.VehicleType = dto.VehicleType.Value;
        if (dto.DriverName != null) vehicle.DriverName = dto.DriverName;
        if (dto.DriverPhone != null) vehicle.DriverPhone = dto.DriverPhone;
        if (dto.LRNumber != null) vehicle.LRNumber = dto.LRNumber;
        if (dto.LRDate.HasValue) vehicle.LRDate = dto.LRDate;

        vehicle.UpdatedBy = userId;
        vehicle.UpdatedDate = DateTime.UtcNow;

        _vehicleRepo.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        var job = await _jobRepo.GetByIdAsync(jobId);
        return _mapper.Map<TransportJobDto>(job!);
    }

    public async Task<TransportJobDto> EnterRateAsync(
        Guid jobId, Guid vehicleId, EnterRateDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Vehicle assignment {vehicleId} not found");

        if (vehicle.TransportRequestId != jobId)
            throw new InvalidOperationException("Vehicle does not belong to this job");

        var rate = _mapper.Map<VehicleRate>(dto);
        rate.TransportVehicleId = vehicleId;
        rate.CreatedBy = userId;
        rate.CreatedDate = DateTime.UtcNow;

        // ── Auto-compute TotalRate ──────────────────────────────
        rate.TotalRate = rate.FreightRate
                       + rate.DetentionCharges
                       + rate.VaraiCharges
                       + rate.EmptyContainerReturn
                       + rate.TollCharges
                       + rate.OtherCharges;

        await _rateRepo.AddAsync(rate);

        var job = await _jobRepo.GetByIdAsync(jobId)!;
        if (job!.Status == TransportStatus.VehicleAssigned)
        {
            job.Status = TransportStatus.RateEntered;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
        }

        await _unitOfWork.SaveChangesAsync();
        await AdvanceWorkflowAsync(job, userId, "Rate entered");

        return _mapper.Map<TransportJobDto>(job);
    }

    public async Task<TransportJobDto> SubmitRateForApprovalAsync(Guid jobId, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        if (job.Status != TransportStatus.RateEntered)
            throw new InvalidOperationException("Rate must be entered before submitting for approval");

        job.Status = TransportStatus.RateApproval;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;

        _jobRepo.Update(job);
        await _unitOfWork.SaveChangesAsync();

        await AdvanceWorkflowAsync(job, userId, "Rate submitted for approval");

        return _mapper.Map<TransportJobDto>(job);
    }

    public async Task<TransportJobDto> AddMovementAsync(Guid jobId, AddMovementDto dto, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        var movement = _mapper.Map<TransportMovement>(dto);
        movement.TransportRequestId = jobId;
        movement.CreatedBy = userId;
        movement.CreatedDate = DateTime.UtcNow;

        await _movementRepo.AddAsync(movement);

        // Auto-advance to InTransit on first movement
        if (job.Status == TransportStatus.RateApproval && dto.Milestone == MovementMilestone.Dispatched)
        {
            job.Status = TransportStatus.InTransit;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
            await AdvanceWorkflowAsync(job, userId, "Vehicle dispatched");
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportJobDto>(job);
    }

    public async Task<TransportJobDto> RecordDeliveryAsync(Guid jobId, RecordDeliveryDto dto, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        if (job.Status != TransportStatus.InTransit && job.Status != TransportStatus.InWarehouse)
            throw new InvalidOperationException("Delivery can only be recorded for in-transit or in-warehouse jobs");

        var delivery = _mapper.Map<TransportDelivery>(dto);
        delivery.TransportRequestId = jobId;
        delivery.CreatedBy = userId;
        delivery.CreatedDate = DateTime.UtcNow;

        await _deliveryRepo.AddAsync(delivery);

        job.Status = TransportStatus.Delivered;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        await _unitOfWork.SaveChangesAsync();
        await AdvanceWorkflowAsync(job, userId, "Delivery recorded");
        NotifyFreightIfApplicable(job, "TRANSPORT_DELIVERED", "Transport delivery recorded");

        return _mapper.Map<TransportJobDto>(job);
    }

    public async Task<TransportJobDto> ClearJobAsync(Guid jobId, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        if (job.Status != TransportStatus.Delivered)
            throw new InvalidOperationException("Only delivered jobs can be cleared");

        // ── Clearance checklist validation ──────────────────────
        var checklist = await BuildClearanceChecklistAsync(job);
        if (!checklist.IsEligibleForClearance)
            throw new InvalidOperationException(
                $"Clearance blocked — missing: {string.Join(", ", checklist.MissingItems)}");

        job.Status = TransportStatus.Cleared;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        await _unitOfWork.SaveChangesAsync();
        await AdvanceWorkflowAsync(job, userId, "Job cleared — ready for billing");
        NotifyFreightIfApplicable(job, "TRANSPORT_COMPLETED", "Transport job cleared — ready for billing");

        return _mapper.Map<TransportJobDto>(job);
    }

    // ════════════════════════════════════════════════════════════
    //  EXPENSES
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<TransportExpenseDto>> GetExpensesAsync(Guid jobId)
    {
        var expenses = await _expenseRepo.FindAsync(e => e.TransportRequestId == jobId);
        return _mapper.Map<IEnumerable<TransportExpenseDto>>(expenses);
    }

    public async Task<ExpenseSummaryDto> GetExpenseSummaryAsync(Guid jobId)
    {
        var expenses = await _expenseRepo.FindAsync(e => e.TransportRequestId == jobId);
        var list = expenses.ToList();

        var summary = new ExpenseSummaryDto
        {
            TransportRequestId = jobId,
            TotalAmount = list.Sum(e => e.Amount),
            CurrencyCode = list.FirstOrDefault()?.CurrencyCode ?? "INR",
            ExpenseCount = list.Count
        };

        var groups = list.GroupBy(e => e.Category);
        foreach (var g in groups)
        {
            summary.CategoryBreakdown.Add(new ExpenseCategoryTotalDto
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount),
                Count = g.Count()
            });
        }

        return summary;
    }

    public async Task<TransportExpenseDto> AddExpenseAsync(
        Guid jobId, CreateExpenseDto dto, Guid userId)
    {
        // Verify job exists
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        var entity = _mapper.Map<TransportExpense>(dto);
        entity.TransportRequestId = jobId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _expenseRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportExpenseDto>(entity);
    }

    public async Task<TransportExpenseDto> UpdateExpenseAsync(
        Guid jobId, Guid expenseId, UpdateExpenseDto dto, Guid userId)
    {
        var entity = await _expenseRepo.GetByIdAsync(expenseId)
            ?? throw new KeyNotFoundException($"Expense {expenseId} not found");

        if (entity.TransportRequestId != jobId)
            throw new InvalidOperationException("Expense does not belong to this job");

        if (dto.Category.HasValue) entity.Category = dto.Category.Value;
        if (dto.CategoryDescription != null) entity.CategoryDescription = dto.CategoryDescription;
        if (dto.Amount.HasValue) entity.Amount = dto.Amount.Value;
        if (dto.CurrencyCode != null) entity.CurrencyCode = dto.CurrencyCode;
        if (dto.ExpenseDate.HasValue) entity.ExpenseDate = dto.ExpenseDate.Value;
        if (dto.Remarks != null) entity.Remarks = dto.Remarks;
        if (dto.ReceiptUrl != null) entity.ReceiptUrl = dto.ReceiptUrl;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _expenseRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportExpenseDto>(entity);
    }

    public async Task DeleteExpenseAsync(Guid jobId, Guid expenseId, Guid userId)
    {
        var entity = await _expenseRepo.GetByIdAsync(expenseId)
            ?? throw new KeyNotFoundException($"Expense {expenseId} not found");

        if (entity.TransportRequestId != jobId)
            throw new InvalidOperationException("Expense does not belong to this job");

        entity.UpdatedBy = userId;
        _expenseRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  FUND REQUESTS
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<VehicleFundRequestDto>> GetFundRequestsAsync(Guid jobId)
    {
        // Get vehicles for this job, then fund requests for those vehicles
        var vehicles = await _vehicleRepo.FindAsync(v => v.TransportRequestId == jobId);
        var vehicleIds = vehicles.Select(v => v.Id).ToList();

        var requests = new List<VehicleFundRequest>();
        foreach (var vId in vehicleIds)
        {
            var vRequests = await _fundRequestRepo.FindAsync(f => f.TransportVehicleId == vId);
            requests.AddRange(vRequests);
        }

        return _mapper.Map<IEnumerable<VehicleFundRequestDto>>(requests);
    }

    public async Task<VehicleFundRequestDto> CreateFundRequestAsync(
        Guid jobId, Guid vehicleId, CreateFundRequestDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Vehicle assignment {vehicleId} not found");

        if (vehicle.TransportRequestId != jobId)
            throw new InvalidOperationException("Vehicle does not belong to this job");

        // Check for existing pending request
        var existing = await _fundRequestRepo.FirstOrDefaultAsync(
            f => f.TransportVehicleId == vehicleId && f.Status == FundRequestStatus.Pending);
        if (existing != null)
            throw new InvalidOperationException("A pending fund request already exists for this vehicle");

        var entity = _mapper.Map<VehicleFundRequest>(dto);
        entity.TransportVehicleId = vehicleId;
        entity.Status = FundRequestStatus.Pending;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _fundRequestRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleFundRequestDto>(entity);
    }

    public async Task<VehicleFundRequestDto> ProcessFundRequestAsync(
        Guid fundRequestId, ProcessFundRequestDto dto, Guid userId)
    {
        var entity = await _fundRequestRepo.GetByIdAsync(fundRequestId)
            ?? throw new KeyNotFoundException($"Fund request {fundRequestId} not found");

        if (entity.Status != FundRequestStatus.Pending)
            throw new InvalidOperationException("Only pending fund requests can be processed");

        entity.Status = dto.Action;
        entity.Remarks = dto.Remarks;
        entity.ProcessedBy = userId;
        entity.ProcessedDate = DateTime.UtcNow;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _fundRequestRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleFundRequestDto>(entity);
    }

    // ════════════════════════════════════════════════════════════
    //  CLEARANCE CHECKLIST
    // ════════════════════════════════════════════════════════════

    public async Task<ClearanceChecklistDto> GetClearanceChecklistAsync(Guid jobId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        return await BuildClearanceChecklistAsync(job);
    }

    private async Task<ClearanceChecklistDto> BuildClearanceChecklistAsync(TransportRequest job)
    {
        var checklist = new ClearanceChecklistDto { JobId = job.Id };

        // Check POD document
        var hasPod = await _documentRepo.AnyAsync(
            d => d.TransportRequestId == job.Id && d.DocumentType == DocumentType.POD);
        checklist.HasPODDocument = hasPod;
        if (!hasPod) checklist.MissingItems.Add("POD document");

        // Check LR document
        var hasLr = await _documentRepo.AnyAsync(
            d => d.TransportRequestId == job.Id && d.DocumentType == DocumentType.LR);
        checklist.HasLRDocument = hasLr;
        if (!hasLr) checklist.MissingItems.Add("LR document");

        // Check Challan document
        var hasChallan = await _documentRepo.AnyAsync(
            d => d.TransportRequestId == job.Id && d.DocumentType == DocumentType.Challan);
        checklist.HasChallanDocument = hasChallan;
        if (!hasChallan) checklist.MissingItems.Add("Challan document");

        // Check rate entry
        var vehicles = await _vehicleRepo.FindAsync(v => v.TransportRequestId == job.Id);
        var vehicleIds = vehicles.Select(v => v.Id).ToList();
        var hasRate = false;
        var hasRateApproval = false;
        foreach (var vId in vehicleIds)
        {
            var rates = await _rateRepo.FindAsync(r => r.TransportVehicleId == vId);
            if (rates.Any())
            {
                hasRate = true;
                if (rates.Any(r => r.IsApproved))
                    hasRateApproval = true;
            }
        }
        checklist.HasRateEntry = hasRate;
        if (!hasRate) checklist.MissingItems.Add("Rate entry");
        checklist.HasRateApproval = hasRateApproval;
        if (hasRate && !hasRateApproval) checklist.MissingItems.Add("Rate approval");

        // Check delivery record
        var hasDelivery = await _deliveryRepo.AnyAsync(d => d.TransportRequestId == job.Id);
        checklist.HasDeliveryRecord = hasDelivery;
        if (!hasDelivery) checklist.MissingItems.Add("Delivery record");

        checklist.IsEligibleForClearance = checklist.MissingItems.Count == 0;
        return checklist;
    }

    // ════════════════════════════════════════════════════════════
    //  WORKFLOW CALLBACK
    // ════════════════════════════════════════════════════════════

    public async Task HandleWorkflowCallbackAsync(Guid jobId, WorkflowCallbackDto dto)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        _logger.LogInformation(
            "Workflow callback for job {JobId}: Action={Action}, StepCode={StepCode}",
            jobId, dto.Action, dto.StepCode);

        job.WorkflowStatus = dto.NewStatus;

        switch (dto.Action.ToLowerInvariant())
        {
            case "approved":
                await HandleWorkflowApprovalAsync(job, dto);
                break;

            case "rejected":
                await HandleWorkflowRejectionAsync(job, dto);
                break;

            default:
                _logger.LogWarning("Unknown workflow action '{Action}' for job {JobId}", dto.Action, jobId);
                break;
        }

        _jobRepo.Update(job);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleWorkflowApprovalAsync(TransportRequest job, WorkflowCallbackDto dto)
    {
        // Rate approval step → advance to InTransit-ready
        if (dto.StepCode?.Contains("RATE", StringComparison.OrdinalIgnoreCase) == true ||
            job.Status == TransportStatus.RateApproval)
        {
            // Mark rates as approved
            var vehicles = await _vehicleRepo.FindAsync(v => v.TransportRequestId == job.Id);
            foreach (var v in vehicles)
            {
                var rates = await _rateRepo.FindAsync(r => r.TransportVehicleId == v.Id && !r.IsApproved);
                foreach (var rate in rates)
                {
                    rate.IsApproved = true;
                    rate.ApprovedAmount = rate.TotalRate;
                    rate.ApprovalRemarks = dto.Remarks;
                    rate.UpdatedDate = DateTime.UtcNow;
                    _rateRepo.Update(rate);
                }
            }

            _logger.LogInformation("Rates approved for job {JobId} via workflow callback", job.Id);
        }
    }

    private Task HandleWorkflowRejectionAsync(TransportRequest job, WorkflowCallbackDto dto)
    {
        // Rate rejection → roll back to RateEntered so user can re-enter
        if (dto.StepCode?.Contains("RATE", StringComparison.OrdinalIgnoreCase) == true ||
            job.Status == TransportStatus.RateApproval)
        {
            job.Status = TransportStatus.RateEntered;
            _logger.LogInformation(
                "Rate rejected for job {JobId}, rolled back to RateEntered. Reason: {Reason}",
                job.Id, dto.Remarks);
        }

        return Task.CompletedTask;
    }

    // ════════════════════════════════════════════════════════════
    //  DOCUMENTS
    // ════════════════════════════════════════════════════════════

    public async Task<TransportDocumentDto> UploadDocumentAsync(
        Guid jobId, UploadDocumentDto dto, Guid userId)
    {
        var doc = _mapper.Map<TransportDocument>(dto);
        doc.TransportRequestId = jobId;
        doc.CreatedBy = userId;
        doc.CreatedDate = DateTime.UtcNow;

        await _documentRepo.AddAsync(doc);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportDocumentDto>(doc);
    }

    // ════════════════════════════════════════════════════════════
    //  CONSOLIDATION
    // ════════════════════════════════════════════════════════════

    public async Task<ConsolidatedTripDto> ConsolidateJobsAsync(ConsolidateJobsDto dto, Guid userId)
    {
        if (dto.JobIds.Count < 2)
            throw new InvalidOperationException("At least 2 jobs are required for consolidation");

        // ── Load and validate all jobs ──────────────────────────
        var jobs = new List<TransportRequest>();
        foreach (var jobId in dto.JobIds)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

            if (job.Status != TransportStatus.RequestCreated && job.Status != TransportStatus.RequestReceived)
                throw new InvalidOperationException($"Job {job.RequestNumber} is not in a consolidatable state");

            if (job.IsConsolidated)
                throw new InvalidOperationException($"Job {job.RequestNumber} is already part of a consolidated trip");

            jobs.Add(job);
        }

        // ── Destination validation ──────────────────────────────
        var destinations = jobs
            .Where(j => j.DestinationLocationId.HasValue)
            .Select(j => j.DestinationLocationId!.Value)
            .Distinct()
            .ToList();

        if (destinations.Count > 1)
            throw new InvalidOperationException(
                "All jobs must share the same destination for consolidation");

        // ── Create ConsolidatedTrip entity ──────────────────────
        var firstJob = jobs.First();
        var year = DateTime.UtcNow.Year;
        var count = await _consolidatedTripRepo.CountAsync(c => c.CountryCode == firstJob.CountryCode);

        var trip = new ConsolidatedTrip
        {
            ReferenceNumber = $"CT-{year}-{firstJob.CountryCode}-{(count + 1):D4}",
            DestinationLocationId = firstJob.DestinationLocationId,
            DestinationLocationName = firstJob.DestinationLocationName,
            Status = Domain.Enums.ConsolidationStatus.Draft,
            Remarks = dto.Remarks,
            JobCount = jobs.Count,
            CountryCode = firstJob.CountryCode,
            BranchId = firstJob.BranchId,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _consolidatedTripRepo.AddAsync(trip);

        // ── Link jobs to trip + create stop deliveries ──────────
        var stopSeq = 1;
        foreach (var job in jobs)
        {
            job.ConsolidatedTripId = trip.Id;
            job.IsConsolidated = true;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);

            // Auto-create a stop delivery entry per child job
            var stop = new ConsolidatedStopDelivery
            {
                ConsolidatedTripId = trip.Id,
                TransportRequestId = job.Id,
                StopSequence = stopSeq++,
                LocationName = job.DestinationLocationName,
                Address = job.DropAddress,
                City = job.DropCity,
                Pincode = job.DropPincode,
                EstimatedArrival = job.RequiredDeliveryDate,
                DeliveryStatus = Domain.Enums.DeliveryStatus.Pending,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };
            await _stopDeliveryRepo.AddAsync(stop);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Consolidated {Count} jobs into trip {TripRef}",
            jobs.Count, trip.ReferenceNumber);

        var result = _mapper.Map<ConsolidatedTripDto>(trip);
        result.Jobs = _mapper.Map<ICollection<TransportJobListDto>>(jobs);
        return result;
    }

    public async Task<ConsolidatedTripDto?> GetConsolidatedTripAsync(Guid tripId)
    {
        var trip = await _consolidatedTripRepo.GetByIdAsync(tripId);
        if (trip == null) return null;

        var dto = _mapper.Map<ConsolidatedTripDto>(trip);
        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        dto.Jobs = _mapper.Map<ICollection<TransportJobListDto>>(jobs);
        return dto;
    }

    public async Task<ConsolidatedTripDto> AssignConsolidatedVehicleAsync(
        Guid tripId, AssignConsolidatedVehicleDto dto, Guid userId)
    {
        var trip = await _consolidatedTripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        trip.SharedVehicleId = dto.VehicleId;
        trip.SharedVehicleNumber = dto.VehicleNumber;
        trip.Status = Domain.Enums.ConsolidationStatus.VehicleAssigned;
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;

        _consolidatedTripRepo.Update(trip);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<ConsolidatedTripDto>(trip);
        var jobs = await _jobRepo.FindAsync(j => j.ConsolidatedTripId == tripId);
        result.Jobs = _mapper.Map<ICollection<TransportJobListDto>>(jobs);
        return result;
    }

    public async Task<ConsolidatedVehicleDto> AddConsolidatedVehicleAsync(
        Guid tripId, CreateConsolidatedVehicleDto dto, Guid userId)
    {
        var trip = await _consolidatedTripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        var entity = _mapper.Map<ConsolidatedVehicle>(dto);
        entity.ConsolidatedTripId = tripId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        // Auto-compute total
        entity.TotalRate = (entity.FreightRate ?? 0) + (entity.TollCharges ?? 0) + (entity.OtherCharges ?? 0);

        await _consolidatedVehicleRepo.AddAsync(entity);

        // Also update the trip's shared vehicle for backward compatibility
        trip.SharedVehicleNumber = dto.VehicleNumber;
        trip.Status = Domain.Enums.ConsolidationStatus.VehicleAssigned;
        trip.UpdatedBy = userId;
        trip.UpdatedDate = DateTime.UtcNow;
        _consolidatedTripRepo.Update(trip);

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ConsolidatedVehicleDto>(entity);
    }

    public async Task<ConsolidatedExpenseDto> AddConsolidatedExpenseAsync(
        Guid tripId, CreateConsolidatedExpenseDto dto, Guid userId)
    {
        _ = await _consolidatedTripRepo.GetByIdAsync(tripId)
            ?? throw new KeyNotFoundException($"Consolidated trip {tripId} not found");

        var entity = _mapper.Map<ConsolidatedExpense>(dto);
        entity.ConsolidatedTripId = tripId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _consolidatedExpenseRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ConsolidatedExpenseDto>(entity);
    }

    public async Task<IEnumerable<ConsolidatedExpenseDto>> GetConsolidatedExpensesAsync(Guid tripId)
    {
        var expenses = await _consolidatedExpenseRepo.FindAsync(e => e.ConsolidatedTripId == tripId);
        return _mapper.Map<IEnumerable<ConsolidatedExpenseDto>>(expenses);
    }

    public async Task<ConsolidatedStopDeliveryDto> RecordStopDeliveryAsync(
        Guid tripId, Guid stopId, RecordStopDeliveryDto dto, Guid userId)
    {
        var stop = await _stopDeliveryRepo.GetByIdAsync(stopId)
            ?? throw new KeyNotFoundException($"Stop delivery {stopId} not found");

        if (stop.ConsolidatedTripId != tripId)
            throw new InvalidOperationException("Stop does not belong to this consolidated trip");

        stop.ActualArrival = dto.ActualArrival;
        stop.DeliveryStatus = dto.DeliveryStatus;
        stop.ReceivedBy = dto.ReceivedBy;
        stop.PODNumber = dto.PODNumber;
        stop.PODDocumentUrl = dto.PODDocumentUrl;
        stop.Remarks = dto.Remarks;
        stop.UpdatedBy = userId;
        stop.UpdatedDate = DateTime.UtcNow;

        _stopDeliveryRepo.Update(stop);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ConsolidatedStopDeliveryDto>(stop);
    }

    // ════════════════════════════════════════════════════════════
    //  BULK OPERATIONS
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<TransportJobListDto>> BulkReceiveAsync(
        ICollection<Guid> jobIds, Guid userId)
    {
        var received = new List<TransportRequest>();

        foreach (var jobId in jobIds)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null || job.Status != TransportStatus.RequestCreated)
            {
                _logger.LogWarning("Skipping job {JobId} — not found or not in RequestCreated state", jobId);
                continue;
            }

            job.Status = TransportStatus.RequestReceived;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
            received.Add(job);
        }

        await _unitOfWork.SaveChangesAsync();

        // Advance workflow for each (non-fatal)
        foreach (var job in received)
        {
            await AdvanceWorkflowAsync(job, userId, "Bulk received by operations");
        }

        return _mapper.Map<IEnumerable<TransportJobListDto>>(received);
    }

    public async Task<IEnumerable<TransportJobListDto>> BulkClearAsync(
        ICollection<Guid> jobIds, Guid userId)
    {
        var cleared = new List<TransportRequest>();
        var failures = new List<string>();

        foreach (var jobId in jobIds)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null)
            {
                failures.Add($"{jobId}: not found");
                continue;
            }
            if (job.Status != TransportStatus.Delivered)
            {
                failures.Add($"{jobId}: status is {job.Status}, expected Delivered");
                continue;
            }

            var checklist = await BuildClearanceChecklistAsync(job);
            if (!checklist.IsEligibleForClearance)
            {
                failures.Add($"{jobId}: missing {string.Join(", ", checklist.MissingItems)}");
                continue;
            }

            job.Status = TransportStatus.Cleared;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
            cleared.Add(job);
        }

        if (failures.Any())
            _logger.LogWarning("Bulk clearance skipped {Count} jobs: {Failures}",
                failures.Count, string.Join("; ", failures));

        await _unitOfWork.SaveChangesAsync();

        foreach (var job in cleared)
        {
            await AdvanceWorkflowAsync(job, userId, "Bulk cleared — ready for billing");
        }

        return _mapper.Map<IEnumerable<TransportJobListDto>>(cleared);
    }

    // ════════════════════════════════════════════════════════════
    //  INTERNAL (MS-to-MS)
    // ════════════════════════════════════════════════════════════

    public async Task<TransportJobStatusDto> GetJobStatusAsync(Guid jobId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new KeyNotFoundException($"Transport job {jobId} not found");

        return _mapper.Map<TransportJobStatusDto>(job);
    }

    // ════════════════════════════════════════════════════════════
    //  TRANSIT WAREHOUSE (Legacy: InTransitWarehouse.aspx)
    // ════════════════════════════════════════════════════════════

    public async Task<TransitWarehouseDto> ArriveAtWarehouseAsync(Guid jobId, ArriveAtWarehouseDto dto, Guid userId)
    {
        var job = await _jobRepo.GetByIdAsync(jobId)
            ?? throw new TransportNotFoundException("TransportRequest", jobId);

        if (job.Status != TransportStatus.InTransit)
            throw new TransportBusinessException($"Job must be InTransit to arrive at warehouse. Current: {job.Status}");

        var entity = new TransitWarehouse
        {
            Id = Guid.NewGuid(),
            TransportRequestId = jobId,
            TransportVehicleId = dto.TransportVehicleId,
            WarehouseName = dto.WarehouseName,
            WarehouseAddress = dto.WarehouseAddress,
            WarehouseCity = dto.WarehouseCity,
            WarehouseState = dto.WarehouseState,
            WarehousePincode = dto.WarehousePincode,
            ArrivalDate = DateTime.UtcNow,
            ArrivalRemarks = dto.ArrivalRemarks,
            ReceivedBy = dto.ReceivedBy,
            ContainerId = dto.ContainerId,
            ContainerSealNumber = dto.ContainerSealNumber,
            IsDispatched = false,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _transitWarehouseRepo.AddAsync(entity);

        // Transition job to InWarehouse status
        job.Status = TransportStatus.InWarehouse;
        job.UpdatedBy = userId;
        job.UpdatedDate = DateTime.UtcNow;
        _jobRepo.Update(job);

        await _unitOfWork.SaveChangesAsync();
        await TryAdvanceWorkflowAsync(job, userId, $"Arrived at warehouse: {dto.WarehouseName}");

        _logger.LogInformation("Job {JobId} arrived at warehouse {Warehouse}", jobId, dto.WarehouseName);

        return MapWarehouseDto(entity);
    }

    public async Task<TransitWarehouseDto> DispatchFromWarehouseAsync(Guid jobId, Guid warehouseId, DispatchFromWarehouseDto dto, Guid userId)
    {
        var entity = await _transitWarehouseRepo.FirstOrDefaultAsync(w =>
            w.Id == warehouseId && w.TransportRequestId == jobId && !w.IsDeleted)
            ?? throw new TransportNotFoundException("TransitWarehouse", warehouseId);

        if (entity.IsDispatched)
            throw new TransportBusinessException("Already dispatched from this warehouse");

        entity.DepartureDate = DateTime.UtcNow;
        entity.DepartureRemarks = dto.DepartureRemarks;
        entity.DispatchedBy = dto.DispatchedBy;
        entity.IsDispatched = true;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _transitWarehouseRepo.Update(entity);

        // Transition back to InTransit (moving to next warehouse or delivery)
        var job = await _jobRepo.GetByIdAsync(jobId);
        if (job != null)
        {
            job.Status = TransportStatus.InTransit;
            job.UpdatedBy = userId;
            job.UpdatedDate = DateTime.UtcNow;
            _jobRepo.Update(job);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Job {JobId} dispatched from warehouse {WarehouseId}", jobId, warehouseId);
        return MapWarehouseDto(entity);
    }

    public async Task<IEnumerable<TransitWarehouseDto>> GetTransitWarehousesAsync(Guid jobId)
    {
        var items = await _transitWarehouseRepo.FindAsync(w => w.TransportRequestId == jobId && !w.IsDeleted);
        return items.OrderBy(w => w.ArrivalDate).Select(MapWarehouseDto);
    }

    // ════════════════════════════════════════════════════════════
    //  EXPENSE APPROVAL (Legacy: ApproveExpense.aspx)
    // ════════════════════════════════════════════════════════════

    public async Task<ExpenseApprovalDto> SubmitExpenseForApprovalAsync(Guid jobId, Guid expenseId, SubmitExpenseForApprovalDto dto, Guid userId)
    {
        var expense = await _expenseRepo.FirstOrDefaultAsync(e =>
            e.Id == expenseId && e.TransportRequestId == jobId && !e.IsDeleted)
            ?? throw new TransportNotFoundException("TransportExpense", expenseId);

        expense.RequiresApproval = true;
        expense.ApprovalStatus = ExpenseApprovalStatus.Pending;
        expense.UpdatedBy = userId;
        expense.UpdatedDate = DateTime.UtcNow;
        _expenseRepo.Update(expense);

        var approval = new ExpenseApproval
        {
            Id = Guid.NewGuid(),
            TransportExpenseId = expenseId,
            Status = ExpenseApprovalStatus.Pending,
            RequestedAmount = expense.Amount,
            Remarks = dto.Remarks,
            ApprovalLevel = 1,
            ApprovalRole = "HOD",
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _expenseApprovalRepo.AddAsync(approval);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Expense {ExpenseId} submitted for approval: {Amount}", expenseId, expense.Amount);
        return MapApprovalDto(approval);
    }

    public async Task<ExpenseApprovalDto> ApproveExpenseAsync(Guid expenseId, ApproveExpenseDto dto, Guid userId)
    {
        var approval = await _expenseApprovalRepo.FirstOrDefaultAsync(a =>
            a.TransportExpenseId == expenseId && a.Status == ExpenseApprovalStatus.Pending && !a.IsDeleted)
            ?? throw new TransportNotFoundException("ExpenseApproval", expenseId);

        approval.Status = ExpenseApprovalStatus.Approved;
        approval.ApprovedAmount = dto.ApprovedAmount;
        approval.Remarks = dto.Remarks;
        approval.ApprovedBy = userId;
        approval.ApprovedDate = DateTime.UtcNow;
        approval.UpdatedBy = userId;
        approval.UpdatedDate = DateTime.UtcNow;
        _expenseApprovalRepo.Update(approval);

        // Update the expense's approval status
        var expense = await _expenseRepo.GetByIdAsync(approval.TransportExpenseId);
        if (expense != null)
        {
            expense.ApprovalStatus = dto.ApprovedAmount < expense.Amount
                ? ExpenseApprovalStatus.PartiallyApproved
                : ExpenseApprovalStatus.Approved;
            expense.UpdatedBy = userId;
            expense.UpdatedDate = DateTime.UtcNow;
            _expenseRepo.Update(expense);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Expense {ExpenseId} approved: {Amount}", expenseId, dto.ApprovedAmount);
        return MapApprovalDto(approval);
    }

    public async Task<ExpenseApprovalDto> RejectExpenseAsync(Guid expenseId, RejectExpenseDto dto, Guid userId)
    {
        var approval = await _expenseApprovalRepo.FirstOrDefaultAsync(a =>
            a.TransportExpenseId == expenseId && a.Status == ExpenseApprovalStatus.Pending && !a.IsDeleted)
            ?? throw new TransportNotFoundException("ExpenseApproval", expenseId);

        approval.Status = ExpenseApprovalStatus.Rejected;
        approval.RejectionReason = dto.RejectionReason;
        approval.ApprovedBy = userId;
        approval.ApprovedDate = DateTime.UtcNow;
        approval.UpdatedBy = userId;
        approval.UpdatedDate = DateTime.UtcNow;
        _expenseApprovalRepo.Update(approval);

        var expense = await _expenseRepo.GetByIdAsync(approval.TransportExpenseId);
        if (expense != null)
        {
            expense.ApprovalStatus = ExpenseApprovalStatus.Rejected;
            expense.UpdatedBy = userId;
            expense.UpdatedDate = DateTime.UtcNow;
            _expenseRepo.Update(expense);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Expense {ExpenseId} rejected: {Reason}", expenseId, dto.RejectionReason);
        return MapApprovalDto(approval);
    }

    public async Task<IEnumerable<PendingExpenseApprovalDto>> GetPendingExpenseApprovalsAsync(Guid userId)
    {
        var pendingApprovals = await _expenseApprovalRepo.FindAsync(a =>
            a.Status == ExpenseApprovalStatus.Pending && !a.IsDeleted);

        var result = new List<PendingExpenseApprovalDto>();
        foreach (var approval in pendingApprovals)
        {
            var expense = await _expenseRepo.GetByIdAsync(approval.TransportExpenseId);
            if (expense == null) continue;

            var job = await _jobRepo.GetByIdAsync(expense.TransportRequestId);

            result.Add(new PendingExpenseApprovalDto
            {
                ExpenseId = expense.Id,
                JobId = expense.TransportRequestId,
                RequestNumber = job?.RequestNumber,
                Category = expense.Category.ToString(),
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Remarks = expense.Remarks,
                ApprovalStatus = approval.Status,
                PendingLevel = approval.ApprovalLevel
            });
        }

        return result;
    }

    // ════════════════════════════════════════════════════════════
    //  PRIVATE HELPERS
    // ════════════════════════════════════════════════════════════

    private async Task<string> GenerateRequestNumberAsync(string countryCode)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _jobRepo.CountAsync(j =>
            j.CountryCode == countryCode && j.RequestDate.Year == year);
        return $"TR-{year}-{countryCode}-{(count + 1):D6}";
    }

    private async Task StartWorkflowAsync(TransportRequest entity, Guid userId)
    {
        try
        {
            // Step 1: Lookup template to get the active WorkflowVersionId
            var template = await _workflowClient.LookupTemplateAsync("Transport", "Job", "Create", entity.CountryCode);
            if (template?.ActiveVersionId == null)
            {
                _logger.LogWarning("No active workflow template found for Transport/Job/Create/{CountryCode}. Job created without workflow.",
                    entity.CountryCode);
                return;
            }

            // Step 2: Create workflow instance using WorkflowVersionId
            var result = await _workflowClient.CreateInstanceAsync(userId, new CreateWorkflowInstanceDto
            {
                WorkflowVersionId = template.ActiveVersionId.Value,
                BusinessKey = $"TRANSPORT:Job:{entity.Id}",
                InitialAssigneeUserId = userId
            });

            if (result != null)
            {
                entity.WorkflowInstanceId = result.InstanceId;
                entity.WorkflowStatus = result.Status;
                entity.WorkflowStepId = result.CurrentStepId;
                _jobRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to start workflow for job {JobId}. Job created without workflow.", entity.Id);
        }
    }

    private async Task AdvanceWorkflowAsync(TransportRequest entity, Guid userId, string remarks)
    {
        if (!entity.WorkflowInstanceId.HasValue) return;

        try
        {
            var result = await _workflowClient.AdvanceAsync(entity.WorkflowInstanceId.Value,
                userId, new AdvanceWorkflowDto { Remarks = remarks });

            if (result != null)
            {
                entity.WorkflowStatus = result.Status;
                entity.WorkflowStepId = result.CurrentStepId;
                _jobRepo.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to advance workflow for job {JobId}", entity.Id);
        }
    }

    /// <summary>
    /// Sends a fire-and-forget callback to Freight MS if this transport job originated from a freight job.
    /// </summary>
    private void NotifyFreightIfApplicable(TransportRequest job, string eventName, string? remarks = null)
    {
        if (job.Source != JobSource.FreightJob || !job.SourceReferenceId.HasValue)
            return;

        var freightJobId = job.SourceReferenceId.Value;
        var payload = new TransportCallbackPayload
        {
            Event = eventName,
            TransportJobNumber = job.RequestNumber,
            Status = job.Status.ToString(),
            Remarks = remarks
        };

        _ = Task.Run(async () =>
        {
            try
            {
                await _freightClient.SendCallbackAsync(freightJobId, payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to notify Freight for transport job {RequestNumber}", job.RequestNumber);
            }
        });
    }

    private static System.Linq.Expressions.Expression<Func<TransportRequest, bool>> BuildJobPredicate(
        TransportJobFilterDto filter)
    {
        return j =>
            (filter.Status == null || j.Status == filter.Status) &&
            (filter.Priority == null || j.Priority == filter.Priority) &&
            (filter.Source == null || j.Source == filter.Source) &&
            (filter.CustomerId == null || j.CustomerId == filter.CustomerId) &&
            (filter.BranchId == null || j.BranchId == filter.BranchId) &&
            (filter.CountryCode == null || j.CountryCode == filter.CountryCode) &&
            (filter.OriginLocationId == null || j.OriginLocationId == filter.OriginLocationId) &&
            (filter.DestinationLocationId == null || j.DestinationLocationId == filter.DestinationLocationId) &&
            (filter.IsConsolidated == null || j.IsConsolidated == filter.IsConsolidated) &&
            (filter.FromDate == null || j.RequestDate >= filter.FromDate) &&
            (filter.ToDate == null || j.RequestDate <= filter.ToDate) &&
            (filter.Search == null ||
                j.RequestNumber.Contains(filter.Search) ||
                j.CustomerName.Contains(filter.Search));
    }

    private async Task TryAdvanceWorkflowAsync(TransportRequest entity, Guid userId, string remarks)
    {
        try { await AdvanceWorkflowAsync(entity, userId, remarks); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Workflow advance failed for job {JobId}", entity.Id);
        }
    }

    private static TransitWarehouseDto MapWarehouseDto(TransitWarehouse w) => new()
    {
        Id = w.Id,
        TransportRequestId = w.TransportRequestId,
        TransportVehicleId = w.TransportVehicleId,
        WarehouseName = w.WarehouseName,
        WarehouseAddress = w.WarehouseAddress,
        WarehouseCity = w.WarehouseCity,
        WarehouseState = w.WarehouseState,
        WarehousePincode = w.WarehousePincode,
        ArrivalDate = w.ArrivalDate,
        ArrivalRemarks = w.ArrivalRemarks,
        ReceivedBy = w.ReceivedBy,
        DepartureDate = w.DepartureDate,
        DepartureRemarks = w.DepartureRemarks,
        DispatchedBy = w.DispatchedBy,
        ContainerId = w.ContainerId,
        ContainerSealNumber = w.ContainerSealNumber,
        IsDispatched = w.IsDispatched
    };

    private static ExpenseApprovalDto MapApprovalDto(ExpenseApproval a) => new()
    {
        Id = a.Id,
        TransportExpenseId = a.TransportExpenseId,
        Status = a.Status,
        RequestedAmount = a.RequestedAmount,
        ApprovedAmount = a.ApprovedAmount,
        Remarks = a.Remarks,
        RejectionReason = a.RejectionReason,
        ApprovedBy = a.ApprovedBy,
        ApproverName = a.ApproverName,
        ApprovedDate = a.ApprovedDate,
        ApprovalLevel = a.ApprovalLevel,
        ApprovalRole = a.ApprovalRole
    };
}
