using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Expense Approval DTOs (Legacy: ApproveExpense.aspx)
// ═══════════════════════════════════════════════════════════════

public class ExpenseApprovalDto
{
    public Guid Id { get; set; }
    public Guid TransportExpenseId { get; set; }
    public ExpenseApprovalStatus Status { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? Remarks { get; set; }
    public string? RejectionReason { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public int ApprovalLevel { get; set; }
    public string? ApprovalRole { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class SubmitExpenseForApprovalDto
{
    public string? Remarks { get; set; }
}

public class ApproveExpenseDto
{
    public decimal ApprovedAmount { get; set; }
    public string? Remarks { get; set; }
}

public class RejectExpenseDto
{
    public string RejectionReason { get; set; } = null!;
}

public class PendingExpenseApprovalDto
{
    public Guid ExpenseId { get; set; }
    public Guid JobId { get; set; }
    public string? RequestNumber { get; set; }
    public string? Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Remarks { get; set; }
    public ExpenseApprovalStatus ApprovalStatus { get; set; }
    public int PendingLevel { get; set; }
}
