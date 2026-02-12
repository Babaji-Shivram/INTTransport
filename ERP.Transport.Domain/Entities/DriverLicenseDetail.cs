namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Cached SARATHI driver license verification result.
/// </summary>
public class DriverLicenseDetail : BaseEntity
{
    // ── Lookup Keys ─────────────────────────────────────────────
    public string LicenseNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    // ── SARATHI Response Fields ─────────────────────────────────
    public string? HolderName { get; set; }
    public string? FatherOrHusbandName { get; set; }
    public string? Address { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? BloodGroup { get; set; }
    public string? Gender { get; set; }
    public string? PhotoUrl { get; set; }

    // ── License Details ─────────────────────────────────────────
    public DateTime? IssueDate { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? IssuingAuthority { get; set; }

    // ── Vehicle Classes Authorized ──────────────────────────────
    /// <summary>Comma-separated: LMV,HMV,HPMV,TRANS etc.</summary>
    public string? VehicleClassesAuthorized { get; set; }
    public bool? HasHazardousGoodsEndorsement { get; set; }
    public bool? HasHillDrivingEndorsement { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public string? LicenseStatus { get; set; }
    public bool? IsSuspended { get; set; }
    public bool? IsRevoked { get; set; }

    // ── Cache Control ───────────────────────────────────────────
    public DateTime LastFetchedFromUlip { get; set; }
    public string? RawApiResponse { get; set; }
}
