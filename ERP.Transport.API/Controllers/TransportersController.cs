using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Transporter vendor master — CRUD + KYC + Bank.
/// </summary>
public class TransportersController : TransportBaseController
{
    private readonly ITransporterService _transporterService;

    public TransportersController(ITransporterService transporterService)
    {
        _transporterService = transporterService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<TransporterListDto>>>> GetAll(
        [FromQuery] TransporterFilterDto filter)
    {
        var result = await _transporterService.GetAllAsync(filter);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransporterDto>>> GetById(Guid id)
    {
        var result = await _transporterService.GetByIdAsync(id);
        if (result == null)
            return NotFoundResponse<TransporterDto>("Transporter not found");
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransporterDto>>> Create(
        [FromBody] CreateTransporterDto dto)
    {
        var result = await _transporterService.CreateAsync(dto, CurrentUserId);
        return OkResponse(result, "Transporter created");
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransporterDto>>> Update(
        Guid id, [FromBody] UpdateTransporterDto dto)
    {
        var result = await _transporterService.UpdateAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Transporter updated");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id)
    {
        await _transporterService.DeleteAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Transporter deleted");
    }

    // ── KYC ─────────────────────────────────────────────────────

    [HttpPost("{id:guid}/kyc")]
    public async Task<ActionResult<ApiResponse<TransporterKYCDto>>> AddKYC(
        Guid id, [FromBody] AddKYCDocumentDto dto)
    {
        var result = await _transporterService.AddKYCDocumentAsync(id, dto, CurrentUserId);
        return OkResponse(result, "KYC document added");
    }

    [HttpDelete("{id:guid}/kyc/{kycId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteKYC(Guid id, Guid kycId)
    {
        await _transporterService.DeleteKYCDocumentAsync(id, kycId, CurrentUserId);
        return OkResponse<object?>(null, "KYC document removed");
    }

    // ── Bank ────────────────────────────────────────────────────

    [HttpPost("{id:guid}/bank")]
    public async Task<ActionResult<ApiResponse<TransporterBankDto>>> AddBank(
        Guid id, [FromBody] AddBankAccountDto dto)
    {
        var result = await _transporterService.AddBankAccountAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Bank account added");
    }

    [HttpPut("{id:guid}/bank/{bankId:guid}")]
    public async Task<ActionResult<ApiResponse<TransporterBankDto>>> UpdateBank(
        Guid id, Guid bankId, [FromBody] UpdateBankAccountDto dto)
    {
        var result = await _transporterService.UpdateBankAccountAsync(
            id, bankId, dto, CurrentUserId);
        return OkResponse(result, "Bank account updated");
    }

    [HttpDelete("{id:guid}/bank/{bankId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteBank(Guid id, Guid bankId)
    {
        await _transporterService.DeleteBankAccountAsync(id, bankId, CurrentUserId);
        return OkResponse<object?>(null, "Bank account removed");
    }

    // ── Notifications ───────────────────────────────────────────

    /// <summary>Get notification preferences for a transporter.</summary>
    [HttpGet("{id:guid}/notifications")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransporterNotificationDto>>>> GetNotifications(Guid id)
    {
        var result = await _transporterService.GetNotificationsAsync(id);
        return OkResponse(result);
    }

    /// <summary>Add a notification preference.</summary>
    [HttpPost("{id:guid}/notifications")]
    public async Task<ActionResult<ApiResponse<TransporterNotificationDto>>> AddNotification(
        Guid id, [FromBody] CreateNotificationDto dto)
    {
        var result = await _transporterService.AddNotificationAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Notification preference added");
    }

    /// <summary>Update a notification preference.</summary>
    [HttpPut("{id:guid}/notifications/{notificationId:guid}")]
    public async Task<ActionResult<ApiResponse<TransporterNotificationDto>>> UpdateNotification(
        Guid id, Guid notificationId, [FromBody] UpdateNotificationDto dto)
    {
        var result = await _transporterService.UpdateNotificationAsync(id, notificationId, dto, CurrentUserId);
        return OkResponse(result, "Notification preference updated");
    }

    /// <summary>Delete a notification preference.</summary>
    [HttpDelete("{id:guid}/notifications/{notificationId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteNotification(Guid id, Guid notificationId)
    {
        await _transporterService.DeleteNotificationAsync(id, notificationId, CurrentUserId);
        return OkResponse<object?>(null, "Notification preference removed");
    }
}
