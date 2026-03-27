using ERP.Transport.Application.DTOs.Consolidation;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Consolidation trip management — dedicated lifecycle operations.
/// </summary>
public interface IConsolidationService
{
    Task<List<ConsolidationSummaryDto>> GetActiveConsolidationsAsync(CancellationToken ct = default);
    Task<ConsolidatedTripDto?> GetByIdAsync(Guid tripId, CancellationToken ct = default);
    Task<ConsolidatedTripDto> CreateAsync(CreateConsolidationRequest request, Guid userId, CancellationToken ct = default);
    Task<ConsolidatedTripDto> AddJobAsync(Guid tripId, AddJobToConsolidationRequest request, Guid userId, CancellationToken ct = default);
    Task RemoveJobAsync(Guid tripId, Guid transportRequestId, Guid userId, CancellationToken ct = default);
    Task<ConsolidatedTripDto> DispatchAsync(Guid tripId, Guid userId, CancellationToken ct = default);
    Task<ConsolidatedTripDto> CompleteAsync(Guid tripId, Guid userId, CancellationToken ct = default);
    Task CancelAsync(Guid tripId, string reason, Guid userId, CancellationToken ct = default);
}
