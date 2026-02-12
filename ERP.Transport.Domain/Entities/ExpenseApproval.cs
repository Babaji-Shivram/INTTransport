using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Expense approval record — HOD / manager approval for transport expenses.
/// Legacy: ApproveExpense.aspx → SP: insAdditionalExpenseApprovalHOD
/// </summary>
public class ExpenseApproval : BaseEntity
{
    public Guid TransportExpenseId { get; set; }

    // ── Approval Details ────────────────────────────────────────
    public ExpenseApprovalStatus Status { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? Remarks { get; set; }
    public string? RejectionReason { get; set; }

    // ── Approver ────────────────────────────────────────────────
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedDate { get; set; }

    // ── Approval Level (for multi-level workflows) ──────────────
    public int ApprovalLevel { get; set; } = 1;
    public string? ApprovalRole { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportExpense TransportExpense { get; set; } = null!;
}
