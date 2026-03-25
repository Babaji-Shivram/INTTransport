using ERP.Transport.Application.Interfaces.Clients;
using ERP.Transport.Application.Services.Clients;
using Polly;
using Polly.Extensions.Http;

namespace ERP.Transport.API.Extensions;

/// <summary>
/// External service clients with Polly retry + circuit breaker.
/// Mirrors CRM's ExternalServicesExtensions.
/// </summary>
public static class ExternalServicesExtensions
{
    public static IServiceCollection AddExternalServiceClients(
        this IServiceCollection services, IConfiguration configuration)
    {
        var externalServices = configuration.GetSection("ExternalServices");
        var internalApiKey = configuration["Security:InternalApiKey"] ?? "";

        // Read resilience settings from config
        var httpSettings = configuration.GetSection("HttpClientSettings");
        var timeoutSeconds = httpSettings.GetValue("TimeoutSeconds", 30);
        var retryCount = httpSettings.GetValue("RetryCount", 3);
        var retryBaseDelayMs = httpSettings.GetValue("RetryBaseDelayMs", 1000);

        // ── Workflow Client ────────────────────────────────────
        services.AddHttpClient<IWorkflowClient, WorkflowClient>(client =>
        {
            client.BaseAddress = new Uri(externalServices["WorkflowServiceUrl"]!);
            client.DefaultRequestHeaders.Add("X-Internal-Key", internalApiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // ── Master Client ──────────────────────────────────────
        services.AddHttpClient("MasterService", client =>
        {
            client.BaseAddress = new Uri(externalServices["MasterServiceUrl"]!);
            client.DefaultRequestHeaders.Add("X-Internal-Key", internalApiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());
        services.AddScoped<IMasterClient, MasterClient>();

        // ── Identity Client ────────────────────────────────────
        services.AddHttpClient("IdentityService", client =>
        {
            client.BaseAddress = new Uri(externalServices["IdentityServiceUrl"]!);
            client.DefaultRequestHeaders.Add("X-Internal-Key", internalApiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());
        services.AddScoped<IIdentityClient, IdentityClient>();

        // ── ULIP Client (direct ULIP staging API) ─────────────
        services.AddHttpClient<IUlipClient, UlipClient>(client =>
        {
            var ulipBaseUrl = configuration["Ulip:BaseUrl"]
                ?? "https://www.ulipstaging.dpiit.gov.in/ulip/v1.0.0";
            client.BaseAddress = new Uri(ulipBaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // ── CharteredInfo Client (e-Invoice / GST sandbox) ─────
        services.AddHttpClient<ICharteredInfoClient, CharteredInfoClient>(client =>
        {
            var baseUrl = configuration["CharteredInfo:BaseUrl"]
                ?? "https://gstsandbox.charteredinfo.com";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // ── Freight Client (for callbacks when transport job completes) ──
        services.AddHttpClient("FreightService", client =>
        {
            client.BaseAddress = new Uri(externalServices["FreightServiceUrl"] ?? "http://localhost:5005");
            client.DefaultRequestHeaders.Add("X-Internal-Key", internalApiKey);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy(retryCount, retryBaseDelayMs))
        .AddPolicyHandler(GetCircuitBreakerPolicy());
        services.AddScoped<IFreightClient, FreightClient>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(
        int retryCount = 3, int baseDelayMs = 1000)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt =>
                TimeSpan.FromMilliseconds(baseDelayMs * Math.Pow(2, retryAttempt - 1)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
    }
}
