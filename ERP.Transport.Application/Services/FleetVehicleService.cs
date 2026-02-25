using AutoMapper;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using ERP.Transport.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Fleet vehicle master — CRUD + driver management + daily status tracking + daily expenses.
/// </summary>
public class FleetVehicleService : IFleetVehicleService
{
    private readonly IRepository<FleetVehicle> _vehicleRepo;
    private readonly IRepository<VehicleDriver> _driverRepo;
    private readonly IRepository<VehicleDailyStatus> _dailyStatusRepo;
    private readonly IRepository<VehicleTravelLog> _travelLogRepo;
    private readonly IRepository<VehicleDailyExpense> _dailyExpenseRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<FleetVehicleService> _logger;

    public FleetVehicleService(
        IRepository<FleetVehicle> vehicleRepo,
        IRepository<VehicleDriver> driverRepo,
        IRepository<VehicleDailyStatus> dailyStatusRepo,
        IRepository<VehicleTravelLog> travelLogRepo,
        IRepository<VehicleDailyExpense> dailyExpenseRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<FleetVehicleService> logger)
    {
        _vehicleRepo = vehicleRepo;
        _driverRepo = driverRepo;
        _dailyStatusRepo = dailyStatusRepo;
        _travelLogRepo = travelLogRepo;
        _dailyExpenseRepo = dailyExpenseRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // ════════════════════════════════════════════════════════════
    //  VEHICLE CRUD
    // ════════════════════════════════════════════════════════════

    public async Task<FleetVehicleDto> CreateVehicleAsync(CreateFleetVehicleDto dto, Guid userId)
    {
        // Duplicate check by registration number
        var existing = await _vehicleRepo.FirstOrDefaultAsync(
            v => v.RegistrationNumber == dto.RegistrationNumber && !v.IsDeleted);
        if (existing != null)
            throw new InvalidOperationException(
                $"Fleet vehicle with registration '{dto.RegistrationNumber}' already exists");

        var entity = _mapper.Map<FleetVehicle>(dto);
        entity.IsActive = true;
        entity.CurrentStatus = FleetVehicleStatus.Available;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _vehicleRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Fleet vehicle {Reg} created by {UserId}", dto.RegistrationNumber, userId);
        return _mapper.Map<FleetVehicleDto>(entity);
    }

    public async Task<FleetVehicleDto?> GetVehicleByIdAsync(Guid id)
    {
        var entity = await _vehicleRepo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<FleetVehicleDto>(entity);
    }

    public async Task<PagedResultDto<FleetVehicleListDto>> GetVehiclesAsync(FleetVehicleFilterDto filter)
    {
        var (items, totalCount) = await _vehicleRepo.GetPagedAsync(
            predicate: v =>
                (filter.Search == null || v.RegistrationNumber.Contains(filter.Search) ||
                    (v.Make != null && v.Make.Contains(filter.Search))) &&
                (filter.VehicleType == null || v.VehicleType == filter.VehicleType) &&
                (filter.Status == null || v.CurrentStatus == filter.Status) &&
                (filter.Ownership == null || v.Ownership == filter.Ownership) &&
                (filter.IsActive == null || v.IsActive == filter.IsActive) &&
                (filter.BranchId == null || v.BranchId == filter.BranchId) &&
                (filter.CountryCode == null || v.CountryCode == filter.CountryCode),
            orderBy: q => q.OrderBy(v => v.RegistrationNumber),
            page: filter.Page,
            pageSize: filter.PageSize);

        return new PagedResultDto<FleetVehicleListDto>
        {
            Items = _mapper.Map<IEnumerable<FleetVehicleListDto>>(items),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<FleetVehicleDto> UpdateVehicleAsync(Guid id, UpdateFleetVehicleDto dto, Guid userId)
    {
        var entity = await _vehicleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Fleet vehicle {id} not found");

        if (dto.VehicleType.HasValue) entity.VehicleType = dto.VehicleType.Value;
        if (dto.Make != null) entity.Make = dto.Make;
        if (dto.Model != null) entity.Model = dto.Model;
        if (dto.ManufactureYear.HasValue) entity.ManufactureYear = dto.ManufactureYear.Value;
        if (dto.ChassisNumber != null) entity.ChassisNumber = dto.ChassisNumber;
        if (dto.EngineNumber != null) entity.EngineNumber = dto.EngineNumber;
        if (dto.Ownership.HasValue) entity.Ownership = dto.Ownership.Value;
        if (dto.LeasingCompany != null) entity.LeasingCompany = dto.LeasingCompany;
        if (dto.LeaseExpiryDate.HasValue) entity.LeaseExpiryDate = dto.LeaseExpiryDate;
        if (dto.InsuranceExpiry.HasValue) entity.InsuranceExpiry = dto.InsuranceExpiry;
        if (dto.InsurancePolicyNumber != null) entity.InsurancePolicyNumber = dto.InsurancePolicyNumber;
        if (dto.FitnessExpiry.HasValue) entity.FitnessExpiry = dto.FitnessExpiry;
        if (dto.PermitExpiry.HasValue) entity.PermitExpiry = dto.PermitExpiry;
        if (dto.PUCExpiry.HasValue) entity.PUCExpiry = dto.PUCExpiry;
        if (dto.FASTagId != null) entity.FASTagId = dto.FASTagId;
        if (dto.LoadCapacityKg.HasValue) entity.LoadCapacityKg = dto.LoadCapacityKg;
        if (dto.VolumeCapacityCBM.HasValue) entity.VolumeCapacityCBM = dto.VolumeCapacityCBM;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
        if (dto.CurrentStatus.HasValue) entity.CurrentStatus = dto.CurrentStatus.Value;
        if (dto.CurrentOdometerKm.HasValue) entity.CurrentOdometerKm = dto.CurrentOdometerKm;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _vehicleRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FleetVehicleDto>(entity);
    }

    public async Task DeleteVehicleAsync(Guid id, Guid userId)
    {
        var entity = await _vehicleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Fleet vehicle {id} not found");

        if (entity.CurrentStatus == FleetVehicleStatus.OnTrip)
            throw new InvalidOperationException("Cannot delete a vehicle that is currently on a trip");

        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;
        _vehicleRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  AVAILABLE VEHICLES
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<FleetVehicleListDto>> GetAvailableVehiclesAsync(
        Guid branchId, string countryCode)
    {
        var vehicles = await _vehicleRepo.FindAsync(v =>
            v.IsActive &&
            v.CurrentStatus == FleetVehicleStatus.Available &&
            v.BranchId == branchId &&
            v.CountryCode == countryCode);

        return _mapper.Map<IEnumerable<FleetVehicleListDto>>(vehicles);
    }

    // ════════════════════════════════════════════════════════════
    //  DRIVER MANAGEMENT
    // ════════════════════════════════════════════════════════════

    public async Task<VehicleDriverDto> AssignDriverAsync(Guid vehicleId, AssignDriverDto dto, Guid userId)
    {
        _ = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Fleet vehicle {vehicleId} not found");

        var driver = _mapper.Map<VehicleDriver>(dto);
        driver.FleetVehicleId = vehicleId;
        driver.IsActive = true;
        driver.AssignedDate = DateTime.UtcNow;
        driver.CreatedBy = userId;
        driver.CreatedDate = DateTime.UtcNow;

        await _driverRepo.AddAsync(driver);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleDriverDto>(driver);
    }

    public async Task<VehicleDriverDto> UpdateDriverAsync(
        Guid vehicleId, Guid driverId, UpdateDriverDto dto, Guid userId)
    {
        var driver = await _driverRepo.GetByIdAsync(driverId)
            ?? throw new KeyNotFoundException($"Driver {driverId} not found");

        if (driver.FleetVehicleId != vehicleId)
            throw new InvalidOperationException("Driver does not belong to this vehicle");

        if (dto.DriverName != null) driver.DriverName = dto.DriverName;
        if (dto.LicenseNumber != null) driver.LicenseNumber = dto.LicenseNumber;
        if (dto.PhoneNumber != null) driver.PhoneNumber = dto.PhoneNumber;
        if (dto.EmergencyContact != null) driver.EmergencyContact = dto.EmergencyContact;
        if (dto.LicenseExpiry.HasValue) driver.LicenseExpiry = dto.LicenseExpiry;
        if (dto.IsActive.HasValue) driver.IsActive = dto.IsActive.Value;

        driver.UpdatedBy = userId;
        driver.UpdatedDate = DateTime.UtcNow;

        _driverRepo.Update(driver);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleDriverDto>(driver);
    }

    public async Task UnassignDriverAsync(Guid vehicleId, Guid driverId, Guid userId)
    {
        var driver = await _driverRepo.GetByIdAsync(driverId)
            ?? throw new KeyNotFoundException($"Driver {driverId} not found");

        if (driver.FleetVehicleId != vehicleId)
            throw new InvalidOperationException("Driver does not belong to this vehicle");

        driver.IsActive = false;
        driver.UnassignedDate = DateTime.UtcNow;
        driver.UpdatedBy = userId;
        driver.UpdatedDate = DateTime.UtcNow;

        _driverRepo.Update(driver);
        await _unitOfWork.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════════════
    //  DAILY STATUS
    // ════════════════════════════════════════════════════════════

    public async Task<VehicleDailyStatusDto> RecordDailyStatusAsync(
        Guid vehicleId, RecordDailyStatusDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Fleet vehicle {vehicleId} not found");

        var status = new VehicleDailyStatus
        {
            FleetVehicleId = vehicleId,
            Date = DateTime.UtcNow.Date,
            Status = dto.Status,
            CurrentJobId = dto.CurrentJobId,
            OdometerKm = dto.OdometerKm,
            Remarks = dto.Remarks,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _dailyStatusRepo.AddAsync(status);

        // Update the vehicle's current status + odometer
        vehicle.CurrentStatus = dto.Status;
        if (dto.OdometerKm.HasValue)
            vehicle.CurrentOdometerKm = dto.OdometerKm;

        vehicle.UpdatedBy = userId;
        vehicle.UpdatedDate = DateTime.UtcNow;
        _vehicleRepo.Update(vehicle);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleDailyStatusDto>(status);
    }

    public async Task<IEnumerable<VehicleDailyStatusDto>> GetDailyStatusHistoryAsync(
        Guid vehicleId, DateTime? from, DateTime? to)
    {
        var statuses = await _dailyStatusRepo.FindAsync(s =>
            s.FleetVehicleId == vehicleId &&
            (from == null || s.Date >= from) &&
            (to == null || s.Date <= to));

        return _mapper.Map<IEnumerable<VehicleDailyStatusDto>>(statuses);
    }

    // ════════════════════════════════════════════════════════════
    //  COMPLIANCE ALERTS
    // ════════════════════════════════════════════════════════════

    public async Task<IEnumerable<FleetVehicleListDto>> GetExpiringComplianceAsync(int daysAhead = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(daysAhead);
        var vehicles = await _vehicleRepo.FindAsync(v =>
            v.IsActive &&
            ((v.InsuranceExpiry.HasValue && v.InsuranceExpiry <= cutoff) ||
             (v.FitnessExpiry.HasValue && v.FitnessExpiry <= cutoff) ||
             (v.PermitExpiry.HasValue && v.PermitExpiry <= cutoff) ||
             (v.PUCExpiry.HasValue && v.PUCExpiry <= cutoff)));

        return _mapper.Map<IEnumerable<FleetVehicleListDto>>(vehicles);
    }

    // ════════════════════════════════════════════════════════════
    //  TRAVEL LOG
    // ════════════════════════════════════════════════════════════

    public async Task<VehicleTravelLogDto> CreateTravelLogAsync(CreateVehicleTravelLogDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(dto.FleetVehicleId)
            ?? throw new KeyNotFoundException($"Fleet vehicle {dto.FleetVehicleId} not found");

        var entity = _mapper.Map<VehicleTravelLog>(dto);
        entity.DistanceKm = dto.EndOdometerKm > dto.StartOdometerKm
            ? dto.EndOdometerKm - dto.StartOdometerKm : 0;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _travelLogRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<VehicleTravelLogDto>(entity);
        result.VehicleRegistration = vehicle.RegistrationNumber;
        return result;
    }

    public async Task<VehicleTravelLogDto?> GetTravelLogByIdAsync(Guid id)
    {
        var entity = await _travelLogRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var dto = _mapper.Map<VehicleTravelLogDto>(entity);
        var vehicle = await _vehicleRepo.GetByIdAsync(entity.FleetVehicleId);
        dto.VehicleRegistration = vehicle?.RegistrationNumber;
        return dto;
    }

    public async Task<PagedResultDto<VehicleTravelLogDto>> GetTravelLogsAsync(TravelLogFilterDto filter)
    {
        var (items, totalCount) = await _travelLogRepo.GetPagedAsync(
            predicate: t =>
                (filter.FleetVehicleId == null || t.FleetVehicleId == filter.FleetVehicleId) &&
                (filter.DriverId == null || t.DriverId == filter.DriverId) &&
                (filter.FromDate == null || t.TripDate >= filter.FromDate) &&
                (filter.ToDate == null || t.TripDate <= filter.ToDate),
            orderBy: q => q.OrderByDescending(t => t.TripDate),
            page: filter.Page,
            pageSize: filter.PageSize);

        return new PagedResultDto<VehicleTravelLogDto>
        {
            Items = _mapper.Map<IEnumerable<VehicleTravelLogDto>>(items),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<VehicleTravelLogDto> CompleteTripAsync(Guid logId, CompleteTripDto dto, Guid userId)
    {
        var entity = await _travelLogRepo.GetByIdAsync(logId)
            ?? throw new KeyNotFoundException($"Travel log {logId} not found");

        entity.EndOdometerKm = dto.EndOdometerKm;
        entity.DistanceKm = dto.EndOdometerKm > entity.StartOdometerKm
            ? dto.EndOdometerKm - entity.StartOdometerKm : 0;
        entity.ArrivalTime = dto.ArrivalTime;
        entity.FuelConsumedLitres = dto.FuelConsumedLitres;
        entity.FuelCost = dto.FuelCost;
        entity.FuelReceiptUrl = dto.FuelReceiptUrl;
        entity.TollCharges = dto.TollCharges;
        entity.ParkingCharges = dto.ParkingCharges;
        entity.OtherExpenses = dto.OtherExpenses;
        entity.ExpenseNotes = dto.ExpenseNotes;
        entity.Remarks = dto.Remarks;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _travelLogRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleTravelLogDto>(entity);
    }

    public async Task<VehicleUsageSummaryDto> GetVehicleUsageSummaryAsync(Guid vehicleId, DateTime from, DateTime to)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new KeyNotFoundException($"Fleet vehicle {vehicleId} not found");

        var logs = (await _travelLogRepo.FindAsync(t =>
            t.FleetVehicleId == vehicleId &&
            t.TripDate >= from && t.TripDate <= to)).ToList();

        var totalDistance = logs.Sum(t => t.DistanceKm);
        var totalFuel = logs.Sum(t => t.FuelConsumedLitres ?? 0);

        return new VehicleUsageSummaryDto
        {
            FleetVehicleId = vehicleId,
            VehicleRegistration = vehicle.RegistrationNumber,
            TotalTrips = logs.Count,
            TotalDistanceKm = totalDistance,
            TotalFuelLitres = totalFuel,
            TotalFuelCost = logs.Sum(t => t.FuelCost ?? 0),
            TotalTollCharges = logs.Sum(t => t.TollCharges ?? 0),
            TotalExpenses = logs.Sum(t => 
                (t.FuelCost ?? 0) + (t.TollCharges ?? 0) + (t.ParkingCharges ?? 0) + (t.OtherExpenses ?? 0)),
            AverageKmPerTrip = logs.Count > 0 ? totalDistance / logs.Count : 0,
            FuelEfficiencyKmPerLitre = totalFuel > 0 ? totalDistance / totalFuel : null
        };
    }

    // ════════════════════════════════════════════════════════════
    //  Daily Expense Management (Legacy: TransDailyExpense.aspx)
    // ════════════════════════════════════════════════════════════

    public async Task<VehicleDailyExpenseDto> RecordDailyExpenseAsync(Guid vehicleId, CreateVehicleDailyExpenseDto dto, Guid userId)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new TransportNotFoundException("FleetVehicle", vehicleId);

        // Business rule: 30-day rolling window
        var cutoffDate = DateTime.UtcNow.Date.AddDays(-30);
        if (dto.ExpenseDate.Date < cutoffDate)
            throw new TransportBusinessException($"Cannot record expenses older than 30 days. Cutoff: {cutoffDate:dd-MMM-yyyy}");

        if (dto.ExpenseDate.Date > DateTime.UtcNow.Date)
            throw new TransportBusinessException("Cannot record expenses for future dates");

        // Check for duplicate on same day
        var existing = await _dailyExpenseRepo.FirstOrDefaultAsync(e =>
            e.FleetVehicleId == vehicleId &&
            e.ExpenseDate.Date == dto.ExpenseDate.Date &&
            !e.IsDeleted);

        if (existing != null)
            throw new TransportBusinessException($"Daily expense already recorded for vehicle {vehicle.RegistrationNumber} on {dto.ExpenseDate:dd-MMM-yyyy}. Use update instead.");

        var totalAmount = dto.Fuel + dto.Fuel2 + dto.TollCharges + dto.Fines + dto.Xerox +
            dto.VaraiUnloading + dto.EmptyContainer + dto.Parking + dto.Garage +
            dto.Bhatta + dto.ODCOverweight + dto.OtherCharges + dto.DamageContainer;

        var entity = new VehicleDailyExpense
        {
            Id = Guid.NewGuid(),
            FleetVehicleId = vehicleId,
            ExpenseDate = dto.ExpenseDate.Date,
            Fuel = dto.Fuel,
            FuelLitres = dto.FuelLitres,
            Fuel2 = dto.Fuel2,
            Fuel2Litres = dto.Fuel2Litres,
            TollCharges = dto.TollCharges,
            Fines = dto.Fines,
            Xerox = dto.Xerox,
            VaraiUnloading = dto.VaraiUnloading,
            EmptyContainer = dto.EmptyContainer,
            Parking = dto.Parking,
            Garage = dto.Garage,
            Bhatta = dto.Bhatta,
            ODCOverweight = dto.ODCOverweight,
            OtherCharges = dto.OtherCharges,
            DamageContainer = dto.DamageContainer,
            TotalAmount = totalAmount,
            Remarks = dto.Remarks,
            TransportRequestId = dto.TransportRequestId,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _dailyExpenseRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Daily expense recorded for vehicle {Reg} on {Date}: {Total}",
            vehicle.RegistrationNumber, dto.ExpenseDate.Date, totalAmount);

        return MapDailyExpenseDto(entity, vehicle.RegistrationNumber);
    }

    public async Task<VehicleDailyExpenseDto> UpdateDailyExpenseAsync(Guid vehicleId, Guid expenseId, UpdateVehicleDailyExpenseDto dto, Guid userId)
    {
        var entity = await _dailyExpenseRepo.FirstOrDefaultAsync(e =>
            e.Id == expenseId && e.FleetVehicleId == vehicleId && !e.IsDeleted)
            ?? throw new TransportNotFoundException("VehicleDailyExpense", expenseId);

        if (dto.Fuel.HasValue) entity.Fuel = dto.Fuel.Value;
        if (dto.FuelLitres.HasValue) entity.FuelLitres = dto.FuelLitres.Value;
        if (dto.Fuel2.HasValue) entity.Fuel2 = dto.Fuel2.Value;
        if (dto.Fuel2Litres.HasValue) entity.Fuel2Litres = dto.Fuel2Litres.Value;
        if (dto.TollCharges.HasValue) entity.TollCharges = dto.TollCharges.Value;
        if (dto.Fines.HasValue) entity.Fines = dto.Fines.Value;
        if (dto.Xerox.HasValue) entity.Xerox = dto.Xerox.Value;
        if (dto.VaraiUnloading.HasValue) entity.VaraiUnloading = dto.VaraiUnloading.Value;
        if (dto.EmptyContainer.HasValue) entity.EmptyContainer = dto.EmptyContainer.Value;
        if (dto.Parking.HasValue) entity.Parking = dto.Parking.Value;
        if (dto.Garage.HasValue) entity.Garage = dto.Garage.Value;
        if (dto.Bhatta.HasValue) entity.Bhatta = dto.Bhatta.Value;
        if (dto.ODCOverweight.HasValue) entity.ODCOverweight = dto.ODCOverweight.Value;
        if (dto.OtherCharges.HasValue) entity.OtherCharges = dto.OtherCharges.Value;
        if (dto.DamageContainer.HasValue) entity.DamageContainer = dto.DamageContainer.Value;
        if (dto.Remarks != null) entity.Remarks = dto.Remarks;

        entity.TotalAmount = entity.Fuel + entity.Fuel2 + entity.TollCharges + entity.Fines + entity.Xerox +
            entity.VaraiUnloading + entity.EmptyContainer + entity.Parking + entity.Garage +
            entity.Bhatta + entity.ODCOverweight + entity.OtherCharges + entity.DamageContainer;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _dailyExpenseRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
        return MapDailyExpenseDto(entity, vehicle?.RegistrationNumber);
    }

    public async Task DeleteDailyExpenseAsync(Guid vehicleId, Guid expenseId, Guid userId)
    {
        var entity = await _dailyExpenseRepo.FirstOrDefaultAsync(e =>
            e.Id == expenseId && e.FleetVehicleId == vehicleId && !e.IsDeleted)
            ?? throw new TransportNotFoundException("VehicleDailyExpense", expenseId);

        entity.IsDeleted = true;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;
        _dailyExpenseRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResultDto<VehicleDailyExpenseDto>> GetDailyExpensesAsync(DailyExpenseFilterDto filter)
    {
        var all = await _dailyExpenseRepo.FindAsync(e => !e.IsDeleted);
        var items = all.AsQueryable();

        if (filter.FleetVehicleId.HasValue)
            items = items.Where(e => e.FleetVehicleId == filter.FleetVehicleId);
        if (filter.FromDate.HasValue)
            items = items.Where(e => e.ExpenseDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            items = items.Where(e => e.ExpenseDate <= filter.ToDate.Value);
        if (filter.TransportRequestId.HasValue)
            items = items.Where(e => e.TransportRequestId == filter.TransportRequestId);

        var total = items.Count();
        var data = items
            .OrderByDescending(e => e.ExpenseDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(e => MapDailyExpenseDto(e, null))
            .ToList();

        return new PagedResultDto<VehicleDailyExpenseDto>
        {
            Items = data,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<DailyExpenseAggregateDto> GetDailyExpenseAggregateAsync(Guid vehicleId, DateTime from, DateTime to)
    {
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId)
            ?? throw new TransportNotFoundException("FleetVehicle", vehicleId);

        var expenses = await _dailyExpenseRepo.FindAsync(e =>
            e.FleetVehicleId == vehicleId &&
            e.ExpenseDate >= from.Date &&
            e.ExpenseDate <= to.Date &&
            !e.IsDeleted);

        var list = expenses.ToList();

        return new DailyExpenseAggregateDto
        {
            FleetVehicleId = vehicleId,
            RegistrationNumber = vehicle.RegistrationNumber,
            FromDate = from,
            ToDate = to,
            DayCount = list.Count,
            TotalFuel = list.Sum(e => e.Fuel + e.Fuel2),
            TotalToll = list.Sum(e => e.TollCharges),
            TotalFines = list.Sum(e => e.Fines),
            TotalParking = list.Sum(e => e.Parking),
            TotalGarage = list.Sum(e => e.Garage),
            TotalOther = list.Sum(e => e.Xerox + e.VaraiUnloading + e.EmptyContainer +
                e.Bhatta + e.ODCOverweight + e.OtherCharges + e.DamageContainer),
            GrandTotal = list.Sum(e => e.TotalAmount)
        };
    }

    private static VehicleDailyExpenseDto MapDailyExpenseDto(VehicleDailyExpense e, string? regNumber) => new()
    {
        Id = e.Id,
        FleetVehicleId = e.FleetVehicleId,
        RegistrationNumber = regNumber,
        ExpenseDate = e.ExpenseDate,
        Fuel = e.Fuel,
        FuelLitres = e.FuelLitres,
        Fuel2 = e.Fuel2,
        Fuel2Litres = e.Fuel2Litres,
        TollCharges = e.TollCharges,
        Fines = e.Fines,
        Xerox = e.Xerox,
        VaraiUnloading = e.VaraiUnloading,
        EmptyContainer = e.EmptyContainer,
        Parking = e.Parking,
        Garage = e.Garage,
        Bhatta = e.Bhatta,
        ODCOverweight = e.ODCOverweight,
        OtherCharges = e.OtherCharges,
        DamageContainer = e.DamageContainer,
        TotalAmount = e.TotalAmount,
        CurrencyCode = e.CurrencyCode,
        Remarks = e.Remarks,
        TransportRequestId = e.TransportRequestId,
        CreatedDate = e.CreatedDate
    };
}
