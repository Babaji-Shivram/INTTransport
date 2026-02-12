using ERP.Transport.Application.Interfaces;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Email notifications for transport jobs.
/// </summary>
public class NotificationsController : TransportBaseController
{
    private readonly ITransportNotificationService _notificationService;

    public NotificationsController(ITransportNotificationService notificationService)
        => _notificationService = notificationService;

    /// <summary>Send daily status email for an in-transit job.</summary>
    [HttpPost("{jobId:guid}/daily-status")]
    public async Task<ActionResult<ApiResponse<object?>>> SendDailyStatus(Guid jobId)
    {
        await _notificationService.SendDailyStatusEmailAsync(jobId);
        return OkResponse<object?>(null, "Daily status email sent");
    }

    /// <summary>Send delivery confirmation email.</summary>
    [HttpPost("{jobId:guid}/delivery-confirmation")]
    public async Task<ActionResult<ApiResponse<object?>>> SendDeliveryNotification(Guid jobId)
    {
        await _notificationService.SendDeliveryNotificationAsync(jobId);
        return OkResponse<object?>(null, "Delivery notification sent");
    }

    /// <summary>Send ELR PDF via email to customer and transporter.</summary>
    [HttpPost("{jobId:guid}/elr")]
    public async Task<ActionResult<ApiResponse<object?>>> SendElrEmail(Guid jobId)
    {
        await _notificationService.SendElrEmailAsync(jobId);
        return OkResponse<object?>(null, "ELR email sent to customer and transporter");
    }

    /// <summary>Send assignment notification to transporter.</summary>
    [HttpPost("{jobId:guid}/vehicles/{vehicleId:guid}/assignment")]
    public async Task<ActionResult<ApiResponse<object?>>> SendAssignment(Guid jobId, Guid vehicleId)
    {
        await _notificationService.SendTransporterAssignmentAsync(jobId, vehicleId);
        return OkResponse<object?>(null, "Assignment notification sent to transporter");
    }
}
