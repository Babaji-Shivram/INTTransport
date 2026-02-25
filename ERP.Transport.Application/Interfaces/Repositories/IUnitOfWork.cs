namespace ERP.Transport.Application.Interfaces.Repositories;

/// <summary>
/// Unit of Work — transaction boundary, mirrors CRM pattern.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
