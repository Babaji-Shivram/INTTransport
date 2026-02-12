using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Advance payment request from transporter before starting the trip.
/// </summary>
public class VehicleFundRequest : BaseEntity
{
    public Guid TransportVehicleId { get; set; }

    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";

    // ── Bank Details ────────────────────────────────────────────
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public FundRequestStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid? ProcessedBy { get; set; }
    public DateTime? ProcessedDate { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportVehicle TransportVehicle { get; set; } = null!;
}
