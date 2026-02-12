namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Client for IAM MS — user lookup, permission checks.
/// </summary>
public interface IIdentityClient
{
    Task<IdentityUserDto?> GetUserAsync(Guid userId);
    Task<IEnumerable<IdentityUserDto>> GetUsersByRoleAsync(string roleName, string? countryCode = null, Guid? branchId = null);
}

/// <summary>
/// User DTO from IAM MS.
/// </summary>
public class IdentityUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
}
