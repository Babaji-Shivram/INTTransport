using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.Interfaces;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// CharteredInfo Integration — GST Lookup, e-Invoice (IRN), E-Way Bill, QR Code.
/// </summary>
public class EInvoiceController : TransportBaseController
{
    private readonly ICharteredInfoService _charteredInfoService;

    public EInvoiceController(ICharteredInfoService charteredInfoService)
    {
        _charteredInfoService = charteredInfoService;
    }

    // ── Health ──────────────────────────────────────────────────

    /// <summary>Check CharteredInfo API connectivity and token status.</summary>
    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<CharteredInfoHealthDto>>> Health()
    {
        var result = await _charteredInfoService.HealthCheckAsync();
        return OkResponse(result);
    }

    // ── GST Lookup ──────────────────────────────────────────────

    /// <summary>Lookup GSTIN details (cached 24h).</summary>
    [HttpGet("gst/{gstin}")]
    public async Task<ActionResult<ApiResponse<GstDetailDto>>> LookupGst(
        string gstin, [FromQuery] bool forceRefresh = false)
    {
        var request = new GstLookupRequestDto { Gstin = gstin, ForceRefresh = forceRefresh };
        var result = await _charteredInfoService.LookupGstAsync(request);
        return OkResponse(result);
    }

    // ── Generate IRN (e-Invoice) ────────────────────────────────

    /// <summary>Generate e-Invoice and get IRN from NIC via CharteredInfo.</summary>
    [HttpPost("irn/generate")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> GenerateIrn(
        [FromBody] GenerateIrnRequestDto request)
    {
        var result = await _charteredInfoService.GenerateIrnAsync(request, CurrentUserId);
        return OkResponse(result, "IRN generated successfully");
    }

    // ── Get IRN ─────────────────────────────────────────────────

    /// <summary>Get e-Invoice details by IRN hash.</summary>
    [HttpGet("irn/{irn}")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> GetIrn(string irn)
    {
        var result = await _charteredInfoService.GetIrnAsync(irn);
        if (result == null)
            return NotFoundResponse<EInvoiceDto>("IRN not found");
        return OkResponse(result);
    }

    /// <summary>Get IRN by document details (type/number/date).</summary>
    [HttpGet("irn/by-doc")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> GetIrnByDoc(
        [FromQuery] string docType, [FromQuery] string docNum, [FromQuery] string docDate)
    {
        var result = await _charteredInfoService.GetIrnByDocumentAsync(docType, docNum, docDate);
        if (result == null)
            return NotFoundResponse<EInvoiceDto>("IRN not found for given document");
        return OkResponse(result);
    }

    // ── Cancel IRN ──────────────────────────────────────────────

    /// <summary>Cancel an e-Invoice IRN.</summary>
    [HttpPost("irn/cancel")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> CancelIrn(
        [FromBody] CancelIrnRequestDto request)
    {
        var result = await _charteredInfoService.CancelIrnAsync(request, CurrentUserId);
        return OkResponse(result, "IRN cancelled successfully");
    }

    // ── Generate E-Way Bill from IRN ────────────────────────────

    /// <summary>Generate E-Way Bill from an existing e-Invoice IRN.</summary>
    [HttpPost("ewb/generate")]
    public async Task<ActionResult<ApiResponse<EInvoiceDto>>> GenerateEwb(
        [FromBody] GenerateEwbFromIrnRequestDto request)
    {
        var result = await _charteredInfoService.GenerateEwbFromIrnAsync(request, CurrentUserId);
        return OkResponse(result, "E-Way Bill generated successfully");
    }

    // ── Cancel E-Way Bill ───────────────────────────────────────

    /// <summary>Cancel an E-Way Bill.</summary>
    [HttpPost("ewb/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> CancelEwb(
        [FromBody] CancelEwbRequestDto request)
    {
        await _charteredInfoService.CancelEwbAsync(request);
        return OkResponse<object>(null!, "E-Way Bill cancelled successfully");
    }

    // ── Get by Transport Job ────────────────────────────────────

    /// <summary>Get all e-Invoices for a transport job.</summary>
    [HttpGet("by-job/{jobId:guid}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EInvoiceDto>>>> GetByJob(Guid jobId)
    {
        var result = await _charteredInfoService.GetByTransportJobAsync(jobId);
        return OkResponse(result);
    }

    // ── Dynamic QR Code ─────────────────────────────────────────

    /// <summary>Generate a dynamic QR code for payment.</summary>
    [HttpPost("qr-code")]
    public async Task<ActionResult<ApiResponse<DynamicQrResponseDto>>> GenerateQrCode(
        [FromBody] DynamicQrRequestDto request)
    {
        var result = await _charteredInfoService.GetDynamicQrAsync(request);
        return OkResponse(result);
    }
}
