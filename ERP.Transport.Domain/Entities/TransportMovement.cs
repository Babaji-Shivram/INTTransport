using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Location tracking entry for a transport job.
/// Each entry records a movement milestone / checkpoint update.
/// </summary>
public class TransportMovement : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    public MovementMilestone Milestone { get; set; }
    public string? LocationName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Remarks { get; set; }

    /// <summary>Vehicle this movement is for (when multiple vehicles)</summary>
    public Guid? TransportVehicleId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
}
