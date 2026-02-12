using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.Transport.API.Filters;

/// <summary>
/// Adds X-Internal-Key header parameter to internal endpoints in swagger UI.
/// Mirrors CRM's InternalApiOperationFilter.
/// </summary>
public class InternalApiOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the controller or method has InternalApiAttribute
        var hasInternalAttribute = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(typeof(Security.InternalApiAttribute), true).Any() == true
            || context.MethodInfo
                .GetCustomAttributes(typeof(Security.InternalApiAttribute), true).Any();

        if (!hasInternalAttribute)
            return;

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Internal-Key",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Internal API key for microservice-to-microservice communication",
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}
