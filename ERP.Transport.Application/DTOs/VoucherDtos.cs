using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Payment Voucher DTOs (Legacy: VesselExpense.aspx PDF voucher)
// ═══════════════════════════════════════════════════════════════

public class PaymentVoucherDto
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; } = null!;
    public DateTime VoucherDate { get; set; }
    public VoucherType VoucherType { get; set; }
    public VoucherPaymentMode PaymentMode { get; set; }
    public string PaidTo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? AmountInWords { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? Description { get; set; }
    public string? BankName { get; set; }
    public string? ChequeNumber { get; set; }
    public string? TransactionReference { get; set; }
    public string? BillNumber { get; set; }
    public string? PdfUrl { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime CreatedDate { get; set; }

    // ── Linked sources ──────────────────────────────────────────
    public Guid? TransportExpenseId { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }
    public Guid? VehicleDailyExpenseId { get; set; }
}

public class CreatePaymentVoucherDto
{
    public VoucherType VoucherType { get; set; }
    public VoucherPaymentMode PaymentMode { get; set; }
    public string PaidTo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    // ── Payment Details ─────────────────────────────────────────
    public string? BankName { get; set; }
    public string? ChequeNumber { get; set; }
    public DateTime? ChequeDate { get; set; }
    public string? TransactionReference { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }

    // ── Bill ────────────────────────────────────────────────────
    public string? BillNumber { get; set; }
    public DateTime? BillDate { get; set; }

    // ── Source link (one required) ──────────────────────────────
    public Guid? TransportExpenseId { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }
    public Guid? VehicleDailyExpenseId { get; set; }

    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = "IN";
}

public class VoucherFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public VoucherType? VoucherType { get; set; }
    public VoucherPaymentMode? PaymentMode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? BranchId { get; set; }
    public string? Search { get; set; }
}
