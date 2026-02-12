using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Payment voucher — tracks voucher generation for expenses (Cash/Cheque/NEFT/RTGS).
/// Legacy: VesselExpense.aspx → iTextSharp PDF voucher generation
/// </summary>
public class PaymentVoucher : BaseEntity
{
    public string VoucherNumber { get; set; } = null!;
    public DateTime VoucherDate { get; set; }

    // ── Linked To ───────────────────────────────────────────────
    public Guid? TransportExpenseId { get; set; }
    public Guid? MaintenanceWorkOrderId { get; set; }
    public Guid? VehicleDailyExpenseId { get; set; }

    // ── Voucher Details ─────────────────────────────────────────
    public VoucherType VoucherType { get; set; }
    public VoucherPaymentMode PaymentMode { get; set; }
    public string PaidTo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? AmountInWords { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? Description { get; set; }

    // ── Payment Details ─────────────────────────────────────────
    public string? BankName { get; set; }
    public string? ChequeNumber { get; set; }
    public DateTime? ChequeDate { get; set; }
    public string? TransactionReference { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }

    // ── Bill Reference ──────────────────────────────────────────
    public string? BillNumber { get; set; }
    public DateTime? BillDate { get; set; }

    // ── PDF ─────────────────────────────────────────────────────
    public string? PdfUrl { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime? PrintedDate { get; set; }

    // ── Org Scoping ─────────────────────────────────────────────
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = "IN";

    // ── Navigation ──────────────────────────────────────────────
    public TransportExpense? TransportExpense { get; set; }
    public MaintenanceWorkOrder? MaintenanceWorkOrder { get; set; }
    public VehicleDailyExpense? VehicleDailyExpense { get; set; }
}
