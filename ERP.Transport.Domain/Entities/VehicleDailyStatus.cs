using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Daily snapshot of a fleet vehicle's operational status.
/// Tracks availability, odometer readings, and trip assignments.
/// </summary>
public class VehicleDailyStatus : BaseEntity
{
    public Guid FleetVehicleId { get; set; }
    public DateTime Date { get; set; }
    public FleetVehicleStatus Status { get; set; }
    public Guid? CurrentJobId { get; set; }
    public decimal? OdometerKm { get; set; }
    public string? Remarks { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public FleetVehicle FleetVehicle { get; set; } = null!;
}
