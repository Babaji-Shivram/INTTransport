using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Direct client for ULIP staging API (www.ulipstaging.dpiit.gov.in).
/// Handles token management, VAHAN, SARATHI, FASTAG, TOLL calls.
/// </summary>
public interface IUlipClient
{
    /// <summary>Login and get/refresh ULIP token.</summary>
    Task<string?> GetTokenAsync(CancellationToken ct = default);

    /// <summary>VAHAN/01 — Vehicle details by registration number.</summary>
    Task<UlipVahanRawResponse?> GetVehicleDetailsAsync(
        string vehicleNumber, CancellationToken ct = default);

    /// <summary>SARATHI/01 — Driver license verification.</summary>
    Task<UlipSarathiRawResponse?> VerifyDriverLicenseAsync(
        string licenseNumber, string dateOfBirth, CancellationToken ct = default);

    /// <summary>FASTAG/01 — FASTag toll transactions by vehicle number.</summary>
    Task<UlipFastagRawResponse?> GetFASTagTransactionsAsync(
        string vehicleNumber, CancellationToken ct = default);

    /// <summary>TOLL/01 — Toll plaza details by plaza ID.</summary>
    Task<UlipTollRawResponse?> GetTollPlazaAsync(
        string plazaId, CancellationToken ct = default);

    /// <summary>Health check — verify ULIP connectivity + token.</summary>
    Task<UlipHealthDto> HealthCheckAsync(CancellationToken ct = default);
}

// ── Raw ULIP API response wrappers ──────────────────────────────
// These map the actual JSON envelope from ULIP.
// The service layer transforms these into domain entities + DTOs.

public class UlipApiEnvelope
{
    public string? Code { get; set; }
    public string? Status { get; set; }
    public object? Response { get; set; }
}

public class UlipVahanRawResponse
{
    public string? Rc_regn_no { get; set; }
    public string? Rc_owner_name { get; set; }
    public string? Rc_f_name { get; set; }
    public string? Rc_present_address { get; set; }
    public string? Rc_permanent_address { get; set; }
    public string? Rc_vh_class_desc { get; set; }
    public string? Rc_vch_catg { get; set; }
    public string? Rc_maker_model { get; set; }
    public string? Rc_maker_desc { get; set; }
    public string? Rc_body_type_desc { get; set; }
    public string? Rc_fuel_desc { get; set; }
    public string? Rc_color { get; set; }
    public string? Rc_manu_month_yr { get; set; }
    public int? Rc_seat_cap { get; set; }
    public decimal? Rc_gvw { get; set; }
    public decimal? Rc_unld_wt { get; set; }
    public int? Rc_no_cyl { get; set; }
    public string? Rc_eng_no { get; set; }
    public string? Rc_chasi_no { get; set; }
    public string? Rc_registered_at { get; set; }
    public string? Rc_registration_state { get; set; }
    public string? Rc_regn_dt { get; set; }
    public string? Rc_fit_upto { get; set; }
    public string? Rc_insurance_upto { get; set; }
    public string? Rc_insurance_comp { get; set; }
    public string? Rc_insurance_policy_no { get; set; }
    public string? Rc_pucc_upto { get; set; }
    public string? Rc_tax_upto { get; set; }
    public string? Rc_permit_valid_upto { get; set; }
    public string? Rc_permit_type { get; set; }
    public string? Rc_np_no { get; set; }
    public string? Rc_np_upto { get; set; }
    public string? Rc_status { get; set; }
    public string? Rc_blacklist_status { get; set; }
    public string? Rc_financer { get; set; }
}

public class UlipSarathiRawResponse
{
    public string? Dlno { get; set; }
    public string? Name { get; set; }
    public string? Swd { get; set; }
    public string? Dob { get; set; }
    public string? Address { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }
    public string? Blood_grp { get; set; }
    public string? Gender { get; set; }
    public string? Photo { get; set; }
    public string? Issue_dt { get; set; }
    public string? Valid_from { get; set; }
    public string? Valid_to { get; set; }
    public string? Authority { get; set; }
    public string? Cov_details { get; set; }
    public string? Hazardous { get; set; }
    public string? Hill { get; set; }
    public string? Status { get; set; }
    public string? Suspended { get; set; }
    public string? Revoked { get; set; }
}

public class UlipFastagRawResponse
{
    public string? VehicleNumber { get; set; }
    public ICollection<UlipFastagTransactionRaw>? Transactions { get; set; }
}

public class UlipFastagTransactionRaw
{
    public string? TagId { get; set; }
    public string? PlazaName { get; set; }
    public string? PlazaId { get; set; }
    public string? LaneDirection { get; set; }
    public string? TxnDateTime { get; set; }
    public string? TxnId { get; set; }
    public string? TxnStatus { get; set; }
    public decimal? TxnAmount { get; set; }
    public string? PlazaState { get; set; }
    public string? PlazaHighway { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? VehicleClass { get; set; }
}

public class UlipTollRawResponse
{
    public string? PlazaCode { get; set; }
    public string? PlazaName { get; set; }
    public string? State { get; set; }
    public string? Highway { get; set; }
    public string? Direction { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? SingleJourneyRate { get; set; }
    public decimal? ReturnJourneyRate { get; set; }
    public decimal? MonthlyPassRate { get; set; }
    public string? VehicleClass { get; set; }
}
