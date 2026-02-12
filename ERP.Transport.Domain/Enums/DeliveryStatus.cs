namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Delivery completion status.
/// </summary>
public enum DeliveryStatus
{
    Pending = -1,
    Full = 0,
    Partial = 1,
    Damaged = 2
}
