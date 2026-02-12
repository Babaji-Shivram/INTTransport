namespace ERP.Transport.API.Middleware;

/// <summary>
/// Middleware that intercepts /internal/ paths and validates X-Internal-Key header.
/// Mirrors CRM's InternalApiMiddleware.
/// </summary>
public class InternalApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private const string InternalKeyHeader = "X-Internal-Key";

    public InternalApiMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        if (path.Contains("/internal/", StringComparison.OrdinalIgnoreCase))
        {
            var configuredKey = _configuration["Security:InternalApiKey"];

            if (string.IsNullOrEmpty(configuredKey))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal API key is not configured."
                });
                return;
            }

            if (!context.Request.Headers.TryGetValue(InternalKeyHeader, out var providedKey)
                || !string.Equals(configuredKey, providedKey.ToString(), StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Invalid or missing internal API key."
                });
                return;
            }
        }

        await _next(context);
    }
}

public static class InternalApiMiddlewareExtensions
{
    public static IApplicationBuilder UseInternalApiMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<InternalApiMiddleware>();
    }
}
