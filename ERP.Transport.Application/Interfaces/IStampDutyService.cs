using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces;

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
