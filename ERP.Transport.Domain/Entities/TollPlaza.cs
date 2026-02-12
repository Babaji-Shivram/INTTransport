namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Toll plaza master data from ULIP TOLL/01 API.
/// </summary>
public class TollPlaza : BaseEntity
{
    public string PlazaCode { get; set; } = null!;
    public string? PlazaName { get; set; }
    public string? State { get; set; }
    public string? Highway { get; set; }
    public string? Direction { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // ── Rate Info ───────────────────────────────────────────────
    public decimal? SingleJourneyRate { get; set; }
    public decimal? ReturnJourneyRate { get; set; }
    public decimal? MonthlyPassRate { get; set; }
    public string? VehicleClassApplicable { get; set; }

    // ── Cache ───────────────────────────────────────────────────
    public DateTime LastFetchedFromUlip { get; set; }
    public string? RawApiResponse { get; set; }
}
