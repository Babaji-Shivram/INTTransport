namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Type of cargo being transported.
/// </summary>
public enum CargoType
{
    General = 0,
    Dangerous = 1,
    Perishable = 2,
    ODC = 3   // Over Dimensional Cargo
}
