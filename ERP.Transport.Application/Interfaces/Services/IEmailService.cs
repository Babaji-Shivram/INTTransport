namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Low-level email sending abstraction.
/// </summary>
public interface IEmailService
{
    /// <summary>Send an email (optionally with attachment).</summary>
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}

/// <summary>
/// High-level transport notification service — 
/// orchestrates sending event-driven email notifications to customers/transporters.
/// </summary>
public interface ITransportNotificationService
{
    /// <summary>Send a daily status update email for an in-transit job.</summary>
    Task SendDailyStatusEmailAsync(Guid transportRequestId, CancellationToken ct = default);

    /// <summary>Send delivery confirmation email with POD details.</summary>
    Task SendDeliveryNotificationAsync(Guid transportRequestId, CancellationToken ct = default);

    /// <summary>Send ELR PDF to customer and transporter.</summary>
    Task SendElrEmailAsync(Guid transportRequestId, CancellationToken ct = default);

    /// <summary>Send job assignment alert to transporter.</summary>
    Task SendTransporterAssignmentAsync(Guid transportRequestId, Guid transportVehicleId, CancellationToken ct = default);
}

/// <summary>
/// Email message model.
/// </summary>
public class EmailMessage
{
    public string To { get; set; } = null!;
    public string? Cc { get; set; }
    public string Subject { get; set; } = null!;
    public string HtmlBody { get; set; } = null!;

    /// <summary>Optional attachment (e.g. ELR PDF).</summary>
    public EmailAttachment? Attachment { get; set; }
}

public class EmailAttachment
{
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string ContentType { get; set; } = "application/pdf";
}
