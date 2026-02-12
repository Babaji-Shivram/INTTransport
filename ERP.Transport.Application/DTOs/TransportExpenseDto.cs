using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Transport Expense DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>Response DTO for transport expense records.</summary>
public class TransportExpenseDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public Guid? TransportVehicleId { get; set; }
    public ExpenseCategory Category { get; set; }
    public string? CategoryDescription { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Remarks { get; set; }
    public string? ReceiptUrl { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Create a trip expense entry.</summary>
public class CreateExpenseDto
{
    public Guid? TransportVehicleId { get; set; }
    public ExpenseCategory Category { get; set; }
    public string? CategoryDescription { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Remarks { get; set; }
    public string? ReceiptUrl { get; set; }
}

/// <summary>Update a trip expense entry.</summary>
public class UpdateExpenseDto
{
    public ExpenseCategory? Category { get; set; }
    public string? CategoryDescription { get; set; }
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime? ExpenseDate { get; set; }
    public string? Remarks { get; set; }
    public string? ReceiptUrl { get; set; }
}

/// <summary>Summary of total expenses per job.</summary>
public class ExpenseSummaryDto
{
    public Guid TransportRequestId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public int ExpenseCount { get; set; }
    public ICollection<ExpenseCategoryTotalDto> CategoryBreakdown { get; set; } = new List<ExpenseCategoryTotalDto>();
}

public class ExpenseCategoryTotalDto
{
    public ExpenseCategory Category { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
}
