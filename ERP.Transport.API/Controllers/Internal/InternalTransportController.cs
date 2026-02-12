using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.DTOs.Workflow;
using ERP.Transport.Application.Interfaces;
using ERP.Transport.API.Security;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers.Internal;

/// <summary>
/// Internal (MS-to-MS) endpoints — secured by X-Internal-Key, no JWT required.
/// </summary>
[AllowAnonymous]
[InternalApi]
[Route("api/v{version:apiVersion}/transport/internal")]
public class InternalTransportController : TransportBaseController
{
    private readonly ITransportJobService _jobService;

    public InternalTransportController(ITransportJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>Create transport job from another microservice.</summary>
    [HttpPost("jobs")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> CreateJob(
        [FromBody] CreateTransportJobDto dto)
    {
        var result = await _jobService.CreateJobAsync(dto, Guid.Empty, "IN", Guid.Empty);
        return OkResponse(result, "Transport job created via internal API");
    }

    /// <summary>Get job status (for CRM/Freight polling).</summary>
    [HttpGet("jobs/{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<TransportJobStatusDto>>> GetJobStatus(Guid id)
    {
        var result = await _jobService.GetJobStatusAsync(id);
        if (result == null)
            return NotFoundResponse<TransportJobStatusDto>("Transport job not found");
        return OkResponse(result);
    }

    /// <summary>Workflow callback — advances job status when Workflow MS notifies.</summary>
    [HttpPost("jobs/{id:guid}/workflow-callback")]
    public async Task<ActionResult<ApiResponse<object?>>> WorkflowCallback(
        Guid id, [FromBody] WorkflowCallbackDto callback)
    {
        await _jobService.HandleWorkflowCallbackAsync(id, callback);
        return OkResponse<object?>(null, "Workflow callback processed");
    }
}
