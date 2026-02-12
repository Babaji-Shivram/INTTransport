namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Per-vehicle daily expense record — 15 fixed expense categories.
/// Legacy: TransDailyExpense.aspx → SP: AddVehicleDailyExpense
/// Business rule: Date-limited to 30-day rolling window.
/// </summary>
public class VehicleDailyExpense : BaseEntity
{
    public Guid FleetVehicleId { get; set; }
    public DateTime ExpenseDate { get; set; }

    // ── 15 Fixed Expense Categories (matching legacy TransDailyExpense) ──
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

    // ── Computed / Metadata ─────────────────────────────────────
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? Remarks { get; set; }
    public Guid? TransportRequestId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public FleetVehicle FleetVehicle { get; set; } = null!;
    public TransportRequest? TransportRequest { get; set; }
}
