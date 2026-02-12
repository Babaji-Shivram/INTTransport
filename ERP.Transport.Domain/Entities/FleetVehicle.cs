using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Company-owned or leased fleet vehicle master record.
/// Separate from TransportVehicle (which is a per-job hire assignment).
/// </summary>
public class FleetVehicle : BaseEntity
{
    // ── Registration ────────────────────────────────────────────
    public string RegistrationNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? ManufactureYear { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }

    // ── Ownership ───────────────────────────────────────────────
    public OwnershipType Ownership { get; set; }
    public string? LeasingCompany { get; set; }
    public DateTime? LeaseExpiryDate { get; set; }

    // ── Compliance ──────────────────────────────────────────────
    public DateTime? InsuranceExpiry { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? FitnessExpiry { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public DateTime? PUCExpiry { get; set; }
    public string? FASTagId { get; set; }

    // ── Capacity ────────────────────────────────────────────────
    public decimal? LoadCapacityKg { get; set; }
    public decimal? VolumeCapacityCBM { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public bool IsActive { get; set; } = true;
    public FleetVehicleStatus CurrentStatus { get; set; }
    public decimal? CurrentOdometerKm { get; set; }

    // ── Scoping ─────────────────────────────────────────────────
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;

    // ── Navigation ──────────────────────────────────────────────
    public ICollection<VehicleDriver> Drivers { get; set; } = new List<VehicleDriver>();
    public ICollection<VehicleDailyStatus> DailyStatuses { get; set; } = new List<VehicleDailyStatus>();
    public ICollection<VehicleDailyExpense> DailyExpenses { get; set; } = new List<VehicleDailyExpense>();
}
