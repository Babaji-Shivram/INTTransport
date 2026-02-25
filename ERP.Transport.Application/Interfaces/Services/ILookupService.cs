using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Warehouse;
using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.Interfaces.Services;

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
