namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Movement tracking milestones for a transport job.
/// </summary>
public enum MovementMilestone
{
    Dispatched = 0,
    InTransit = 1,
    AtCheckpoint = 2,
    NearDestination = 3,
    ReachedDestination = 4,
    Unloading = 5,
    Completed = 6
}
