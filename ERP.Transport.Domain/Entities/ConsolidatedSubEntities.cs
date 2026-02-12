using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Vehicle assigned to a consolidated trip (shared across multiple jobs).
/// </summary>
public class ConsolidatedVehicle : BaseEntity
{
    public Guid ConsolidatedTripId { get; set; }

    // ── Transporter ─────────────────────────────────────────────
    public Guid TransporterId { get; set; }
    public string? TransporterName { get; set; }

    // ── Vehicle ─────────────────────────────────────────────────
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }

    // ── Driver ──────────────────────────────────────────────────
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }

    // ── Rate ────────────────────────────────────────────────────
    public decimal? FreightRate { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? OtherCharges { get; set; }
    public decimal? TotalRate { get; set; }
    public string CurrencyCode { get; set; } = "INR";

    // ── LR ──────────────────────────────────────────────────────
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
    public string? MemoCopyUrl { get; set; }

    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────────────────────
    public ConsolidatedTrip ConsolidatedTrip { get; set; } = null!;
    public Transporter Transporter { get; set; } = null!;
}

/// <summary>
/// Expense shared across a consolidated trip (e.g. fuel, toll for the entire run).
/// </summary>
public class ConsolidatedExpense : BaseEntity
{
    public Guid ConsolidatedTripId { get; set; }

    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Remarks { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public ConsolidatedTrip ConsolidatedTrip { get; set; } = null!;
}

/// <summary>
/// Per-stop delivery tracking within a consolidated trip.
/// Each stop corresponds to a child job's destination.
/// </summary>
public class ConsolidatedStopDelivery : BaseEntity
{
    public Guid ConsolidatedTripId { get; set; }
    public Guid TransportRequestId { get; set; }

    /// <summary>Sequence of this stop in the route (1, 2, 3, ...)</summary>
    public int StopSequence { get; set; }

    // ── Location ────────────────────────────────────────────────
    public string? LocationName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Pincode { get; set; }

    // ── Delivery ────────────────────────────────────────────────
    public DateTime? EstimatedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? ReceivedBy { get; set; }
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }
    public string? Remarks { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public ConsolidatedTrip ConsolidatedTrip { get; set; } = null!;
    public TransportRequest TransportRequest { get; set; } = null!;
}
