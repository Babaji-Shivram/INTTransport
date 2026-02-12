using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Trip expense entry per vehicle — tracks actual costs incurred during a trip.
/// 15 expense categories as per PRD.
/// </summary>
public class TransportExpense : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    /// <summary>Optional link to specific vehicle</summary>
    public Guid? TransportVehicleId { get; set; }

    public ExpenseCategory Category { get; set; }
    public string? CategoryDescription { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Remarks { get; set; }
    public string? ReceiptUrl { get; set; }

    // ── Approval (Legacy: ApproveExpense.aspx) ───────────────────
    public ExpenseApprovalStatus ApprovalStatus { get; set; }
    public bool RequiresApproval { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
    public ICollection<ExpenseApproval> Approvals { get; set; } = new List<ExpenseApproval>();
}
