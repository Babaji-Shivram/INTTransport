namespace ERP.Transport.Domain.Entities;

/// <summary>
/// KYC document for a transporter — PAN card, GST cert, TDS declaration, etc.
/// </summary>
public class TransporterKYC : BaseEntity
{
    public Guid TransporterId { get; set; }

    public string DocumentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsVerified { get; set; }
    public string? VerifiedByName { get; set; }
    public DateTime? VerifiedDate { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public Transporter Transporter { get; set; } = null!;
}
