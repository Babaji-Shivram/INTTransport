using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ERP.Transport.API.Clients;

/// <summary>
/// HttpClient calling Masters MS for customer/city/branch/country lookups.
/// </summary>
public class MasterClient : IMasterClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MasterClient> _logger;

    public MasterClient(IHttpClientFactory httpClientFactory, ILogger<MasterClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("MasterService");

    public async Task<MasterLookupDto?> GetCustomerAsync(Guid customerId)
    {
        try
        {
            return await CreateClient().GetFromJsonAsync<MasterLookupDto>(
                $"api/v1/master/internal/customers/{customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer {CustomerId}", customerId);
            return null;
        }
    }

    public async Task<IEnumerable<MasterLookupDto>> SearchCustomersAsync(string query)
    {
        try
        {
            var result = await CreateClient().GetFromJsonAsync<List<MasterLookupDto>>(
                $"api/v1/master/internal/customers/search?q={Uri.EscapeDataString(query)}");
            return result ?? Enumerable.Empty<MasterLookupDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search customers with query {Query}", query);
            return Enumerable.Empty<MasterLookupDto>();
        }
    }

    public async Task<MasterLookupDto?> GetCityAsync(Guid cityId)
    {
        try
        {
            return await CreateClient().GetFromJsonAsync<MasterLookupDto>(
                $"api/v1/master/internal/cities/{cityId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get city {CityId}", cityId);
            return null;
        }
    }

    public async Task<IEnumerable<MasterLookupDto>> SearchCitiesAsync(string query)
    {
        try
        {
            var result = await CreateClient().GetFromJsonAsync<List<MasterLookupDto>>(
                $"api/v1/master/internal/cities/search?q={Uri.EscapeDataString(query)}");
            return result ?? Enumerable.Empty<MasterLookupDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search cities with query {Query}", query);
            return Enumerable.Empty<MasterLookupDto>();
        }
    }

    public async Task<MasterLookupDto?> GetBranchAsync(Guid branchId)
    {
        try
        {
            return await CreateClient().GetFromJsonAsync<MasterLookupDto>(
                $"api/v1/master/internal/branches/{branchId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get branch {BranchId}", branchId);
            return null;
        }
    }

    public async Task<MasterLookupDto?> GetCountryAsync(string countryCode)
    {
        try
        {
            return await CreateClient().GetFromJsonAsync<MasterLookupDto>(
                $"api/v1/master/internal/countries/{Uri.EscapeDataString(countryCode)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get country {CountryCode}", countryCode);
            return null;
        }
    }
}
