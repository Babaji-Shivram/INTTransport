namespace ERP.Transport.Domain.Entities;

/// <summary>
/// E-Invoice (IRN) record — generated via CharteredInfo / NIC e-Invoice API.
/// One per transport invoice/document.
/// </summary>
public class EInvoice : BaseEntity
{
    // ── Link to Transport Job ───────────────────────────────────
    public Guid? TransportRequestId { get; set; }
    public TransportRequest? TransportRequest { get; set; }

    // ── IRN Identity ────────────────────────────────────────────
    public string? Irn { get; set; }
    public string? AckNumber { get; set; }
    public DateTime? AckDate { get; set; }
    public string? SignedInvoice { get; set; }
    public string? SignedQrCode { get; set; }
    public string? QrCodeImageBase64 { get; set; }
    public string? EInvoiceStatus { get; set; } // GENERATED, CANCELLED, FAILED

    // ── Document Details ────────────────────────────────────────
    public string? DocumentType { get; set; } // INV, CRN, DBN
    public string? DocumentNumber { get; set; }
    public DateTime? DocumentDate { get; set; }

    // ── Seller / Buyer ──────────────────────────────────────────
    public string? SellerGstin { get; set; }
    public string? SellerLegalName { get; set; }
    public string? SellerTradeName { get; set; }
    public string? BuyerGstin { get; set; }
    public string? BuyerLegalName { get; set; }
    public string? BuyerTradeName { get; set; }

    // ── Values ──────────────────────────────────────────────────
    public decimal? TotalAssessableValue { get; set; }
    public decimal? TotalCgstValue { get; set; }
    public decimal? TotalSgstValue { get; set; }
    public decimal? TotalIgstValue { get; set; }
    public decimal? TotalCessValue { get; set; }
    public decimal? TotalInvoiceValue { get; set; }

    // ── E-Way Bill (if generated with IRN) ──────────────────────
    public long? EwbNumber { get; set; }
    public DateTime? EwbDate { get; set; }
    public DateTime? EwbValidTill { get; set; }

    // ── Cancellation ────────────────────────────────────────────
    public string? CancelReason { get; set; }
    public string? CancelRemarks { get; set; }
    public DateTime? CancelledDate { get; set; }

    // ── Raw ─────────────────────────────────────────────────────
    public string? RawRequest { get; set; }
    public string? RawResponse { get; set; }
}
