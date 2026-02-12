namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Transit warehouse record — intermediate stage between movement and delivery.
/// Legacy: InTransitWarehouse.aspx → SP: insJobTransitHistory (AddTransitWarehouse)
/// </summary>
public class TransitWarehouse : BaseEntity
{
    public Guid TransportRequestId { get; set; }
    public Guid? TransportVehicleId { get; set; }

    // ── Warehouse Details ───────────────────────────────────────
    public string WarehouseName { get; set; } = null!;
    public string? WarehouseAddress { get; set; }
    public string? WarehouseCity { get; set; }
    public string? WarehouseState { get; set; }
    public string? WarehousePincode { get; set; }

    // ── Arrival ─────────────────────────────────────────────────
    public DateTime ArrivalDate { get; set; }
    public string? ArrivalRemarks { get; set; }
    public string? ReceivedBy { get; set; }

    // ── Departure (to delivery) ─────────────────────────────────
    public DateTime? DepartureDate { get; set; }
    public string? DepartureRemarks { get; set; }
    public string? DispatchedBy { get; set; }

    // ── Container tracking (Sea mode) ───────────────────────────
    public string? ContainerId { get; set; }
    public string? ContainerSealNumber { get; set; }

    // ── Status ──────────────────────────────────────────────────
    public bool IsDispatched { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
    public TransportVehicle? TransportVehicle { get; set; }
}
