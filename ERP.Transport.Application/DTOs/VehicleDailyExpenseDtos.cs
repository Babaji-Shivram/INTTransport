namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Vehicle Daily Expense DTOs (Legacy: TransDailyExpense.aspx)
// ═══════════════════════════════════════════════════════════════

public class VehicleDailyExpenseDto
{
    public Guid Id { get; set; }
    public Guid FleetVehicleId { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime ExpenseDate { get; set; }

    // ── 15 Categories ───────────────────────────────────────────
    public decimal Fuel { get; set; }
    public decimal FuelLitres { get; set; }
    public decimal Fuel2 { get; set; }
    public decimal Fuel2Litres { get; set; }
    public decimal TollCharges { get; set; }
    public decimal Fines { get; set; }
    public decimal Xerox { get; set; }
    public decimal VaraiUnloading { get; set; }
    public decimal EmptyContainer { get; set; }
    public decimal Parking { get; set; }
    public decimal Garage { get; set; }
    public decimal Bhatta { get; set; }
    public decimal ODCOverweight { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal DamageContainer { get; set; }

    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? Remarks { get; set; }
    public Guid? TransportRequestId { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateVehicleDailyExpenseDto
{
    public DateTime ExpenseDate { get; set; }
    public decimal Fuel { get; set; }
    public decimal FuelLitres { get; set; }
    public decimal Fuel2 { get; set; }
    public decimal Fuel2Litres { get; set; }
    public decimal TollCharges { get; set; }
    public decimal Fines { get; set; }
    public decimal Xerox { get; set; }
    public decimal VaraiUnloading { get; set; }
    public decimal EmptyContainer { get; set; }
    public decimal Parking { get; set; }
    public decimal Garage { get; set; }
    public decimal Bhatta { get; set; }
    public decimal ODCOverweight { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal DamageContainer { get; set; }
    public string? Remarks { get; set; }
    public Guid? TransportRequestId { get; set; }
}

public class UpdateVehicleDailyExpenseDto
{
    public decimal? Fuel { get; set; }
    public decimal? FuelLitres { get; set; }
    public decimal? Fuel2 { get; set; }
    public decimal? Fuel2Litres { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? Fines { get; set; }
    public decimal? Xerox { get; set; }
    public decimal? VaraiUnloading { get; set; }
    public decimal? EmptyContainer { get; set; }
    public decimal? Parking { get; set; }
    public decimal? Garage { get; set; }
    public decimal? Bhatta { get; set; }
    public decimal? ODCOverweight { get; set; }
    public decimal? OtherCharges { get; set; }
    public decimal? DamageContainer { get; set; }
    public string? Remarks { get; set; }
}

public class DailyExpenseFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? FleetVehicleId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? TransportRequestId { get; set; }
}

public class DailyExpenseAggregateDto
{
    public Guid FleetVehicleId { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int DayCount { get; set; }
    public decimal TotalFuel { get; set; }
    public decimal TotalToll { get; set; }
    public decimal TotalFines { get; set; }
    public decimal TotalParking { get; set; }
    public decimal TotalGarage { get; set; }
    public decimal TotalOther { get; set; }
    public decimal GrandTotal { get; set; }
    public string CurrencyCode { get; set; } = "INR";
}
