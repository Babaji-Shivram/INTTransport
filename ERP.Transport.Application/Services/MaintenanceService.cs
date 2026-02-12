using AutoMapper;
using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.Interfaces;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Maintenance work order service — CRUD + parts + scheduling.
/// </summary>
public class MaintenanceService : IMaintenanceService
{
    private readonly IRepository<MaintenanceWorkOrder> _workOrderRepo;
    private readonly IRepository<MaintenancePart> _partRepo;
    private readonly IRepository<MaintenanceDocument> _documentRepo;
    private readonly IRepository<FleetVehicle> _vehicleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MaintenanceService> _logger;

    public MaintenanceService(
        IRepository<MaintenanceWorkOrder> workOrderRepo,
        IRepository<MaintenancePart> partRepo,
        IRepository<MaintenanceDocument> documentRepo,
        IRepository<FleetVehicle> vehicleRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MaintenanceService> logger)
    {
        _workOrderRepo = workOrderRepo;
        _partRepo = partRepo;
        _documentRepo = documentRepo;
        _vehicleRepo = vehicleRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // ════════════════════════════════════════════════════════════
    //  WORK ORDER CRUD
    // ════════════════════════════════════════════════════════════

    public async Task<MaintenanceWorkOrderDto> CreateWorkOrderAsync(
        CreateMaintenanceWorkOrderDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(dto.FleetVehicleId)
            ?? throw new KeyNotFoundException($"Fleet vehicle {dto.FleetVehicleId} not found");

        var year = DateTime.UtcNow.Year;
        var count = await _workOrderRepo.CountAsync(w => w.CountryCode == dto.CountryCode);

        var entity = _mapper.Map<MaintenanceWorkOrder>(dto);
        entity.WorkOrderNumber = $"MWO-{year}-{dto.CountryCode}-{(count + 1):D5}";
        entity.Status = MaintenanceStatus.Scheduled;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _workOrderRepo.AddAsync(entity);

        // Update vehicle status if maintenance is immediate
        if (dto.ScheduledDate.Date <= DateTime.UtcNow.Date)
        {
            vehicle.CurrentStatus = FleetVehicleStatus.UnderMaintenance;
            vehicle.UpdatedBy = userId;
            vehicle.UpdatedDate = DateTime.UtcNow;
            _vehicleRepo.Update(vehicle);
        }

        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<MaintenanceWorkOrderDto>(entity);
        result.VehicleRegistration = vehicle.RegistrationNumber;
        return result;
    }

    public async Task<MaintenanceWorkOrderDto?> GetWorkOrderByIdAsync(Guid id)
    {
        var entity = await _workOrderRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var dto = _mapper.Map<MaintenanceWorkOrderDto>(entity);
        var vehicle = await _vehicleRepo.GetByIdAsync(entity.FleetVehicleId);
        dto.VehicleRegistration = vehicle?.RegistrationNumber;
        return dto;
    }

    public async Task<PagedResultDto<MaintenanceWorkOrderListDto>> GetWorkOrdersAsync(
        MaintenanceFilterDto filter)
    {
        var (items, totalCount) = await _workOrderRepo.GetPagedAsync(
            predicate: w =>
                (filter.FleetVehicleId == null || w.FleetVehicleId == filter.FleetVehicleId) &&
                (filter.MaintenanceType == null || w.MaintenanceType == filter.MaintenanceType) &&
                (filter.Status == null || w.Status == filter.Status) &&
                (filter.Priority == null || w.Priority == filter.Priority) &&
                (filter.BranchId == null || w.BranchId == filter.BranchId) &&
                (filter.CountryCode == null || w.CountryCode == filter.CountryCode) &&
                (filter.FromDate == null || w.ScheduledDate >= filter.FromDate) &&
                (filter.ToDate == null || w.ScheduledDate <= filter.ToDate),
            orderBy: q => q.OrderByDescending(w => w.ScheduledDate),
            page: filter.Page,
            pageSize: filter.PageSize);

        var dtos = _mapper.Map<IEnumerable<MaintenanceWorkOrderListDto>>(items).ToList();

        // Populate vehicle registrations
        foreach (var dto in dtos)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(
                items.First(i => i.Id == dto.Id).FleetVehicleId);
            dto.VehicleRegistration = vehicle?.RegistrationNumber;
        }

        return new PagedResultDto<MaintenanceWorkOrderListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<MaintenanceWorkOrderDto> UpdateWorkOrderAsync(
        Guid id, UpdateMaintenanceWorkOrderDto dto, Guid userId)
    {
        var entity = await _workOrderRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Work order {id} not found");

        if (entity.Status == MaintenanceStatus.Completed || entity.Status == MaintenanceStatus.Cancelled)
            throw new InvalidOperationException("Cannot update a completed or cancelled work order");

        if (dto.MaintenanceType.HasValue) entity.MaintenanceType = dto.MaintenanceType.Value;
        if (dto.Priority.HasValue) entity.Priority = dto.Priority.Value;
        if (dto.Status.HasValue) entity.Status = dto.Status.Value;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.DiagnosticNotes != null) entity.DiagnosticNotes = dto.DiagnosticNotes;
        if (dto.ScheduledDate.HasValue) entity.ScheduledDate = dto.ScheduledDate.Value;
        if (dto.StartedDate.HasValue) entity.StartedDate = dto.StartedDate;
        if (dto.EstimatedHours.HasValue) entity.EstimatedHours = dto.EstimatedHours;
        if (dto.ActualHours.HasValue) entity.ActualHours = dto.ActualHours;
        if (dto.EstimatedCost.HasValue) entity.EstimatedCost = dto.EstimatedCost.Value;
        if (dto.ActualCost.HasValue) entity.ActualCost = dto.ActualCost;
        if (dto.LaborCost.HasValue) entity.LaborCost = dto.LaborCost;
        if (dto.PartsCost.HasValue) entity.PartsCost = dto.PartsCost;
        if (dto.ServiceProviderName != null) entity.ServiceProviderName = dto.ServiceProviderName;
        if (dto.ServiceProviderContact != null) entity.ServiceProviderContact = dto.ServiceProviderContact;
        if (dto.InvoiceNumber != null) entity.InvoiceNumber = dto.InvoiceNumber;
        if (dto.InvoiceUrl != null) entity.InvoiceUrl = dto.InvoiceUrl;
        if (dto.OdometerAtService.HasValue) entity.OdometerAtService = dto.OdometerAtService;
        if (dto.NextServiceOdometer.HasValue) entity.NextServiceOdometer = dto.NextServiceOdometer;
        if (dto.NextServiceDate.HasValue) entity.NextServiceDate = dto.NextServiceDate;

        // Auto-set StartedDate when status changes to InProgress
        if (dto.Status == MaintenanceStatus.InProgress && !entity.StartedDate.HasValue)
            entity.StartedDate = DateTime.UtcNow;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _workOrderRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MaintenanceWorkOrderDto>(entity);
    }

    public async Task<MaintenanceWorkOrderDto> CompleteWorkOrderAsync(
        Guid id, CompleteMaintenanceDto dto, Guid userId)
    {
        var entity = await _workOrderRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Work order {id} not found");

        if (entity.Status == MaintenanceStatus.Completed)
            throw new InvalidOperationException("Work order is already completed");

        entity.Status = MaintenanceStatus.Completed;
        entity.CompletedDate = dto.CompletedDate;
        entity.ActualHours = dto.ActualHours;
        entity.ActualCost = dto.ActualCost;
        entity.LaborCost = dto.LaborCost;
        entity.PartsCost = dto.PartsCost;
        entity.CompletionNotes = dto.CompletionNotes;
        entity.InvoiceNumber = dto.InvoiceNumber;
        entity.InvoiceUrl = dto.InvoiceUrl;
        entity.NextServiceOdometer = dto.NextServiceOdometer;
        entity.NextServiceDate = dto.NextServiceDate;
        entity.CompletedBy = userId;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _workOrderRepo.Update(entity);

        // Restore vehicle to Available
        var vehicle = await _vehicleRepo.GetByIdAsync(entity.FleetVehicleId);
        if (vehicle != null && vehicle.CurrentStatus == FleetVehicleStatus.UnderMaintenance)
        {
            vehicle.CurrentStatus = FleetVehicleStatus.Available;
            vehicle.UpdatedBy = userId;
            vehicle.UpdatedDate = DateTime.UtcNow;
            _vehicleRepo.Update(vehicle);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Work order {WO} completed for vehicle {VehicleId}",
            entity.WorkOrderNumber, entity.FleetVehicleId);

        return _mapper.Map<MaintenanceWorkOrderDto>(entity);
    }

    public async Task CancelWorkOrderAsync(Guid id, Guid userId)
    {
        var entity = await _workOrderRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Work order {id} not found");

        if (entity.Status == MaintenanceStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed work order");

        entity.Status = MaintenanceStatus.Cancelled;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _workOrderRepo.Update(entity);

        // Restore vehicle if it was under maintenance
        var vehicle = await _vehicleRepo.GetByIdAsync(entity.FleetVehicleId);
        if (vehicle != null && vehicle.CurrentStatus == FleetVehicleStatus.UnderMaintenance)
        {
            vehicle.CurrentStatus = FleetVehicleStatus.Available;
            vehicle.UpdatedBy = userId;
            vehicle.UpdatedDate = DateTime.UtcNow;
            _vehicleRepo.Update(vehicle);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  PARTS MANAGEMENT
    // ════════════════════════════════════════════════════════════

    public async Task<MaintenancePartDto> AddPartAsync(
        Guid workOrderId, AddMaintenancePartDto dto, Guid userId)
    {
        _ = await _workOrderRepo.GetByIdAsync(workOrderId)
            ?? throw new KeyNotFoundException($"Work order {workOrderId} not found");

        var part = _mapper.Map<MaintenancePart>(dto);
        part.MaintenanceWorkOrderId = workOrderId;
        part.TotalCost = dto.Quantity * dto.UnitCost;
        part.CreatedBy = userId;
        part.CreatedDate = DateTime.UtcNow;

        await _partRepo.AddAsync(part);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MaintenancePartDto>(part);
    }

    public async Task DeletePartAsync(Guid workOrderId, Guid partId, Guid userId)
    {
        var part = await _partRepo.GetByIdAsync(partId)
            ?? throw new KeyNotFoundException($"Part {partId} not found");

        if (part.MaintenanceWorkOrderId != workOrderId)
            throw new InvalidOperationException("Part does not belong to this work order");

        part.UpdatedBy = userId;
        _partRepo.Delete(part);
        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  DOCUMENT MANAGEMENT
    // ════════════════════════════════════════════════════════════

    public async Task<MaintenanceDocumentDto> AddDocumentAsync(
        Guid workOrderId, CreateMaintenanceDocumentDto dto, Guid userId)
    {
        _ = await _workOrderRepo.GetByIdAsync(workOrderId)
            ?? throw new KeyNotFoundException($"Work order {workOrderId} not found");

        var document = _mapper.Map<MaintenanceDocument>(dto);
        document.MaintenanceWorkOrderId = workOrderId;
        document.CreatedBy = userId;
        document.CreatedDate = DateTime.UtcNow;

        await _documentRepo.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document {FileName} added to work order {WorkOrderId}",
            dto.FileName, workOrderId);

        return _mapper.Map<MaintenanceDocumentDto>(document);
    }

    public async Task<IEnumerable<MaintenanceDocumentDto>> GetDocumentsAsync(Guid workOrderId)
    {
        var documents = await _documentRepo.FindAsync(d => d.MaintenanceWorkOrderId == workOrderId);
        return _mapper.Map<IEnumerable<MaintenanceDocumentDto>>(documents);
    }

    public async Task DeleteDocumentAsync(Guid workOrderId, Guid documentId, Guid userId)
    {
        var document = await _documentRepo.GetByIdAsync(documentId)
            ?? throw new KeyNotFoundException($"Document {documentId} not found");

        if (document.MaintenanceWorkOrderId != workOrderId)
            throw new InvalidOperationException("Document does not belong to this work order");

        document.UpdatedBy = userId;
        _documentRepo.Delete(document);
        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  QUERIES
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<MaintenanceWorkOrderListDto>> GetUpcomingMaintenanceAsync(
        int daysAhead = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(daysAhead);
        var items = await _workOrderRepo.FindAsync(w =>
            w.Status == MaintenanceStatus.Scheduled &&
            w.ScheduledDate <= cutoff &&
            w.ScheduledDate >= DateTime.UtcNow.Date);

        return _mapper.Map<IEnumerable<MaintenanceWorkOrderListDto>>(items);
    }

    public async Task<IEnumerable<MaintenanceWorkOrderListDto>> GetOverdueMaintenanceAsync()
    {
        var items = await _workOrderRepo.FindAsync(w =>
            (w.Status == MaintenanceStatus.Scheduled || w.Status == MaintenanceStatus.InProgress) &&
            w.ScheduledDate < DateTime.UtcNow.Date);

        return _mapper.Map<IEnumerable<MaintenanceWorkOrderListDto>>(items);
    }

    public async Task<IEnumerable<MaintenanceWorkOrderListDto>> GetVehicleMaintenanceHistoryAsync(
        Guid vehicleId)
    {
        var items = await _workOrderRepo.FindAsync(w => w.FleetVehicleId == vehicleId);
        return _mapper.Map<IEnumerable<MaintenanceWorkOrderListDto>>(items);
    }
}
