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
/// Stamp duty management service — regulatory compliance tracking.
/// </summary>
public interface IStampDutyService
{
    Task<StampDutyDto> CreateAsync(CreateStampDutyDto dto, Guid userId);
    Task<StampDutyDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<StampDutyDto>> GetAllAsync(StampDutyFilterDto filter);
    Task<StampDutyDto> UpdateAsync(Guid id, UpdateStampDutyDto dto, Guid userId);
    Task<StampDutyDto> RecordPaymentAsync(Guid id, RecordStampDutyPaymentDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<IEnumerable<StampDutyDto>> GetByJobAsync(Guid transportRequestId);
    Task<decimal> GetTotalUnpaidByBranchAsync(Guid branchId);
}
