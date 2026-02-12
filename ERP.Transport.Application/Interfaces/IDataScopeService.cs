using System.Linq.Expressions;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// Data scope levels — controls how much data a user can access.
/// All > Branch > Own.
/// </summary>
public enum DataScopeLevel
{
    All,
    Branch,
    Own,
    None
}

/// <summary>
/// Result of a scope check — includes the resolved scope level and user context.
/// </summary>
public class DataScopeResult
{
    public DataScopeLevel Level { get; set; }
    public Guid UserId { get; set; }
    public List<Guid> AllowedBranches { get; set; } = new();
    public List<Guid> AllowedCountries { get; set; } = new();

    public static DataScopeResult NoAccess(Guid userId) => new()
        { Level = DataScopeLevel.None, UserId = userId };

    public static DataScopeResult FullAccess(Guid userId) => new()
        { Level = DataScopeLevel.All, UserId = userId };
}

/// <summary>
/// Service-layer data scoping — determines what data the current user can access
/// based on their permission scope (All / Branch / Own).
/// Mirrors CRM's DataScopeService pattern.
/// </summary>
public interface IDataScopeService
{
    /// <summary>Get the current user's scope for a given permission group.</summary>
    Task<DataScopeResult> GetCurrentScopeAsync(string module, string permission, CancellationToken ct = default);

    /// <summary>Ensure the user has access to the specified branch for the operation.</summary>
    Task EnsureBranchAccessAsync(Guid branchId, string module, string permission, CancellationToken ct = default);

    /// <summary>
    /// Ensure the user has access to a specific entity (checks branch + ownership).
    /// </summary>
    Task EnsureEntityAccessAsync(Guid entityBranchId, Guid entityCreatedBy, string module, string permission, CancellationToken ct = default);

    /// <summary>
    /// Apply scope filter to an in-memory collection.
    /// </summary>
    Task<IEnumerable<T>> ApplyScopeFilterAsync<T>(
        IEnumerable<T> items,
        Func<T, Guid> getBranchId,
        Func<T, Guid> getCreatedBy,
        string module,
        string permission,
        CancellationToken ct = default);
}
