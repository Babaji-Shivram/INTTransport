namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Vehicle Travel Log DTOs
// ═══════════════════════════════════════════════════════════════

public class VehicleTravelLogDto
{
    public Guid Id { get; set; }
    public Guid FleetVehicleId { get; set; }
    public string? VehicleRegistration { get; set; }
    public Guid? TransportRequestId { get; set; }
    public string? TransportRequestNumber { get; set; }
    public DateTime TripDate { get; set; }
    public string? TripPurpose { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public decimal StartOdometerKm { get; set; }
    public decimal EndOdometerKm { get; set; }
    public decimal DistanceKm { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int? TripDurationMinutes { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public decimal? FuelConsumedLitres { get; set; }
    public decimal? FuelCost { get; set; }
    public string? FuelReceiptUrl { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? ParkingCharges { get; set; }
    public decimal? OtherExpenses { get; set; }
    public string? ExpenseNotes { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateVehicleTravelLogDto
{
    public Guid FleetVehicleId { get; set; }
    public Guid? TransportRequestId { get; set; }
    public DateTime TripDate { get; set; }
    public string? TripPurpose { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public decimal StartOdometerKm { get; set; }
    public decimal EndOdometerKm { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public decimal? FuelConsumedLitres { get; set; }
    public decimal? FuelCost { get; set; }
    public string? FuelReceiptUrl { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? ParkingCharges { get; set; }
    public decimal? OtherExpenses { get; set; }
    public string? ExpenseNotes { get; set; }
    public string? Remarks { get; set; }
}

public class TravelLogFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? FleetVehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class VehicleUsageSummaryDto
{
    public Guid FleetVehicleId { get; set; }
    public string VehicleRegistration { get; set; } = null!;
    public int TotalTrips { get; set; }
    public decimal TotalDistanceKm { get; set; }
    public decimal TotalFuelLitres { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal TotalTollCharges { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal AverageKmPerTrip { get; set; }
    public decimal? FuelEfficiencyKmPerLitre { get; set; }
}

/// <summary>Complete a trip - record end details</summary>
public class CompleteTripDto
{
    public decimal EndOdometerKm { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal? FuelConsumedLitres { get; set; }
    public decimal? FuelCost { get; set; }
    public string? FuelReceiptUrl { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? ParkingCharges { get; set; }
    public decimal? OtherExpenses { get; set; }
    public string? ExpenseNotes { get; set; }
    public string? Remarks { get; set; }
}
