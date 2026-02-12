using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Document attached to a maintenance work order — service reports, photos, invoices, warranties.
/// </summary>
public class MaintenanceDocument : BaseEntity
{
    public Guid MaintenanceWorkOrderId { get; set; }

    public MaintenanceDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public MaintenanceWorkOrder MaintenanceWorkOrder { get; set; } = null!;
}
