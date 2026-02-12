namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Transport job lifecycle status — maps to workflow steps.
/// </summary>
public enum TransportStatus
{
    RequestCreated = 0,
    RequestReceived = 1,
    VehicleAssigned = 2,
    RateEntered = 3,
    RateApproval = 4,
    InTransit = 5,
    InWarehouse = 6,
    Delivered = 7,
    Cleared = 8,
    Cancelled = 9
}
