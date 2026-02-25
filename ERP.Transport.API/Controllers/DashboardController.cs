using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Dashboard — aggregated metrics for transport operations.
/// </summary>
public class DashboardController : TransportBaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>Get transport dashboard summary.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> Get()
    {
        var userContext = HttpContext.GetRequiredUserContext();
        var countryCode = userContext.Countries?.FirstOrDefault() ?? "IN";
        var branchId = Guid.TryParse(
            userContext.Branches?.FirstOrDefault(), out var bid) ? bid : (Guid?)null;

        var result = await _dashboardService.GetDashboardAsync(
            CurrentUserId, countryCode, branchId);
        return OkResponse(result);
    }
}
