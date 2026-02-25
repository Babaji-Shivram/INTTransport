using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Resolves data-access scope from the current HTTP request's UserContext.
/// Zero extra network I/O — all data comes from JWT via API Gateway headers.
/// Mirrors CRM's DataScopeService.
/// </summary>
public class DataScopeService : IDataScopeService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DataScopeService> _logger;

    public DataScopeService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<DataScopeService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Task<DataScopeResult> GetCurrentScopeAsync(
        string module, string permission, CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("[DataScope] No HttpContext – returning NoAccess");
            return Task.FromResult(DataScopeResult.NoAccess(Guid.Empty));
        }

        var userContext = httpContext.GetUserContext();
        if (userContext == null || string.IsNullOrEmpty(userContext.UserId))
        {
            _logger.LogWarning("[DataScope] No UserContext – returning NoAccess");
            return Task.FromResult(DataScopeResult.NoAccess(Guid.Empty));
        }

        if (!Guid.TryParse(userContext.UserId, out var userId))
        {
            _logger.LogWarning("[DataScope] Invalid UserId format: {UserId}", userContext.UserId);
            return Task.FromResult(DataScopeResult.NoAccess(Guid.Empty));
        }

        // ── SuperAdmin bypass ──────────────────────────────────
        if (userContext.IsSuperAdmin)
        {
            _logger.LogDebug("[DataScope] SuperAdmin {UserId} – full access", userId);
            return Task.FromResult(DataScopeResult.FullAccess(userId));
        }

        // ── Resolve scope from permission string ───────────────
        var scopeString = ResolvePermissionScope(userContext, module, permission);
        var branches = ParseBranchGuids(userContext);

        var result = scopeString switch
        {
            "All" => new DataScopeResult
            {
                Level = DataScopeLevel.All,
                UserId = userId,
                AllowedBranches = branches
            },
            "Branch" => new DataScopeResult
            {
                Level = DataScopeLevel.Branch,
                UserId = userId,
                AllowedBranches = branches
            },
            "Own" => new DataScopeResult
            {
                Level = DataScopeLevel.Own,
                UserId = userId,
                AllowedBranches = branches
            },
            _ => new DataScopeResult
            {
                Level = DataScopeLevel.None,
                UserId = userId
            }
        };

        _logger.LogDebug(
            "[DataScope] User {UserId} | {Module}.{Permission} → {Level} | Branches={BranchCount}",
            userId, module, permission, result.Level, result.AllowedBranches.Count);

        return Task.FromResult(result);
    }

    public async Task EnsureBranchAccessAsync(
        Guid branchId, string module, string permission, CancellationToken ct = default)
    {
        var scope = await GetCurrentScopeAsync(module, permission, ct);

        switch (scope.Level)
        {
            case DataScopeLevel.All:
                return;

            case DataScopeLevel.Branch:
            case DataScopeLevel.Own:
                if (scope.AllowedBranches.Contains(branchId))
                    return;
                _logger.LogWarning("[DataScope] Branch access denied. User {UserId}, Branch {BranchId}", scope.UserId, branchId);
                throw new UnauthorizedAccessException("You do not have access to this branch's data.");

            case DataScopeLevel.None:
            default:
                throw new UnauthorizedAccessException("You do not have permission to access this resource.");
        }
    }

    public async Task EnsureEntityAccessAsync(
        Guid entityBranchId, Guid entityCreatedBy, string module, string permission, CancellationToken ct = default)
    {
        var scope = await GetCurrentScopeAsync(module, permission, ct);

        switch (scope.Level)
        {
            case DataScopeLevel.All:
                return;

            case DataScopeLevel.Branch:
                if (scope.AllowedBranches.Contains(entityBranchId))
                    return;
                throw new UnauthorizedAccessException("You do not have access to this branch's data.");

            case DataScopeLevel.Own:
                if (entityCreatedBy == scope.UserId)
                    return;
                _logger.LogWarning("[DataScope] Own-access denied. User {UserId} vs CreatedBy {CreatedBy}", scope.UserId, entityCreatedBy);
                throw new UnauthorizedAccessException("You can only access records you created.");

            case DataScopeLevel.None:
            default:
                throw new UnauthorizedAccessException("You do not have permission to access this resource.");
        }
    }

    public async Task<IEnumerable<T>> ApplyScopeFilterAsync<T>(
        IEnumerable<T> items,
        Func<T, Guid> getBranchId,
        Func<T, Guid> getCreatedBy,
        string module,
        string permission,
        CancellationToken ct = default)
    {
        if (items == null || !items.Any())
            return items ?? Enumerable.Empty<T>();

        var scope = await GetCurrentScopeAsync(module, permission, ct);

        return scope.Level switch
        {
            DataScopeLevel.All => items,
            DataScopeLevel.Branch => items.Where(x => scope.AllowedBranches.Contains(getBranchId(x))),
            DataScopeLevel.Own => items.Where(x => getCreatedBy(x) == scope.UserId),
            _ => Enumerable.Empty<T>()
        };
    }

    // ════════════════════════════════════════════════════════════
    //  PRIVATE HELPERS
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Extract permission scope from UserContext.Permissions array.
    /// Looks for "{module}.{permission}.{scope}" format — e.g. "Transport.Read.Branch".
    /// Returns "All" / "Branch" / "Own" / null.
    /// </summary>
    private static string? ResolvePermissionScope(UserContext userContext, string module, string permission)
    {
        // Check for explicit scoped permission: Transport.Read.All, Transport.Read.Branch, Transport.Read.Own
        var scopes = new[] { "All", "Branch", "Own" };
        foreach (var scope in scopes)
        {
            if (userContext.HasPermission($"{module}.{permission}.{scope}"))
                return scope;
        }

        // If the user has the module-level permission without scope (e.g., "Transport.Read"),
        // default to "Own" for safety
        if (userContext.HasPermission($"{module}.{permission}"))
            return "Own";

        // Wildcard: e.g., "Transport.*" → full access
        if (userContext.HasPermission($"{module}.*"))
            return "All";

        return null;
    }

    /// <summary>
    /// Parse branch strings from UserContext.Branches into Guid list.
    /// </summary>
    private static List<Guid> ParseBranchGuids(UserContext userContext)
    {
        var guids = new List<Guid>();
        foreach (var b in userContext.Branches ?? Array.Empty<string>())
        {
            if (Guid.TryParse(b, out var guid))
                guids.Add(guid);
        }
        return guids;
    }
}
