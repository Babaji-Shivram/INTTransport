namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Bank account details for a transporter — used for fund requests and payments.
/// </summary>
public class TransporterBank : BaseEntity
{
    public Guid TransporterId { get; set; }

    public string BankName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? IFSCCode { get; set; }
    public string? BranchName { get; set; }
    public string? AccountHolderName { get; set; }
    public bool IsPrimary { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public Transporter Transporter { get; set; } = null!;
}
