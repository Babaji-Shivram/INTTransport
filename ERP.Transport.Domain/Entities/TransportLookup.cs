using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Generic lookup / master table for configurable code-value pairs.
/// Replaces hard-coded enums with DB-backed values for UI flexibility.
/// Categories: VehicleType, ExpenseCategory, VehicleStatus, MaintenanceCategory.
/// </summary>
public class TransportLookup : BaseEntity
{
    public LookupCategory Category { get; set; }

    /// <summary>Short code used in logic (e.g. "TRAILER_40FT", "FUEL_TYPE1").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Display name shown in UI.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Optional longer description.</summary>
    public string? Description { get; set; }

    /// <summary>Sort order for dropdowns.</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Country scoping — null means global.</summary>
    public string? CountryCode { get; set; }
}
