using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Maintenance Work Order DTOs
// ═══════════════════════════════════════════════════════════════

public class MaintenanceWorkOrderDto
{
    public Guid Id { get; set; }
    public string WorkOrderNumber { get; set; } = null!;
    public Guid FleetVehicleId { get; set; }
    public string? VehicleRegistration { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceStatus Status { get; set; }
    public string Description { get; set; } = null!;
    public string? ReportedIssue { get; set; }
    public string? DiagnosticNotes { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? ServiceProviderName { get; set; }
    public string? ServiceProviderContact { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public decimal? OdometerAtService { get; set; }
    public decimal? NextServiceOdometer { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public string? CompletionNotes { get; set; }
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public ICollection<MaintenancePartDto> Parts { get; set; } = new List<MaintenancePartDto>();
    public ICollection<MaintenanceDocumentDto> Documents { get; set; } = new List<MaintenanceDocumentDto>();
}

public class MaintenanceWorkOrderListDto
{
    public Guid Id { get; set; }
    public string WorkOrderNumber { get; set; } = null!;
    public Guid FleetVehicleId { get; set; }
    public string? VehicleRegistration { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceStatus Status { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
}

public class CreateMaintenanceWorkOrderDto
{
    public Guid FleetVehicleId { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public MaintenancePriority Priority { get; set; }
    public string Description { get; set; } = null!;
    public string? ReportedIssue { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int? EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? ServiceProviderName { get; set; }
    public string? ServiceProviderContact { get; set; }
    public decimal? OdometerAtService { get; set; }
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = null!;
}

public class UpdateMaintenanceWorkOrderDto
{
    public MaintenanceType? MaintenanceType { get; set; }
    public MaintenancePriority? Priority { get; set; }
    public MaintenanceStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? DiagnosticNotes { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartedDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string? ServiceProviderName { get; set; }
    public string? ServiceProviderContact { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public decimal? OdometerAtService { get; set; }
    public decimal? NextServiceOdometer { get; set; }
    public DateTime? NextServiceDate { get; set; }
}

public class CompleteMaintenanceDto
{
    public DateTime CompletedDate { get; set; }
    public int ActualHours { get; set; }
    public decimal ActualCost { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string? CompletionNotes { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public decimal? NextServiceOdometer { get; set; }
    public DateTime? NextServiceDate { get; set; }
}

public class MaintenanceFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? FleetVehicleId { get; set; }
    public MaintenanceType? MaintenanceType { get; set; }
    public MaintenanceStatus? Status { get; set; }
    public MaintenancePriority? Priority { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? BranchId { get; set; }
    public string? CountryCode { get; set; }
}

// ── Maintenance Parts ───────────────────────────────────────────

public class MaintenancePartDto
{
    public Guid Id { get; set; }
    public Guid MaintenanceWorkOrderId { get; set; }
    public string PartName { get; set; } = null!;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? Supplier { get; set; }
    public string? WarrantyInfo { get; set; }
}

public class AddMaintenancePartDto
{
    public string PartName { get; set; } = null!;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? Supplier { get; set; }
    public string? WarrantyInfo { get; set; }
}

// ── Maintenance Documents ───────────────────────────────────────

public class MaintenanceDocumentDto
{
    public Guid Id { get; set; }
    public Guid MaintenanceWorkOrderId { get; set; }
    public MaintenanceDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateMaintenanceDocumentDto
{
    public MaintenanceDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
}
