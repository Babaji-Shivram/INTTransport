using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Fleet;

// ═══════════════════════════════════════════════════════════════
//  Fleet Vehicle Master DTOs
// ═══════════════════════════════════════════════════════════════

// ── Fleet Vehicle ───────────────────────────────────────────────

public class FleetVehicleDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? ManufactureYear { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public OwnershipType Ownership { get; set; }
    public string? LeasingCompany { get; set; }
    public DateTime? LeaseExpiryDate { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? FitnessExpiry { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public DateTime? PUCExpiry { get; set; }
    public string? FASTagId { get; set; }
    public decimal? LoadCapacityKg { get; set; }
    public decimal? VolumeCapacityCBM { get; set; }
    public bool IsActive { get; set; }
    public FleetVehicleStatus CurrentStatus { get; set; }
    public decimal? CurrentOdometerKm { get; set; }
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;
    public DateTime CreatedDate { get; set; }

    // ── Computed ─────────────────────────────────────────────────
    public bool IsInsuranceExpired { get; set; }
    public bool IsFitnessExpired { get; set; }
    public bool IsPermitExpired { get; set; }

    // ── Child collections ───────────────────────────────────────
    public ICollection<VehicleDriverDto> Drivers { get; set; } = new List<VehicleDriverDto>();
}

public class FleetVehicleListDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public OwnershipType Ownership { get; set; }
    public bool IsActive { get; set; }
    public FleetVehicleStatus CurrentStatus { get; set; }
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = null!;
}

public class CreateFleetVehicleDto
{
    public string RegistrationNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? ManufactureYear { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public OwnershipType Ownership { get; set; }
    public string? LeasingCompany { get; set; }
    public DateTime? LeaseExpiryDate { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? FitnessExpiry { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public DateTime? PUCExpiry { get; set; }
    public string? FASTagId { get; set; }
    public decimal? LoadCapacityKg { get; set; }
    public decimal? VolumeCapacityCBM { get; set; }
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;
}

public class UpdateFleetVehicleDto
{
    public VehicleTypeEnum? VehicleType { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? ManufactureYear { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
    public OwnershipType? Ownership { get; set; }
    public string? LeasingCompany { get; set; }
    public DateTime? LeaseExpiryDate { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? FitnessExpiry { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public DateTime? PUCExpiry { get; set; }
    public string? FASTagId { get; set; }
    public decimal? LoadCapacityKg { get; set; }
    public decimal? VolumeCapacityCBM { get; set; }
    public bool? IsActive { get; set; }
    public FleetVehicleStatus? CurrentStatus { get; set; }
    public decimal? CurrentOdometerKm { get; set; }
}

public class FleetVehicleFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public VehicleTypeEnum? VehicleType { get; set; }
    public FleetVehicleStatus? Status { get; set; }
    public OwnershipType? Ownership { get; set; }
    public bool? IsActive { get; set; }
    public Guid? BranchId { get; set; }
    public string? CountryCode { get; set; }
}

// ── Vehicle Driver ──────────────────────────────────────────────

public class VehicleDriverDto
{
    public Guid Id { get; set; }
    public Guid FleetVehicleId { get; set; }
    public string DriverName { get; set; } = null!;
    public string? LicenseNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? UnassignedDate { get; set; }
}

public class AssignDriverDto
{
    public string DriverName { get; set; } = null!;
    public string? LicenseNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime? LicenseExpiry { get; set; }
}

public class UpdateDriverDto
{
    public string? DriverName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool? IsActive { get; set; }
}

// ── Vehicle Daily Status ────────────────────────────────────────

public class VehicleDailyStatusDto
{
    public Guid Id { get; set; }
    public Guid FleetVehicleId { get; set; }
    public DateTime Date { get; set; }
    public FleetVehicleStatus Status { get; set; }
    public Guid? CurrentJobId { get; set; }
    public decimal? OdometerKm { get; set; }
    public string? Remarks { get; set; }
}

public class RecordDailyStatusDto
{
    public FleetVehicleStatus Status { get; set; }
    public Guid? CurrentJobId { get; set; }
    public decimal? OdometerKm { get; set; }
    public string? Remarks { get; set; }
}
