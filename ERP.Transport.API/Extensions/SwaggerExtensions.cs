using ERP.Transport.API.Filters;
using Microsoft.OpenApi.Models;

namespace ERP.Transport.API.Extensions;

/// <summary>
/// Swagger with public (v1) + internal doc split — mirrors CRM's SwaggerExtensions.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Transport API",
                Version = "v1",
                Description = "LT Transport Microservice — manages transport jobs, vehicles, rates, delivery, and fleet."
            });

            c.SwaggerDoc("internal", new OpenApiInfo
            {
                Title = "Transport Internal APIs",
                Version = "internal",
                Description = "Internal APIs for microservice-to-microservice communication. Secured by X-Internal-Key."
            });

            // JWT Bearer auth
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Filters
            c.DocumentFilter<InternalApiDocumentFilter>();
            c.OperationFilter<InternalApiOperationFilter>();
        });

        return services;
    }
}
