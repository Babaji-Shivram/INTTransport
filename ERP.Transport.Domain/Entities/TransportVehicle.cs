using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Vehicle assignment for a transport job.
/// One job can have multiple vehicles (e.g. 3 trucks for a large shipment).
/// </summary>
public class TransportVehicle : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    // ── Transporter ─────────────────────────────────────────────
    public Guid TransporterId { get; set; }
    public string? TransporterName { get; set; }

    // ── Vehicle ─────────────────────────────────────────────────
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }

    // ── Driver ──────────────────────────────────────────────────
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }

    // ── LR (Lorry Receipt) ──────────────────────────────────────
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
    public string? MemoCopyUrl { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
    public Transporter Transporter { get; set; } = null!;
    public VehicleRate? Rate { get; set; }
    public VehicleFundRequest? FundRequest { get; set; }
}
