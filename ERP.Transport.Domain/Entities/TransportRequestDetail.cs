namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Dynamic/extra key-value fields per transport request.
/// Stores additional data from workflow field definitions or source-specific fields.
/// </summary>
public class TransportRequestDetail : BaseEntity
{
    public Guid TransportRequestId { get; set; }

    /// <summary>Field key e.g. "TR.HSNCode", "TR.ConsigneeGSTIN"</summary>
    public string FieldKey { get; set; } = null!;

    /// <summary>Display label e.g. "HSN Code"</summary>
    public string? FieldLabel { get; set; }

    /// <summary>Stored value (always string, parsed by consumer)</summary>
    public string? FieldValue { get; set; }

    /// <summary>Data type hint: string, number, date, bool, enum, lookup, document</summary>
    public string? DataType { get; set; }

    /// <summary>Workflow step this field belongs to (nullable)</summary>
    public Guid? WorkflowStepId { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportRequest TransportRequest { get; set; } = null!;
}
