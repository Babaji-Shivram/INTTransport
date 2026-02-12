using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Delivery record for a transport job — captures POD, challan, receiver info.
/// One-to-one with TransportRequest (a job has one delivery).
/// </summary>
public class TransportDelivery : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    public DateTime DeliveryDate { get; set; }
    public string ReceivedBy { get; set; } = null!;

    // ── POD ─────────────────────────────────────────────────────
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }

    // ── Challan ─────────────────────────────────────────────────
    public string? ChallanNumber { get; set; }
    public string? ChallanDocumentUrl { get; set; }

    // ── LR Copy ─────────────────────────────────────────────────
    public string? LRCopyUrl { get; set; }

    // ── E-Way Bill ──────────────────────────────────────────────
    public string? EWayBillNumber { get; set; }

    // ── Indian Compliance Fields (Legacy: WarehouseDelivery.aspx) ─
    public string? LRNo { get; set; }
    public DateTime? LRDate { get; set; }
    public string? RoadPermitNo { get; set; }
    public DateTime? RoadPermitDate { get; set; }
    public string? NFormNo { get; set; }
    public DateTime? NFormDate { get; set; }
    public string? SFormNo { get; set; }
    public DateTime? SFormDate { get; set; }
    public string? OctroiReceiptNo { get; set; }
    public decimal? OctroiAmount { get; set; }
    public string? BabajiChallanNo { get; set; }
    public DateTime? BabajiChallanDate { get; set; }
    public DateTime? EmptyContReturnDate { get; set; }
    public string? DeliveryPoint { get; set; }
    public string? ContainerId { get; set; }
    public bool IsOwnFleetDelivery { get; set; }

    // ── Document Uploads ────────────────────────────────────────
    public string? DamageDocumentUrl { get; set; }

    // ── Delivery Status ─────────────────────────────────────────
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? DamageNotes { get; set; }
    public string? ShortDeliveryNotes { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
}
