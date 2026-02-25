using System.Linq.Expressions;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Transport.Infrastructure.Repositories;

/// <summary>
/// Generic repository — mirrors CRM's Repository pattern exactly.
/// All queries auto-filter soft-deleted records via HasQueryFilter.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly TransportDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TransportDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
        }
        _dbSet.UpdateRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int page = 1,
        int pageSize = 10,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        foreach (var include in includes)
            query = query.Include(include);

        var totalCount = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);
        else
            query = query.OrderByDescending(e => e.CreatedDate);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
