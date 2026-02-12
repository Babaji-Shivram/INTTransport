using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Groups multiple TransportRequests sharing the same destination/route
/// into a single trip for efficiency.
/// </summary>
public class ConsolidatedTrip : BaseEntity
{
    public string ReferenceNumber { get; set; } = null!;

    // ── Shared destination ──────────────────────────────────────
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }

    // ── Shared vehicle (optional — assigned after consolidation) ─
    public Guid? SharedVehicleId { get; set; }
    public string? SharedVehicleNumber { get; set; }

    // ── Metadata ────────────────────────────────────────────────
    public ConsolidationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public int JobCount { get; set; }
    public string CountryCode { get; set; } = null!;
    public Guid BranchId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public ICollection<TransportRequest> Jobs { get; set; } = new List<TransportRequest>();
    public ICollection<ConsolidatedVehicle> Vehicles { get; set; } = new List<ConsolidatedVehicle>();
    public ICollection<ConsolidatedExpense> Expenses { get; set; } = new List<ConsolidatedExpense>();
    public ICollection<ConsolidatedStopDelivery> StopDeliveries { get; set; } = new List<ConsolidatedStopDelivery>();
}
