using ERP.Transport.Application.DTOs.Consolidation;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Consolidation trip management — dedicated lifecycle operations.
/// </summary>
public class ConsolidationsController : TransportBaseController
{
    private readonly IConsolidationService _consolidationService;

    public ConsolidationsController(IConsolidationService consolidationService)
    {
        _consolidationService = consolidationService;
    }

    /// <summary>List active consolidations (Draft, Confirmed, VehicleAssigned, InTransit).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ConsolidationSummaryDto>>>> GetActive()
    {
        var result = await _consolidationService.GetActiveConsolidationsAsync();
        return OkResponse(result);
    }

    /// <summary>Get consolidation details with linked jobs.</summary>
    [HttpGet("{tripId:guid}")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> GetById(Guid tripId)
    {
        var result = await _consolidationService.GetByIdAsync(tripId);
        if (result == null)
            return NotFoundResponse<ConsolidatedTripDto>("Consolidated trip not found");
        return OkResponse(result);
    }

    /// <summary>Create a new consolidation from multiple jobs.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> Create(
        [FromBody] CreateConsolidationRequest request)
    {
        var result = await _consolidationService.CreateAsync(request, CurrentUserId);
        return OkResponse(result, "Consolidation created");
    }

    /// <summary>Add a job to an existing consolidation.</summary>
    [HttpPost("{tripId:guid}/jobs")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> AddJob(
        Guid tripId, [FromBody] AddJobToConsolidationRequest request)
    {
        var result = await _consolidationService.AddJobAsync(tripId, request, CurrentUserId);
        return OkResponse(result, "Job added to consolidation");
    }

    /// <summary>Remove a job from a consolidation.</summary>
    [HttpDelete("{tripId:guid}/jobs/{requestId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> RemoveJob(Guid tripId, Guid requestId)
    {
        await _consolidationService.RemoveJobAsync(tripId, requestId, CurrentUserId);
        return OkResponse<object?>(null, "Job removed from consolidation");
    }

    /// <summary>Dispatch the consolidated trip — sets all jobs to InTransit.</summary>
    [HttpPost("{tripId:guid}/dispatch")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> Dispatch(Guid tripId)
    {
        var result = await _consolidationService.DispatchAsync(tripId, CurrentUserId);
        return OkResponse(result, "Consolidation dispatched");
    }

    /// <summary>Mark consolidated trip as complete — sets all jobs to Delivered.</summary>
    [HttpPost("{tripId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> Complete(Guid tripId)
    {
        var result = await _consolidationService.CompleteAsync(tripId, CurrentUserId);
        return OkResponse(result, "Consolidation completed");
    }

    /// <summary>Cancel a consolidation — unlinks all jobs.</summary>
    [HttpPost("{tripId:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<object?>>> Cancel(
        Guid tripId, [FromBody] CancelConsolidationRequest request)
    {
        await _consolidationService.CancelAsync(tripId, request.Reason, CurrentUserId);
        return OkResponse<object?>(null, "Consolidation cancelled");
    }
}
