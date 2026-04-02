using EPR.Shared.Contracts.Entities;

namespace ERP.Transport.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for DynamicFieldDetail operations.
/// Standalone (not extending IRepository&lt;T&gt;) because DynamicFieldDetail
/// inherits from AuditableEntity directly, not Transport's BaseEntity.
/// </summary>
public interface IDynamicFieldDetailRepository
{
    Task<DynamicFieldDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DynamicFieldDetail> AddAsync(DynamicFieldDetail entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DynamicFieldDetail entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<DynamicFieldDetail>> GetByParentAsync(Guid parentEntityId, string parentEntityType, CancellationToken cancellationToken = default);
}
