namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Cached VAHAN vehicle lookup — avoids repeated ULIP API calls.
/// Refreshed on demand or when stale (>24h).
/// </summary>
public class VehicleDetail : BaseEntity
{
    // ── Lookup Key ──────────────────────────────────────────────
    public string VehicleNumber { get; set; } = null!;

    // ── VAHAN Response Fields ───────────────────────────────────
    public string? OwnerName { get; set; }
    public string? FatherName { get; set; }
    public string? PresentAddress { get; set; }
    public string? PermanentAddress { get; set; }

    public string? VehicleClass { get; set; }
    public string? VehicleCategory { get; set; }
    public string? MakerModel { get; set; }
    public string? MakerDescription { get; set; }
    public string? BodyType { get; set; }
    public string? FuelType { get; set; }
    public string? Color { get; set; }
    public int? ManufacturingYear { get; set; }
    public int? SeatingCapacity { get; set; }
    public decimal? GrossVehicleWeight { get; set; }
    public decimal? UnladenWeight { get; set; }
    public int? NumberOfCylinders { get; set; }
    public string? EngineNumber { get; set; }
    public string? ChassisNumber { get; set; }

    // ── Registration ────────────────────────────────────────────
    public string? RegisteringAuthority { get; set; }
    public string? RegistrationState { get; set; }
    public DateTime? RegistrationDate { get; set; }

    // ── Validity / Insurance ────────────────────────────────────
    public DateTime? FitnessUpto { get; set; }
    public DateTime? InsuranceUpto { get; set; }
    public string? InsuranceCompany { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? PucValidUpto { get; set; }
    public DateTime? TaxValidUpto { get; set; }
    public DateTime? PermitValidUpto { get; set; }
    public string? PermitType { get; set; }
    public string? NationalPermitNumber { get; set; }
    public DateTime? NationalPermitUpto { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public string? VehicleStatus { get; set; }
    public bool? IsBlacklisted { get; set; }
    public bool? IsFinanced { get; set; }
    public string? FinancerName { get; set; }

    // ── Cache Control ───────────────────────────────────────────
    public DateTime LastFetchedFromUlip { get; set; }
    public string? RawApiResponse { get; set; }
}
