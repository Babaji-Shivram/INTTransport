using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// ULIP Integration — VAHAN, SARATHI, FASTag, Toll, E-Way Bill.
/// </summary>
public class UlipController : TransportBaseController
{
    private readonly IUlipService _ulipService;

    public UlipController(IUlipService ulipService)
    {
        _ulipService = ulipService;
    }

    // ── Health ──────────────────────────────────────────────────

    /// <summary>Check ULIP connectivity and token status.</summary>
    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<UlipHealthDto>>> Health()
    {
        var result = await _ulipService.HealthCheckAsync();
        return OkResponse(result);
    }

    // ── VAHAN — Vehicle Lookup ──────────────────────────────────

    /// <summary>Lookup vehicle details by registration number (VAHAN/01).</summary>
    [HttpGet("vehicle/{vehicleNumber}")]
    public async Task<ActionResult<ApiResponse<VehicleDetailDto>>> LookupVehicle(
        string vehicleNumber, [FromQuery] bool forceRefresh = false)
    {
        var request = new VehicleLookupRequestDto
        {
            VehicleNumber = vehicleNumber,
            ForceRefresh = forceRefresh
        };
        var result = await _ulipService.LookupVehicleAsync(request);
        return OkResponse(result);
    }

    // ── SARATHI — Driver License ────────────────────────────────

    /// <summary>Verify driver license (SARATHI/01).</summary>
    [HttpPost("driver-license")]
    public async Task<ActionResult<ApiResponse<DriverLicenseDetailDto>>> VerifyDriverLicense(
        [FromBody] DriverLicenseVerifyRequestDto request)
    {
        var result = await _ulipService.VerifyDriverLicenseAsync(request);
        return OkResponse(result);
    }

    // ── FASTag — Toll Tracking ──────────────────────────────────

    /// <summary>Get FASTag toll transactions for a vehicle (FASTAG/01).</summary>
    [HttpGet("fastag/{vehicleNumber}")]
    public async Task<ActionResult<ApiResponse<FASTagLookupResponseDto>>> GetFASTag(
        string vehicleNumber, [FromQuery] Guid? transportRequestId = null)
    {
        var result = await _ulipService.GetFASTagTransactionsAsync(
            vehicleNumber, transportRequestId);
        return OkResponse(result);
    }

    // ── TOLL — Plaza Details ────────────────────────────────────

    /// <summary>Get toll plaza details by plaza ID (TOLL/01).</summary>
    [HttpGet("toll/{plazaId}")]
    public async Task<ActionResult<ApiResponse<TollPlazaDto>>> GetTollPlaza(string plazaId)
    {
        var result = await _ulipService.GetTollPlazaAsync(plazaId);
        if (result == null)
            return NotFoundResponse<TollPlazaDto>("Toll plaza not found");
        return OkResponse(result);
    }

    // ── E-Way Bill ──────────────────────────────────────────────

    /// <summary>Generate E-Way Bill for a transport job.</summary>
    [HttpPost("eway-bill")]
    public async Task<ActionResult<ApiResponse<EWayBillDto>>> GenerateEWayBill(
        [FromBody] GenerateEWayBillRequestDto request)
    {
        var result = await _ulipService.GenerateEWayBillAsync(request, CurrentUserId);
        return OkResponse(result, "E-Way Bill record created");
    }

    /// <summary>Auto-fill E-Way Bill from transport job data (customer, addresses, vehicle).</summary>
    [HttpPost("eway-bill/from-job/{jobId:guid}")]
    public async Task<ActionResult<ApiResponse<EWayBillDto>>> GenerateEWayBillFromJob(Guid jobId)
    {
        var result = await _ulipService.GenerateEWayBillFromJobAsync(jobId, CurrentUserId);
        return OkResponse(result, "E-Way Bill auto-generated from job data");
    }

    /// <summary>Get E-Way Bill for a transport job.</summary>
    [HttpGet("eway-bill/by-job/{jobId:guid}")]
    public async Task<ActionResult<ApiResponse<EWayBillDto>>> GetEWayBillByJob(Guid jobId)
    {
        var result = await _ulipService.GetEWayBillByJobAsync(jobId);
        if (result == null)
            return NotFoundResponse<EWayBillDto>("No E-Way Bill found for this job");
        return OkResponse(result);
    }
}
