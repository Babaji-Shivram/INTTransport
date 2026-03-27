using ERP.Transport.Application.DTOs.Rate;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Vehicle rate management — independent CRUD + search for rates.
/// </summary>
public class RatesController : TransportBaseController
{
    private readonly IVehicleRateService _rateService;

    public RatesController(IVehicleRateService rateService)
    {
        _rateService = rateService;
    }

    /// <summary>Search rates with filters.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<VehicleRateListDto>>>> Search(
        [FromQuery] RateSearchRequest request)
    {
        var result = await _rateService.SearchAsync(request);
        return OkResponse(result);
    }

    /// <summary>Get rate by ID.</summary>
    [HttpGet("{rateId:guid}")]
    public async Task<ActionResult<ApiResponse<VehicleRateMasterDto>>> GetById(Guid rateId)
    {
        var result = await _rateService.GetByIdAsync(rateId);
        if (result == null)
            return NotFoundResponse<VehicleRateMasterDto>("Vehicle rate not found");
        return OkResponse(result);
    }

    /// <summary>Find the best (cheapest approved) rate.</summary>
    [HttpGet("best")]
    public async Task<ActionResult<ApiResponse<VehicleRateMasterDto>>> GetBestRate(
        [FromQuery] Guid? transporterId,
        [FromQuery] Guid? transportVehicleId)
    {
        var result = await _rateService.GetBestRateAsync(transporterId, transportVehicleId);
        if (result == null)
            return NotFoundResponse<VehicleRateMasterDto>("No approved rate found matching criteria");
        return OkResponse(result);
    }

    /// <summary>Create a new rate entry.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<VehicleRateMasterDto>>> Create(
        [FromBody] CreateVehicleRateRequest request)
    {
        var result = await _rateService.CreateAsync(request, CurrentUserId);
        return OkResponse(result, "Vehicle rate created");
    }

    /// <summary>Update an existing rate.</summary>
    [HttpPut("{rateId:guid}")]
    public async Task<ActionResult<ApiResponse<VehicleRateMasterDto>>> Update(
        Guid rateId, [FromBody] UpdateVehicleRateRequest request)
    {
        var result = await _rateService.UpdateAsync(rateId, request, CurrentUserId);
        return OkResponse(result, "Vehicle rate updated");
    }

    /// <summary>Soft-delete a rate.</summary>
    [HttpDelete("{rateId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid rateId)
    {
        await _rateService.DeleteAsync(rateId, CurrentUserId);
        return OkResponse<object?>(null, "Vehicle rate deleted");
    }
}
