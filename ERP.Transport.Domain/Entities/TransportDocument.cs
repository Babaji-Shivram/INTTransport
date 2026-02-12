using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Document uploaded for a transport job — POD, LR, challan, memo, etc.
/// </summary>
public class TransportDocument : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }

    /// <summary>Optional link to a specific vehicle</summary>
    public Guid? TransportVehicleId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
}
