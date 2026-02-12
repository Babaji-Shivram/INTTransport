namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Daily operational status of a fleet vehicle.
/// </summary>
public enum FleetVehicleStatus
{
    Available = 0,
    OnTrip = 1,
    UnderMaintenance = 2,
    OutOfService = 3,
    Reserved = 4
}
