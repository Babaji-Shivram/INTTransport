using ERP.Transport.Application.DTOs.Dispatch;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Dispatch management — tracks when jobs are physically dispatched.
/// </summary>
public class DispatchController : TransportBaseController
{
    private readonly IDispatchService _dispatchService;

    public DispatchController(IDispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    /// <summary>Get jobs pending dispatch (vehicle assigned but not yet in transit).</summary>
    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponse<List<DispatchDto>>>> GetPending()
    {
        var result = await _dispatchService.GetPendingDispatchAsync();
        return OkResponse(result);
    }

    /// <summary>Get jobs dispatched today.</summary>
    [HttpGet("today")]
    public async Task<ActionResult<ApiResponse<List<DispatchDto>>>> GetDispatchedToday()
    {
        var result = await _dispatchService.GetDispatchedTodayAsync();
        return OkResponse(result);
    }

    /// <summary>Dispatch a job — set it in transit with movement tracking.</summary>
    [HttpPost("{requestId:guid}")]
    public async Task<ActionResult<ApiResponse<DispatchDto>>> DispatchJob(
        Guid requestId, [FromBody] CreateDispatchRequest request)
    {
        var result = await _dispatchService.DispatchJobAsync(requestId, request, CurrentUserId);
        return OkResponse(result, "Job dispatched");
    }

    /// <summary>Get dispatch summary/dashboard for a given date.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DispatchSummaryDto>>> GetSummary(
        [FromQuery] DateTime? date)
    {
        var result = await _dispatchService.GetSummaryAsync(date);
        return OkResponse(result);
    }
}
