using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Warehouse;

namespace ERP.Transport.Application.Interfaces.Services;

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
