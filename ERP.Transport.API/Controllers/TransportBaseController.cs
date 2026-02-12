using Asp.Versioning;
using EPR.Shared.Contracts.Controllers;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Base controller for all Transport API controllers.
/// Provides strongly-typed access to UserContext — mirrors CRM's CrmBaseController.
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/transport/[controller]")]
[Produces("application/json")]
public abstract class TransportBaseController : BaseApiController
{
    /// <summary>
    /// Gets the current user's ID as a Guid
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var userContext = HttpContext.GetRequiredUserContext();
            if (!Guid.TryParse(userContext.UserId, out var userId))
            {
                throw new InvalidOperationException($"Invalid user ID format: {userContext.UserId}");
            }
            return userId;
        }
    }

    /// <summary>
    /// Gets the current user context
    /// </summary>
    protected UserContext CurrentUser => HttpContext.GetRequiredUserContext();

    /// <summary>
    /// Gets the current user's email
    /// </summary>
    protected string? CurrentUserEmail => HttpContext.GetUserContext()?.Email;

    /// <summary>
    /// Gets the current user's username
    /// </summary>
    protected string CurrentUsername => HttpContext.GetUserContext()?.Username ?? "Unknown";

    /// <summary>
    /// Checks if current user has the specified permission
    /// </summary>
    protected bool HasPermission(string permission)
    {
        return HttpContext.GetUserContext()?.HasPermission(permission) ?? false;
    }

    /// <summary>
    /// Gets the list of permissions granted to the current user
    /// </summary>
    protected IEnumerable<string> CurrentUserPermissions =>
        HttpContext.GetUserContext()?.Permissions ?? Array.Empty<string>();
}
