namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Travel log entry for a fleet vehicle — tracks each trip/usage.
/// </summary>
public class VehicleTravelLog : BaseEntity
{
    public Guid FleetVehicleId { get; set; }
    public Guid? TransportRequestId { get; set; }
    
    // ── Trip Details ────────────────────────────────────────────
    public DateTime TripDate { get; set; }
    public string? TripPurpose { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    
    // ── Odometer ────────────────────────────────────────────────
    public decimal StartOdometerKm { get; set; }
    public decimal EndOdometerKm { get; set; }
    public decimal DistanceKm { get; set; }
    
    // ── Time ────────────────────────────────────────────────────
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int? TripDurationMinutes { get; set; }
    
    // ── Driver ──────────────────────────────────────────────────
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    
    // ── Fuel ────────────────────────────────────────────────────
    public decimal? FuelConsumedLitres { get; set; }
    public decimal? FuelCost { get; set; }
    public string? FuelReceiptUrl { get; set; }
    
    // ── Toll / Other Expenses ───────────────────────────────────
    public decimal? TollCharges { get; set; }
    public decimal? ParkingCharges { get; set; }
    public decimal? OtherExpenses { get; set; }
    public string? ExpenseNotes { get; set; }
    
    // ── Remarks ─────────────────────────────────────────────────
    public string? Remarks { get; set; }
    
    // ── Navigation ──────────────────────────────────────────────
    public FleetVehicle FleetVehicle { get; set; } = null!;
    public TransportRequest? TransportRequest { get; set; }
}
