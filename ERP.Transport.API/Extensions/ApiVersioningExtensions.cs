using Asp.Versioning;

namespace ERP.Transport.API.Extensions;

/// <summary>
/// API versioning configuration — URL segment + header + query string.
/// Matches IAM/Workflow patterns with dedicated extension method.
/// </summary>
public static class ApiVersioningExtensions
{
    public static IServiceCollection AddTransportApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"),
                new QueryStringApiVersionReader("api-version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
