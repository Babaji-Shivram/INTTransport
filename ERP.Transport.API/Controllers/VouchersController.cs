using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Payment vouchers — cash/cheque/NEFT/RTGS voucher generation &amp; PDF (legacy: PrintVoucher.aspx).
/// </summary>
public class VouchersController : TransportBaseController
{
    private readonly IVoucherService _svc;

    public VouchersController(IVoucherService svc) => _svc = svc;

    /// <summary>Create a payment voucher.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentVoucherDto>>> Create(
        [FromBody] CreatePaymentVoucherDto dto)
    {
        var result = await _svc.CreateVoucherAsync(dto, CurrentUserId);
        return OkResponse(result, "Voucher created");
    }

    /// <summary>Get all vouchers (paged + filtered).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<PaymentVoucherDto>>>> GetAll(
        [FromQuery] VoucherFilterDto filter)
    {
        var result = await _svc.GetAllAsync(filter);
        return OkResponse(result);
    }

    /// <summary>Get a voucher by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentVoucherDto>>> GetById(Guid id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null)
            return NotFoundResponse<PaymentVoucherDto>("Voucher not found");
        return OkResponse(result);
    }

    /// <summary>Generate PDF voucher for printing.</summary>
    [HttpGet("{id:guid}/pdf")]
    [Produces("text/html")]
    public async Task<IActionResult> GenerateVoucherPdf(Guid id)
    {
        var html = await _svc.GenerateVoucherPdfAsync(id);
        return Content(System.Text.Encoding.UTF8.GetString(html), "text/html");
    }

    /// <summary>Get the next voucher number (for UI preview).</summary>
    [HttpGet("next-number")]
    public async Task<ActionResult<ApiResponse<string>>> GetNextNumber([FromQuery] Guid branchId)
    {
        var result = await _svc.GetNextVoucherNumberAsync(branchId);
        return OkResponse(result);
    }
}
