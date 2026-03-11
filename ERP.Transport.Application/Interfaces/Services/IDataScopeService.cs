using EPR.Shared.Contracts.Models;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Service-layer data scoping — determines what data the current user can access
/// based on their permission scope (All / Branch / Own).
/// Uses SharedLibrary's DataScopeResult and DataScopeLevel types.
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
