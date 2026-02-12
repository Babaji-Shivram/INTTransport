using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// Dashboard data service.
/// </summary>
public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid userId, string? countryCode, Guid? branchId);
}
