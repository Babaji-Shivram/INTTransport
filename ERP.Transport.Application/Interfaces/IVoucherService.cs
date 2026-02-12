using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// Payment voucher service — generate and track payment vouchers (Cash/Cheque/NEFT/RTGS).
/// Legacy: VesselExpense.aspx iTextSharp PDF generation.
/// </summary>
public interface IVoucherService
{
    Task<PaymentVoucherDto> CreateVoucherAsync(CreatePaymentVoucherDto dto, Guid userId);
    Task<PaymentVoucherDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<PaymentVoucherDto>> GetAllAsync(VoucherFilterDto filter);
    Task<byte[]> GenerateVoucherPdfAsync(Guid voucherId);
    Task<string> GetNextVoucherNumberAsync(Guid branchId);
}
