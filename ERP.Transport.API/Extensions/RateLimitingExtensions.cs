using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace ERP.Transport.API.Extensions;

/// <summary>
/// Rate limiting — 3 policies: standard (public APIs), strict (sensitive ops), internal (service-to-service).
/// Matches IAM/Workflow patterns. Config-driven from RateLimiting section.
/// </summary>
public static class RateLimitingExtensions
{
    public static IServiceCollection AddTransportRateLimiting(
        this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("RateLimiting");
        var standardPermit = section.GetValue("StandardPermitLimit", 100);
        var standardWindow = section.GetValue("StandardWindowSeconds", 60);
        var strictPermit = section.GetValue("StrictPermitLimit", 10);
        var strictWindow = section.GetValue("StrictWindowSeconds", 60);
        var internalPermit = section.GetValue("InternalPermitLimit", 1000);
        var internalWindow = section.GetValue("InternalWindowSeconds", 60);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Standard — for most public endpoints
            options.AddFixedWindowLimiter("standard", limiter =>
            {
                limiter.PermitLimit = standardPermit;
                limiter.Window = TimeSpan.FromSeconds(standardWindow);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 5;
            });

            // Strict — for sensitive/expensive operations (reports, exports, PDF gen)
            options.AddFixedWindowLimiter("strict", limiter =>
            {
                limiter.PermitLimit = strictPermit;
                limiter.Window = TimeSpan.FromSeconds(strictWindow);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 2;
            });

            // Internal — service-to-service calls (high throughput)
            options.AddFixedWindowLimiter("internal", limiter =>
            {
                limiter.PermitLimit = internalPermit;
                limiter.Window = TimeSpan.FromSeconds(internalWindow);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 50;
            });

            // Global fallback for unannotated endpoints
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = standardPermit,
                        Window = TimeSpan.FromSeconds(standardWindow)
                    }));
        });

        return services;
    }
}
