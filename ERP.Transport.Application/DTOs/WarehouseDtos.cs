namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Transit Warehouse DTOs (Legacy: InTransitWarehouse.aspx)
// ═══════════════════════════════════════════════════════════════

public class TransitWarehouseDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public Guid? TransportVehicleId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public string? WarehouseAddress { get; set; }
    public string? WarehouseCity { get; set; }
    public string? WarehouseState { get; set; }
    public string? WarehousePincode { get; set; }
    public DateTime ArrivalDate { get; set; }
    public string? ArrivalRemarks { get; set; }
    public string? ReceivedBy { get; set; }
    public DateTime? DepartureDate { get; set; }
    public string? DepartureRemarks { get; set; }
    public string? DispatchedBy { get; set; }
    public string? ContainerId { get; set; }
    public string? ContainerSealNumber { get; set; }
    public bool IsDispatched { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ArriveAtWarehouseDto
{
    public string WarehouseName { get; set; } = null!;
    public string? WarehouseAddress { get; set; }
    public string? WarehouseCity { get; set; }
    public string? WarehouseState { get; set; }
    public string? WarehousePincode { get; set; }
    public string? ReceivedBy { get; set; }
    public string? ArrivalRemarks { get; set; }
    public Guid? TransportVehicleId { get; set; }
    public string? ContainerId { get; set; }
    public string? ContainerSealNumber { get; set; }
}

public class DispatchFromWarehouseDto
{
    public string? DispatchedBy { get; set; }
    public string? DepartureRemarks { get; set; }
}
