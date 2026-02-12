namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Trip expense categories — 15 types as per PRD.
/// </summary>
public enum ExpenseCategory
{
    FuelType1 = 0,          // Primary fuel (Diesel)
    FuelType2 = 1,          // CNG / AdBlue
    FuelLitres = 2,         // Fuel quantity
    TollCharges = 3,
    Fines = 4,
    Documentation = 5,      // Xerox / documentation
    VaraiUnloading = 6,     // Loading/unloading labour
    EmptyContainer = 7,
    Parking = 8,
    Garage = 9,             // Repair/mechanic during trip
    DriverAllowance = 10,   // Bhatta — food & stay
    ODCOverweight = 11,     // Over-dimensional cargo charges
    DamageContainer = 12,
    TempoUnion = 13,        // Local union/association charges
    Other = 99
}
