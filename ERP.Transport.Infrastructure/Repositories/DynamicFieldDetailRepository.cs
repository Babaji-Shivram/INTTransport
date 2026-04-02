using EPR.Shared.Contracts.Entities;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Transport.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for DynamicFieldDetail operations.
/// Standalone (does not extend Repository&lt;T&gt;) because DynamicFieldDetail
/// inherits from AuditableEntity, not Transport's BaseEntity.
/// </summary>
public class DynamicFieldDetailRepository : IDynamicFieldDetailRepository
{
    private readonly TransportDbContext _dbContext;

    public DynamicFieldDetailRepository(TransportDbContext context)
    {
        _dbContext = context;
    }

    public async Task<DynamicFieldDetail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DynamicFieldDetails
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DynamicFieldDetail> AddAsync(DynamicFieldDetail entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.DynamicFieldDetails.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(DynamicFieldDetail entity, CancellationToken cancellationToken = default)
    {
        _dbContext.DynamicFieldDetails.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<DynamicFieldDetail>> GetByParentAsync(
        Guid parentEntityId, string parentEntityType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DynamicFieldDetails
            .Where(d => d.ParentEntityId == parentEntityId
                     && d.ParentEntityType == parentEntityType
                     && !d.IsDeleted)
            .OrderBy(d => d.SectionOrder)
            .ThenBy(d => d.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
