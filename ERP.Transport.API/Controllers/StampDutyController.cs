using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Stamp duty tracking — Indian compliance (legacy: StampDuty.aspx).
/// </summary>
public class StampDutyController : TransportBaseController
{
    private readonly IStampDutyService _svc;

    public StampDutyController(IStampDutyService svc) => _svc = svc;

    /// <summary>Create a stamp duty record.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StampDutyDto>>> Create(
        [FromBody] CreateStampDutyDto dto)
    {
        var result = await _svc.CreateAsync(dto, CurrentUserId);
        return OkResponse(result, "Stamp duty created");
    }

    /// <summary>Get all stamp duties (paged + filtered).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<StampDutyDto>>>> GetAll(
        [FromQuery] StampDutyFilterDto filter)
    {
        var result = await _svc.GetAllAsync(filter);
        return OkResponse(result);
    }

    /// <summary>Get a stamp duty by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StampDutyDto>>> GetById(Guid id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null)
            return NotFoundResponse<StampDutyDto>("Stamp duty not found");
        return OkResponse(result);
    }

    /// <summary>Update a stamp duty record.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StampDutyDto>>> Update(
        Guid id, [FromBody] UpdateStampDutyDto dto)
    {
        var result = await _svc.UpdateAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Stamp duty updated");
    }

    /// <summary>Record payment for a stamp duty.</summary>
    [HttpPost("{id:guid}/payment")]
    public async Task<ActionResult<ApiResponse<StampDutyDto>>> RecordPayment(
        Guid id, [FromBody] RecordStampDutyPaymentDto dto)
    {
        var result = await _svc.RecordPaymentAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Payment recorded");
    }

    /// <summary>Delete a stamp duty record (soft).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id)
    {
        await _svc.DeleteAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Stamp duty deleted");
    }

    /// <summary>Get stamp duties for a specific job.</summary>
    [HttpGet("by-job/{jobId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StampDutyDto>>>> GetByJob(Guid jobId)
    {
        var result = await _svc.GetByJobAsync(jobId);
        return OkResponse(result);
    }

    /// <summary>Get total unpaid stamp duty amount for a branch.</summary>
    [HttpGet("unpaid/{branchId:guid}")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetTotalUnpaid(Guid branchId)
    {
        var result = await _svc.GetTotalUnpaidByBranchAsync(branchId);
        return OkResponse(result);
    }
}
