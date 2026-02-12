using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.Transport.API.Filters;

/// <summary>
/// Separates public (v1) from internal swagger documents.
/// Internal endpoints are identified by route containing "internal".
/// Mirrors CRM's InternalApiDocumentFilter.
/// </summary>
public class InternalApiDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new List<string>();

        foreach (var path in swaggerDoc.Paths)
        {
            var isInternalPath = path.Key.Contains("/internal/",
                StringComparison.OrdinalIgnoreCase);

            if (swaggerDoc.Info.Version == "internal" && !isInternalPath)
            {
                pathsToRemove.Add(path.Key);
            }
            else if (swaggerDoc.Info.Version != "internal" && isInternalPath)
            {
                pathsToRemove.Add(path.Key);
            }
        }

        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
    }
}
