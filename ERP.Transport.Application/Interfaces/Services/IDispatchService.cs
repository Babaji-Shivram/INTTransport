using ERP.Transport.Application.DTOs.Dispatch;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Dispatch management — tracks when jobs are physically dispatched.
/// </summary>
public interface IDispatchService
{
    Task<List<DispatchDto>> GetPendingDispatchAsync(CancellationToken ct = default);
    Task<List<DispatchDto>> GetDispatchedTodayAsync(CancellationToken ct = default);
    Task<DispatchDto> DispatchJobAsync(Guid requestId, CreateDispatchRequest request, Guid userId, CancellationToken ct = default);
    Task<DispatchSummaryDto> GetSummaryAsync(DateTime? date, CancellationToken ct = default);
}
