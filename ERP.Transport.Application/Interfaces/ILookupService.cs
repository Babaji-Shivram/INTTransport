using ERP.Transport.Application.DTOs;
using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// CRUD service for transport lookup / master tables.
/// </summary>
public interface ILookupService
{
    Task<TransportLookupDto> CreateAsync(CreateTransportLookupDto dto, Guid userId);
    Task<TransportLookupDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TransportLookupDto>> GetByCategoryAsync(LookupCategory category, string? countryCode = null, bool activeOnly = true);
    Task<IEnumerable<TransportLookupDto>> SearchAsync(LookupFilterDto filter);
    Task<TransportLookupDto> UpdateAsync(Guid id, UpdateTransportLookupDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task SeedDefaultsAsync(Guid userId);
}
