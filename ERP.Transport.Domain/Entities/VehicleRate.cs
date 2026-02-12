namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Rate (buying rate) for a specific vehicle assignment.
/// Records what we pay the transporter — base freight + additional charges.
/// </summary>
public class VehicleRate : BaseEntity
{
    public Guid TransportVehicleId { get; set; }

    // ── Rate Components ─────────────────────────────────────────
    public decimal FreightRate { get; set; }
    public decimal DetentionCharges { get; set; }
    public decimal VaraiCharges { get; set; }
    public decimal EmptyContainerReturn { get; set; }
    public decimal TollCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal TotalRate { get; set; }

    /// <summary>Currency code e.g. INR, AED, USD</summary>
    public string CurrencyCode { get; set; } = "INR";

    // ── Approval ────────────────────────────────────────────────
    // ── Extra Rate Fields (Legacy: VehiclePlace.aspx) ────────
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }

    public bool IsApproved { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? ApprovalRemarks { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public TransportVehicle TransportVehicle { get; set; } = null!;
}
