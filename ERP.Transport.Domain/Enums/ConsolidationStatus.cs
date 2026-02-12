namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Status of a consolidated trip grouping.
/// </summary>
public enum ConsolidationStatus
{
    Draft = 0,
    Confirmed = 1,
    VehicleAssigned = 2,
    InTransit = 3,
    Completed = 4,
    Cancelled = 5
}
