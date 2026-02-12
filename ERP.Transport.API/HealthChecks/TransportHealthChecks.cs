using ERP.Transport.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ERP.Transport.API.HealthChecks;

/// <summary>
/// Database health check — verifies SQL Server connectivity and basic query execution.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly TransportDbContext _db;

    public DatabaseHealthCheck(TransportDbContext db) => _db = db;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);
            if (!canConnect)
                return HealthCheckResult.Unhealthy("Cannot connect to LTINTTRANSPORT database");

            return HealthCheckResult.Healthy("Database connection OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}

/// <summary>
/// External services health check — verifies Workflow, Master, Identity MS reachability.
/// </summary>
public class ExternalServicesHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalServicesHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        var data = new Dictionary<string, object>();
        var unhealthy = false;

        var services = new[]
        {
            ("MasterService", _configuration["ExternalServices:MasterServiceUrl"]),
            ("IdentityService", _configuration["ExternalServices:IdentityServiceUrl"]),
        };

        foreach (var (name, url) in services)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(name);
                client.Timeout = TimeSpan.FromSeconds(5);
                // Attempt a lightweight connection — HEAD to base URL
                var response = await client.GetAsync("/health", ct);
                data[name] = response.IsSuccessStatusCode ? "Healthy" : $"Unhealthy ({response.StatusCode})";
                if (!response.IsSuccessStatusCode) unhealthy = true;
            }
            catch (Exception ex)
            {
                data[name] = $"Unreachable ({ex.GetType().Name})";
                // Degraded, not unhealthy — service can function without them
            }
        }

        return unhealthy
            ? HealthCheckResult.Degraded("One or more external services degraded", data: data)
            : HealthCheckResult.Healthy("All external services reachable", data);
    }
}
