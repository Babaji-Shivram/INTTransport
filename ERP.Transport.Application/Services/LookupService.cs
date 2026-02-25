using AutoMapper;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// CRUD service for transport lookup / master tables.
/// </summary>
public class LookupService : ILookupService
{
    private readonly IRepository<TransportLookup> _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LookupService> _logger;

    public LookupService(
        IRepository<TransportLookup> repo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LookupService> logger)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TransportLookupDto> CreateAsync(CreateTransportLookupDto dto, Guid userId)
    {
        // Check for duplicate code within category
        var existing = await _repo.FirstOrDefaultAsync(l =>
            l.Category == dto.Category && l.Code == dto.Code);

        if (existing != null)
            throw new InvalidOperationException(
                $"Lookup with code '{dto.Code}' already exists in category {dto.Category}");

        var entity = _mapper.Map<TransportLookup>(dto);
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Lookup {Category}/{Code} created", dto.Category, dto.Code);
        return _mapper.Map<TransportLookupDto>(entity);
    }

    public async Task<TransportLookupDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<TransportLookupDto>(entity);
    }

    public async Task<IEnumerable<TransportLookupDto>> GetByCategoryAsync(
        LookupCategory category, string? countryCode = null, bool activeOnly = true)
    {
        var items = await _repo.FindAsync(l =>
            l.Category == category &&
            (!activeOnly || l.IsActive) &&
            (countryCode == null || l.CountryCode == null || l.CountryCode == countryCode));

        var sorted = items.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Name);
        return _mapper.Map<IEnumerable<TransportLookupDto>>(sorted);
    }

    public async Task<IEnumerable<TransportLookupDto>> SearchAsync(LookupFilterDto filter)
    {
        var items = await _repo.FindAsync(l =>
            (filter.Category == null || l.Category == filter.Category) &&
            (filter.IsActive == null || l.IsActive == filter.IsActive) &&
            (filter.CountryCode == null || l.CountryCode == null || l.CountryCode == filter.CountryCode) &&
            (filter.SearchTerm == null ||
             l.Name.Contains(filter.SearchTerm) ||
             l.Code.Contains(filter.SearchTerm)));

        var sorted = items.OrderBy(l => l.Category).ThenBy(l => l.DisplayOrder).ThenBy(l => l.Name);
        return _mapper.Map<IEnumerable<TransportLookupDto>>(sorted);
    }

    public async Task<TransportLookupDto> UpdateAsync(Guid id, UpdateTransportLookupDto dto, Guid userId)
    {
        var entity = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Lookup {id} not found");

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.DisplayOrder.HasValue) entity.DisplayOrder = dto.DisplayOrder.Value;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _repo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransportLookupDto>(entity);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var entity = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Lookup {id} not found");

        entity.UpdatedBy = userId;
        _repo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Lookup {Category}/{Code} deleted", entity.Category, entity.Code);
    }

    // ════════════════════════════════════════════════════════════
    //  SEED DEFAULTS (idempotent — skips existing codes)
    // ════════════════════════════════════════════════════════════

    public async Task SeedDefaultsAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var seeds = GetDefaultLookups();
        int added = 0;

        foreach (var seed in seeds)
        {
            var exists = await _repo.AnyAsync(l =>
                l.Category == seed.Category && l.Code == seed.Code);

            if (!exists)
            {
                seed.CreatedBy = userId;
                seed.CreatedDate = now;
                await _repo.AddAsync(seed);
                added++;
            }
        }

        if (added > 0)
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} default lookup entries", added);
        }
    }

    private static List<TransportLookup> GetDefaultLookups()
    {
        var lookups = new List<TransportLookup>();
        int order;

        // ── Vehicle Types (mirrors VehicleTypeEnum) ─────────
        order = 0;
        foreach (var (code, name) in new[]
        {
            ("TRAILER_40FT", "40ft Trailer"),
            ("TRUCK_20FT", "20ft Truck"),
            ("TEMPO", "Tempo"),
            ("LCV", "Light Commercial Vehicle"),
            ("TRAILER_20FT", "20ft Trailer"),
            ("OPEN_TRUCK", "Open Truck"),
            ("CLOSED_CONTAINER", "Closed Container"),
            ("TANKER", "Tanker"),
            ("CAR", "Car")
        })
        {
            lookups.Add(new TransportLookup
            {
                Category = LookupCategory.VehicleType,
                Code = code,
                Name = name,
                DisplayOrder = ++order,
                IsActive = true
            });
        }

        // ── Expense Categories (mirrors ExpenseCategory enum) ─
        order = 0;
        foreach (var (code, name) in new[]
        {
            ("FUEL_TYPE1", "Fuel Type 1 (Diesel)"),
            ("FUEL_TYPE2", "Fuel Type 2 (Petrol/CNG)"),
            ("FUEL_LITRES", "Fuel Litres"),
            ("TOLL_CHARGES", "Toll Charges"),
            ("FINES", "Fines & Penalties"),
            ("DOCUMENTATION", "Documentation"),
            ("VARAI_UNLOADING", "Varai / Unloading"),
            ("EMPTY_CONTAINER", "Empty Container Return"),
            ("PARKING", "Parking"),
            ("GARAGE", "Garage / Repair"),
            ("DRIVER_ALLOWANCE", "Driver Allowance"),
            ("ODC_OVERWEIGHT", "ODC / Overweight"),
            ("DAMAGE_CONTAINER", "Damage Container"),
            ("TEMPO_UNION", "Tempo Union"),
            ("OTHER", "Other")
        })
        {
            lookups.Add(new TransportLookup
            {
                Category = LookupCategory.ExpenseCategory,
                Code = code,
                Name = name,
                DisplayOrder = ++order,
                IsActive = true
            });
        }

        // ── Vehicle Statuses (mirrors FleetVehicleStatus enum) ─
        order = 0;
        foreach (var (code, name) in new[]
        {
            ("AVAILABLE", "Available"),
            ("ON_TRIP", "On Trip"),
            ("UNDER_MAINTENANCE", "Under Maintenance"),
            ("OUT_OF_SERVICE", "Out of Service"),
            ("RESERVED", "Reserved")
        })
        {
            lookups.Add(new TransportLookup
            {
                Category = LookupCategory.VehicleStatus,
                Code = code,
                Name = name,
                DisplayOrder = ++order,
                IsActive = true
            });
        }

        // ── Maintenance Categories (mirrors MaintenanceType enum) ─
        order = 0;
        foreach (var (code, name) in new[]
        {
            ("PREVENTIVE", "Preventive Maintenance"),
            ("CORRECTIVE", "Corrective / Breakdown"),
            ("TYRE", "Tyre Replacement / Rotation"),
            ("ENGINE", "Engine Service"),
            ("BRAKE", "Brake Service"),
            ("ELECTRICAL", "Electrical System"),
            ("BODY_REPAIR", "Body Repair / Paint"),
            ("AC_HVAC", "AC / HVAC"),
            ("TRANSMISSION", "Transmission"),
            ("SUSPENSION", "Suspension"),
            ("BATTERY", "Battery Replacement"),
            ("INSPECTION", "Mandatory Inspection / PUC"),
            ("OTHER", "Other")
        })
        {
            lookups.Add(new TransportLookup
            {
                Category = LookupCategory.MaintenanceCategory,
                Code = code,
                Name = name,
                DisplayOrder = ++order,
                IsActive = true
            });
        }

        return lookups;
    }
}
