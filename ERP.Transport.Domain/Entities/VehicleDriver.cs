namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Driver assigned to a fleet vehicle (many-to-many over time).
/// </summary>
public class VehicleDriver : BaseEntity
{
    public Guid FleetVehicleId { get; set; }
    public string DriverName { get; set; } = null!;
    public string? LicenseNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime AssignedDate { get; set; }
    public DateTime? UnassignedDate { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public FleetVehicle FleetVehicle { get; set; } = null!;
}
