using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Maintenance work order for a fleet vehicle.
/// </summary>
public class MaintenanceWorkOrder : BaseEntity
{
    public string WorkOrderNumber { get; set; } = null!;
    public Guid FleetVehicleId { get; set; }
    
    // ── Type & Priority ─────────────────────────────────────────
    public MaintenanceType MaintenanceType { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceStatus Status { get; set; }
    
    // ── Description ─────────────────────────────────────────────
    public string Description { get; set; } = null!;
    public string? ReportedIssue { get; set; }
    public string? DiagnosticNotes { get; set; }
    
    // ── Scheduling ──────────────────────────────────────────────
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    
    // ── Costs ───────────────────────────────────────────────────
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    
    // ── Service Provider ────────────────────────────────────────
    public string? ServiceProviderName { get; set; }
    public string? ServiceProviderContact { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    
    // ── Odometer ────────────────────────────────────────────────
    public decimal? OdometerAtService { get; set; }
    public decimal? NextServiceOdometer { get; set; }
    public DateTime? NextServiceDate { get; set; }
    
    // ── Completion ──────────────────────────────────────────────
    public string? CompletionNotes { get; set; }
    public Guid? CompletedBy { get; set; }
    
    // ── Scoping ─────────────────────────────────────────────────
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = null!;
    
    // ── Navigation ──────────────────────────────────────────────
    public FleetVehicle FleetVehicle { get; set; } = null!;
    public ICollection<MaintenancePart> Parts { get; set; } = new List<MaintenancePart>();
    public ICollection<MaintenanceDocument> Documents { get; set; } = new List<MaintenanceDocument>();
}

/// <summary>
/// Parts used in a maintenance work order.
/// </summary>
public class MaintenancePart : BaseEntity
{
    public Guid MaintenanceWorkOrderId { get; set; }
    public string PartName { get; set; } = null!;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Supplier { get; set; }
    public string? WarrantyInfo { get; set; }
    
    // ── Navigation ──────────────────────────────────────────────
    public MaintenanceWorkOrder MaintenanceWorkOrder { get; set; } = null!;
}
