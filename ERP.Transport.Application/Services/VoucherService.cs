using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using ERP.Transport.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Payment voucher service — generate and track vouchers for expenses.
/// Legacy: VesselExpense.aspx iTextSharp PDF generation (Cash/Cheque/NEFT/RTGS).
/// </summary>
public class VoucherService : IVoucherService
{
    private readonly IRepository<PaymentVoucher> _voucherRepo;
    private readonly IRepository<TransportExpense> _expenseRepo;
    private readonly IRepository<MaintenanceWorkOrder> _workOrderRepo;
    private readonly IRepository<VehicleDailyExpense> _dailyExpenseRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<VoucherService> _logger;

    public VoucherService(
        IRepository<PaymentVoucher> voucherRepo,
        IRepository<TransportExpense> expenseRepo,
        IRepository<MaintenanceWorkOrder> workOrderRepo,
        IRepository<VehicleDailyExpense> dailyExpenseRepo,
        IUnitOfWork uow,
        ILogger<VoucherService> logger)
    {
        _voucherRepo = voucherRepo;
        _expenseRepo = expenseRepo;
        _workOrderRepo = workOrderRepo;
        _dailyExpenseRepo = dailyExpenseRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<PaymentVoucherDto> CreateVoucherAsync(CreatePaymentVoucherDto dto, Guid userId)
    {
        // Validate at least one source is linked
        if (dto.TransportExpenseId == null && dto.MaintenanceWorkOrderId == null && dto.VehicleDailyExpenseId == null)
            throw new TransportValidationException(nameof(dto.TransportExpenseId), "At least one source (expense/work order/daily expense) must be linked");

        var voucherNumber = await GetNextVoucherNumberAsync(dto.BranchId);

        var entity = new PaymentVoucher
        {
            Id = Guid.NewGuid(),
            VoucherNumber = voucherNumber,
            VoucherDate = DateTime.UtcNow,
            VoucherType = dto.VoucherType,
            PaymentMode = dto.PaymentMode,
            PaidTo = dto.PaidTo,
            Amount = dto.Amount,
            AmountInWords = ConvertToWords(dto.Amount),
            CurrencyCode = "INR",
            Description = dto.Description,
            BankName = dto.BankName,
            ChequeNumber = dto.ChequeNumber,
            ChequeDate = dto.ChequeDate,
            TransactionReference = dto.TransactionReference,
            AccountNumber = dto.AccountNumber,
            IFSCCode = dto.IFSCCode,
            BillNumber = dto.BillNumber,
            BillDate = dto.BillDate,
            TransportExpenseId = dto.TransportExpenseId,
            MaintenanceWorkOrderId = dto.MaintenanceWorkOrderId,
            VehicleDailyExpenseId = dto.VehicleDailyExpenseId,
            BranchId = dto.BranchId,
            CountryCode = dto.CountryCode,
            IsPrinted = false,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _voucherRepo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Voucher {No} created: {Mode} {Amount} to {PaidTo}",
            voucherNumber, dto.PaymentMode, dto.Amount, dto.PaidTo);

        return MapToDto(entity);
    }

    public async Task<PaymentVoucherDto?> GetByIdAsync(Guid id)
    {
        var entity = await _voucherRepo.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<PagedResultDto<PaymentVoucherDto>> GetAllAsync(VoucherFilterDto filter)
    {
        var query = await _voucherRepo.FindAsync(v => !v.IsDeleted);
        var items = query.AsQueryable();

        if (filter.VoucherType.HasValue)
            items = items.Where(v => v.VoucherType == filter.VoucherType);
        if (filter.PaymentMode.HasValue)
            items = items.Where(v => v.PaymentMode == filter.PaymentMode);
        if (filter.FromDate.HasValue)
            items = items.Where(v => v.VoucherDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            items = items.Where(v => v.VoucherDate <= filter.ToDate.Value);
        if (filter.BranchId.HasValue)
            items = items.Where(v => v.BranchId == filter.BranchId);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            items = items.Where(v => v.VoucherNumber.Contains(filter.Search) || v.PaidTo.Contains(filter.Search));

        var total = items.Count();
        var data = items
            .OrderByDescending(v => v.VoucherDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResultDto<PaymentVoucherDto>
        {
            Items = data,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<byte[]> GenerateVoucherPdfAsync(Guid voucherId)
    {
        var entity = await _voucherRepo.FirstOrDefaultAsync(v => v.Id == voucherId && !v.IsDeleted)
            ?? throw new TransportNotFoundException("PaymentVoucher", voucherId);

        // Build HTML voucher from template
        var html = BuildVoucherHtml(entity);

        // Mark as printed
        entity.IsPrinted = true;
        entity.PrintedDate = DateTime.UtcNow;
        _voucherRepo.Update(entity);
        await _uow.SaveChangesAsync();

        // Return HTML as UTF-8 bytes (frontend can render as PDF via browser print)
        // For server-side PDF, integrate with a PDF library (e.g., QuestPDF, iText7)
        return System.Text.Encoding.UTF8.GetBytes(html);
    }

    public async Task<string> GetNextVoucherNumberAsync(Guid branchId)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;
        var count = await _voucherRepo.CountAsync(v =>
            v.BranchId == branchId &&
            v.VoucherDate.Year == year &&
            v.VoucherDate.Month == month &&
            !v.IsDeleted);

        return $"PV-{year}{month:D2}-{(count + 1):D5}";
    }

    private static string BuildVoucherHtml(PaymentVoucher v) => $@"
<!DOCTYPE html>
<html>
<head><title>Payment Voucher - {v.VoucherNumber}</title>
<style>
    body {{ font-family: Arial, sans-serif; max-width: 800px; margin: auto; padding: 20px; }}
    .header {{ text-align: center; border-bottom: 2px solid #333; padding-bottom: 10px; }}
    .voucher-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
    .voucher-table td {{ padding: 8px; border: 1px solid #ddd; }}
    .voucher-table .label {{ font-weight: bold; width: 30%; background: #f5f5f5; }}
    .amount {{ font-size: 1.2em; font-weight: bold; color: #333; }}
    .footer {{ margin-top: 40px; display: flex; justify-content: space-between; }}
    .signature {{ text-align: center; border-top: 1px solid #333; padding-top: 5px; width: 200px; }}
</style>
</head>
<body>
    <div class='header'>
        <h2>PAYMENT VOUCHER</h2>
        <p>Voucher No: <strong>{v.VoucherNumber}</strong> &nbsp; | &nbsp; Date: <strong>{v.VoucherDate:dd-MMM-yyyy}</strong></p>
    </div>
    <table class='voucher-table'>
        <tr><td class='label'>Paid To</td><td>{v.PaidTo}</td></tr>
        <tr><td class='label'>Amount</td><td class='amount'>{v.CurrencyCode} {v.Amount:N2}</td></tr>
        <tr><td class='label'>Amount in Words</td><td>{v.AmountInWords}</td></tr>
        <tr><td class='label'>Payment Mode</td><td>{v.PaymentMode}</td></tr>
        {(v.PaymentMode != VoucherPaymentMode.Cash ? $@"
        <tr><td class='label'>Bank Name</td><td>{v.BankName}</td></tr>
        <tr><td class='label'>Cheque/Txn Ref</td><td>{v.ChequeNumber ?? v.TransactionReference}</td></tr>
        " : "")}
        <tr><td class='label'>Description</td><td>{v.Description}</td></tr>
        {(v.BillNumber != null ? $"<tr><td class='label'>Bill No.</td><td>{v.BillNumber} / {v.BillDate:dd-MMM-yyyy}</td></tr>" : "")}
    </table>
    <div class='footer'>
        <div class='signature'>Prepared By</div>
        <div class='signature'>Approved By</div>
        <div class='signature'>Received By</div>
    </div>
</body>
</html>";

    private static string ConvertToWords(decimal amount)
    {
        var intPart = (long)Math.Floor(amount);
        var decPart = (int)((amount - intPart) * 100);

        var result = NumberToWords(intPart) + " Rupees";
        if (decPart > 0)
            result += " and " + NumberToWords(decPart) + " Paise";
        result += " Only";
        return result;
    }

    private static string NumberToWords(long number)
    {
        if (number == 0) return "Zero";

        var ones = new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        var tens = new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        if (number < 20) return ones[number];
        if (number < 100) return tens[number / 10] + (number % 10 > 0 ? " " + ones[number % 10] : "");
        if (number < 1000) return ones[number / 100] + " Hundred" + (number % 100 > 0 ? " and " + NumberToWords(number % 100) : "");
        if (number < 100000) return NumberToWords(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + NumberToWords(number % 1000) : "");
        if (number < 10000000) return NumberToWords(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + NumberToWords(number % 100000) : "");
        return NumberToWords(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + NumberToWords(number % 10000000) : "");
    }

    private static PaymentVoucherDto MapToDto(PaymentVoucher v) => new()
    {
        Id = v.Id,
        VoucherNumber = v.VoucherNumber,
        VoucherDate = v.VoucherDate,
        VoucherType = v.VoucherType,
        PaymentMode = v.PaymentMode,
        PaidTo = v.PaidTo,
        Amount = v.Amount,
        AmountInWords = v.AmountInWords,
        CurrencyCode = v.CurrencyCode,
        Description = v.Description,
        BankName = v.BankName,
        ChequeNumber = v.ChequeNumber,
        TransactionReference = v.TransactionReference,
        BillNumber = v.BillNumber,
        PdfUrl = v.PdfUrl,
        IsPrinted = v.IsPrinted,
        TransportExpenseId = v.TransportExpenseId,
        MaintenanceWorkOrderId = v.MaintenanceWorkOrderId,
        VehicleDailyExpenseId = v.VehicleDailyExpenseId,
        CreatedDate = v.CreatedDate
    };
}
