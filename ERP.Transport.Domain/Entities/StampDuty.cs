namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Stamp duty tracking — regulatory compliance for transport documents.
/// Legacy: BS_GetStampDutyDetailById, insStampDutyDetail, updStampDutyAmnt
/// </summary>
public class StampDuty : BaseEntity
{
    public string ReferenceNumber { get; set; } = null!;
    public Guid? TransportRequestId { get; set; }
    public Guid? TransporterId { get; set; }

    // ── Stamp Duty Details ──────────────────────────────────────
    public string? DocumentType { get; set; }
    public decimal StampDutyAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime DutyDate { get; set; }
    public string? StateCode { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? ReceiptDocumentUrl { get; set; }
    public string? Remarks { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PaidByName { get; set; }

    // ── Org Scoping ─────────────────────────────────────────────
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = "IN";
    public string CurrencyCode { get; set; } = "INR";

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest? TransportRequest { get; set; }
    public Transporter? Transporter { get; set; }
}
