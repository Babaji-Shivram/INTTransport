using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ERP.Transport.Application.Services.Clients;

/// <summary>
/// HttpClient calling IAM MS for user/role lookups.
/// </summary>
public class IdentityClient : IIdentityClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IdentityClient> _logger;

    public IdentityClient(IHttpClientFactory httpClientFactory, ILogger<IdentityClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("IdentityService");

    public async Task<IdentityUserDto?> GetUserAsync(Guid userId)
    {
        try
        {
            return await CreateClient().GetFromJsonAsync<IdentityUserDto>(
                $"api/v1/iam/internal/users/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {UserId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<IdentityUserDto>> GetUsersByRoleAsync(
        string roleName, string? countryCode = null, Guid? branchId = null)
    {
        try
        {
            var url = $"api/v1/iam/internal/users/by-role/{Uri.EscapeDataString(roleName)}";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(countryCode))
                queryParams.Add($"countryCode={Uri.EscapeDataString(countryCode)}");
            if (branchId.HasValue)
                queryParams.Add($"branchId={branchId.Value}");
            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var result = await CreateClient().GetFromJsonAsync<List<IdentityUserDto>>(url);
            return result ?? Enumerable.Empty<IdentityUserDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users by role {RoleName}", roleName);
            return Enumerable.Empty<IdentityUserDto>();
        }
    }
}
