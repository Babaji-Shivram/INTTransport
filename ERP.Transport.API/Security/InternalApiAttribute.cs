using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.Transport.API.Security;

/// <summary>
/// Authorization filter validating X-Internal-Key header for MS-to-MS calls.
/// Exact CRM pattern: 500 if key not configured, 401 if header missing, 403 if key invalid.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class InternalApiAttribute : Attribute, IAuthorizationFilter
{
    private const string InternalKeyHeader = "X-Internal-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();

        var configuredKey = configuration["Security:InternalApiKey"];

        if (string.IsNullOrEmpty(configuredKey))
        {
            context.Result = new ObjectResult(new
            {
                error = "Internal API key is not configured on the server."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        if (!context.HttpContext.Request.Headers
                .TryGetValue(InternalKeyHeader, out var providedKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Internal API key header is missing.",
                requiredHeader = InternalKeyHeader
            });
            return;
        }

        if (!string.Equals(configuredKey, providedKey.ToString(), StringComparison.Ordinal))
        {
            context.Result = new ObjectResult(new
            {
                error = "Invalid internal API key."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
