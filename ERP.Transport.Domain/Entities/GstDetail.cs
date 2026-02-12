namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Cached GST Details from CharteredInfo master/gstin API.
/// One record per GSTIN lookup.
/// </summary>
public class GstDetail : BaseEntity
{
    // ── Lookup Key ──────────────────────────────────────────────
    public string Gstin { get; set; } = null!;

    // ── Business Info ───────────────────────────────────────────
    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public string? BusinessType { get; set; }
    public string? GstinStatus { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? CancellationDate { get; set; }

    // ── Address ─────────────────────────────────────────────────
    public string? Address { get; set; }
    public string? StateCode { get; set; }
    public string? StateName { get; set; }
    public string? Pincode { get; set; }

    // ── Nature of Business ──────────────────────────────────────
    public string? NatureOfBusiness { get; set; }
    public string? ConstitutionOfBusiness { get; set; }
    public string? TaxpayerType { get; set; }

    // ── E-Invoice / E-Way Bill Eligibility ──────────────────────
    public bool? IsEInvoiceApplicable { get; set; }
    public bool? IsEWayBillApplicable { get; set; }

    // ── Filing Status ───────────────────────────────────────────
    public string? LastFiledReturn { get; set; }
    public string? LastFiledReturnDate { get; set; }

    // ── Cache Control ───────────────────────────────────────────
    public DateTime LastFetchedFromApi { get; set; }
    public string? RawApiResponse { get; set; }
}
