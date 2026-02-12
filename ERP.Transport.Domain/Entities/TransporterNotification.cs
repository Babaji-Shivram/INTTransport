using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Notification preference for a transporter — email, SMS, WhatsApp.
/// </summary>
public class TransporterNotification : BaseEntity
{
    public Guid TransporterId { get; set; }

    public NotificationType Type { get; set; }
    public string? Destination { get; set; }
    public bool IsEnabled { get; set; } = true;

    // ── Navigation ──────────────────────────────────────────────
    public Transporter Transporter { get; set; } = null!;
}
