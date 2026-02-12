using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Transporter (trucking vendor) master record.
/// Transport-specific — not in Masters MS.
/// </summary>
public class Transporter : BaseEntity
{
    public string TransporterName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PANNumber { get; set; }
    public string? GSTNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }
    public string? CountryCode { get; set; }

    public TransporterStatus Status { get; set; }
    public string? SuspensionReason { get; set; }
    public DateTime? SuspensionDate { get; set; }

    public decimal Rating { get; set; }

    /// <summary>Configurable own-fleet flag (replaces legacy NavBharat/NavJeevan hardcoded IDs)</summary>
    public bool IsOwnFleet { get; set; }

    /// <summary>Branch this transporter is registered under</summary>
    public Guid? BranchId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public ICollection<TransporterKYC> KYCDocuments { get; set; } = new List<TransporterKYC>();
    public ICollection<TransporterBank> BankAccounts { get; set; } = new List<TransporterBank>();
    public ICollection<TransporterNotification> NotificationSettings { get; set; } = new List<TransporterNotification>();
    public ICollection<TransportVehicle> VehicleAssignments { get; set; } = new List<TransportVehicle>();
}
