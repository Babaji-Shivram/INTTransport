namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Client for Masters MS — lookup data (customers, cities, branches, countries).
/// </summary>
public interface IMasterClient
{
    Task<MasterLookupDto?> GetCustomerAsync(Guid customerId);
    Task<IEnumerable<MasterLookupDto>> SearchCustomersAsync(string query);
    Task<MasterLookupDto?> GetCityAsync(Guid cityId);
    Task<IEnumerable<MasterLookupDto>> SearchCitiesAsync(string query);
    Task<MasterLookupDto?> GetBranchAsync(Guid branchId);
    Task<MasterLookupDto?> GetCountryAsync(string countryCode);
}

/// <summary>
/// Generic lookup DTO returned from Masters MS.
/// </summary>
public class MasterLookupDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
