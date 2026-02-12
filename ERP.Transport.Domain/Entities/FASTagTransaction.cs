namespace ERP.Transport.Domain.Entities;

/// <summary>
/// FASTag toll transaction record from ULIP FASTAG/01 API.
/// Multiple transactions per vehicle number.
/// </summary>
public class FASTagTransaction : BaseEntity
{
    // ── Lookup Key ──────────────────────────────────────────────
    public string VehicleNumber { get; set; } = null!;

    // ── Transaction Details ─────────────────────────────────────
    public string? TagId { get; set; }
    public string? TollPlazaName { get; set; }
    public string? TollPlazaId { get; set; }
    public string? LaneDirection { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public string? TransactionId { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal? TransactionAmount { get; set; }

    // ── Location ────────────────────────────────────────────────
    public string? PlazaState { get; set; }
    public string? PlazaHighway { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // ── Vehicle Info at toll ────────────────────────────────────
    public string? VehicleClassAtToll { get; set; }

    // ── Link to Transport Job (optional) ────────────────────────
    public Guid? TransportRequestId { get; set; }
    public TransportRequest? TransportRequest { get; set; }

    // ── Cache ───────────────────────────────────────────────────
    public DateTime FetchedFromUlip { get; set; }
    public string? RawApiResponse { get; set; }
}
