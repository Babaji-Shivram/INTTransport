namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Client for sending callbacks to Freight MS when transport job status changes.
/// </summary>
public interface IFreightClient
{
    Task SendCallbackAsync(Guid freightJobId, TransportCallbackPayload payload, CancellationToken ct = default);
}

public class TransportCallbackPayload
{
    public string Event { get; set; } = null!;            // "TRANSPORT_COMPLETED", "TRANSPORT_DELIVERED", "TRANSPORT_STATUS_CHANGED"
    public string? TransportJobNumber { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }
}
