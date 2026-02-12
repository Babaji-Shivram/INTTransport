namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Payment voucher type — what the voucher is for.
/// </summary>
public enum VoucherType
{
    Expense = 0,
    Maintenance = 1,
    DailyExpense = 2,
    FundRequest = 3,
    Refund = 4
}

/// <summary>
/// Payment mode for vouchers.
/// </summary>
public enum VoucherPaymentMode
{
    Cash = 0,
    Cheque = 1,
    NEFT = 2,
    RTGS = 3,
    IMPS = 4,
    UPI = 5
}
