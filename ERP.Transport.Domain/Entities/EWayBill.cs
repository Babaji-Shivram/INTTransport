namespace ERP.Transport.Domain.Entities;

/// <summary>
/// E-Way Bill — generated or fetched from NIC/ULIP for India GST compliance.
/// Linked to a transport request.
/// </summary>
public class EWayBill : BaseEntity
{
    // ── Link to Transport Job ───────────────────────────────────
    public Guid TransportRequestId { get; set; }
    public TransportRequest TransportRequest { get; set; } = null!;

    // ── E-Way Bill Identity ─────────────────────────────────────
    public string? EWayBillNumber { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public DateTime? ValidUpto { get; set; }
    public string? EWayBillStatus { get; set; }

    // ── Supplier / Recipient ────────────────────────────────────
    public string? SupplierGstin { get; set; }
    public string? SupplierName { get; set; }
    public string? RecipientGstin { get; set; }
    public string? RecipientName { get; set; }

    // ── Document ────────────────────────────────────────────────
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? DocumentDate { get; set; }

    // ── Goods ───────────────────────────────────────────────────
    public string? HsnCode { get; set; }
    public string? ProductDescription { get; set; }
    public decimal? TaxableAmount { get; set; }
    public decimal? CgstAmount { get; set; }
    public decimal? SgstAmount { get; set; }
    public decimal? IgstAmount { get; set; }
    public decimal? TotalInvoiceValue { get; set; }

    // ── Transport ───────────────────────────────────────────────
    public string? TransporterGstin { get; set; }
    public string? TransporterName { get; set; }
    public string? TransportMode { get; set; }
    public string? VehicleNumber { get; set; }
    public string? VehicleType { get; set; }
    public decimal? ApproximateDistanceKm { get; set; }

    // ── From / To ───────────────────────────────────────────────
    public string? FromState { get; set; }
    public string? FromPlace { get; set; }
    public string? FromPincode { get; set; }
    public string? ToState { get; set; }
    public string? ToPlace { get; set; }
    public string? ToPincode { get; set; }

    // ── Cache ───────────────────────────────────────────────────
    public DateTime? LastFetchedFromApi { get; set; }
    public string? RawApiResponse { get; set; }
}
