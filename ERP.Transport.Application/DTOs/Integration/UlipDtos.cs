namespace ERP.Transport.Application.DTOs.Integration;

// ═══════════════════════════════════════════════════════════════
//  ULIP DTOs — mapped from ULIP staging API responses
//  Base: https://www.ulipstaging.dpiit.gov.in/ulip/v1.0.0
// ═══════════════════════════════════════════════════════════════

// ── Auth ────────────────────────────────────────────────────────

public class UlipLoginRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UlipLoginResponseDto
{
    public string? Response { get; set; }
    public string? Code { get; set; }
    public string? Status { get; set; }
}

// ── VAHAN/01 — Vehicle Lookup ───────────────────────────────────

public class VehicleDetailDto
{
    public Guid? Id { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public string? OwnerName { get; set; }
    public string? FatherName { get; set; }
    public string? PresentAddress { get; set; }
    public string? VehicleClass { get; set; }
    public string? VehicleCategory { get; set; }
    public string? MakerModel { get; set; }
    public string? BodyType { get; set; }
    public string? FuelType { get; set; }
    public string? Color { get; set; }
    public int? ManufacturingYear { get; set; }
    public decimal? GrossVehicleWeight { get; set; }
    public string? EngineNumber { get; set; }
    public string? ChassisNumber { get; set; }
    public string? RegisteringAuthority { get; set; }
    public string? RegistrationState { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? FitnessUpto { get; set; }
    public DateTime? InsuranceUpto { get; set; }
    public string? InsuranceCompany { get; set; }
    public DateTime? PucValidUpto { get; set; }
    public DateTime? TaxValidUpto { get; set; }
    public DateTime? PermitValidUpto { get; set; }
    public string? PermitType { get; set; }
    public string? NationalPermitNumber { get; set; }
    public string? VehicleStatus { get; set; }
    public bool? IsBlacklisted { get; set; }
    public bool? IsFinanced { get; set; }
    public string? FinancerName { get; set; }
    public DateTime LastFetchedFromUlip { get; set; }

    // ── Compliance flags (computed) ─────────────────────────────
    public bool IsFitnessExpired => FitnessUpto.HasValue && FitnessUpto.Value < DateTime.UtcNow;
    public bool IsInsuranceExpired => InsuranceUpto.HasValue && InsuranceUpto.Value < DateTime.UtcNow;
    public bool IsPucExpired => PucValidUpto.HasValue && PucValidUpto.Value < DateTime.UtcNow;
    public bool IsPermitExpired => PermitValidUpto.HasValue && PermitValidUpto.Value < DateTime.UtcNow;
}

public class VehicleLookupRequestDto
{
    public string VehicleNumber { get; set; } = null!;
    public bool ForceRefresh { get; set; }
}

// ── SARATHI/01 — Driver License ─────────────────────────────────

public class DriverLicenseDetailDto
{
    public Guid? Id { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string? HolderName { get; set; }
    public string? FatherOrHusbandName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? State { get; set; }
    public string? BloodGroup { get; set; }
    public string? Gender { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? VehicleClassesAuthorized { get; set; }
    public bool? HasHazardousGoodsEndorsement { get; set; }
    public bool? HasHillDrivingEndorsement { get; set; }
    public string? LicenseStatus { get; set; }
    public bool? IsSuspended { get; set; }
    public bool? IsRevoked { get; set; }
    public DateTime LastFetchedFromUlip { get; set; }

    // ── Compliance flags ────────────────────────────────────────
    public bool IsExpired => ValidTo.HasValue && ValidTo.Value < DateTime.UtcNow;
    public bool IsValidForHMV => VehicleClassesAuthorized?.Contains("HMV") == true
                              || VehicleClassesAuthorized?.Contains("HPMV") == true
                              || VehicleClassesAuthorized?.Contains("TRANS") == true;
}

public class DriverLicenseVerifyRequestDto
{
    public string LicenseNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public bool ForceRefresh { get; set; }
}

// ── FASTAG/01 — FASTag Toll Tracking ────────────────────────────

public class FASTagTransactionDto
{
    public Guid? Id { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public string? TagId { get; set; }
    public string? TollPlazaName { get; set; }
    public string? TollPlazaId { get; set; }
    public string? LaneDirection { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public string? TransactionId { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public string? PlazaState { get; set; }
    public string? PlazaHighway { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? VehicleClassAtToll { get; set; }
    public Guid? TransportRequestId { get; set; }
}

public class FASTagLookupResponseDto
{
    public string VehicleNumber { get; set; } = null!;
    public ICollection<FASTagTransactionDto> Transactions { get; set; } = new List<FASTagTransactionDto>();
    public int TotalTransactions { get; set; }
    public decimal? TotalTollAmount { get; set; }
    public string? LastKnownLocation { get; set; }
    public DateTime? LastTransactionTime { get; set; }
}

// ── TOLL/01 — Toll Plaza Details ────────────────────────────────

public class TollPlazaDto
{
    public Guid? Id { get; set; }
    public string PlazaCode { get; set; } = null!;
    public string? PlazaName { get; set; }
    public string? State { get; set; }
    public string? Highway { get; set; }
    public string? Direction { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? SingleJourneyRate { get; set; }
    public decimal? ReturnJourneyRate { get; set; }
    public decimal? MonthlyPassRate { get; set; }
    public string? VehicleClassApplicable { get; set; }
}

// ── E-Way Bill ──────────────────────────────────────────────────

public class EWayBillDto
{
    public Guid? Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public string? EWayBillNumber { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public DateTime? ValidUpto { get; set; }
    public string? EWayBillStatus { get; set; }
    public string? SupplierGstin { get; set; }
    public string? SupplierName { get; set; }
    public string? RecipientGstin { get; set; }
    public string? RecipientName { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? DocumentDate { get; set; }
    public string? HsnCode { get; set; }
    public string? ProductDescription { get; set; }
    public decimal? TaxableAmount { get; set; }
    public decimal? TotalInvoiceValue { get; set; }
    public string? TransporterGstin { get; set; }
    public string? VehicleNumber { get; set; }
    public decimal? ApproximateDistanceKm { get; set; }
    public string? FromState { get; set; }
    public string? FromPlace { get; set; }
    public string? ToState { get; set; }
    public string? ToPlace { get; set; }

    public bool IsExpired => ValidUpto.HasValue && ValidUpto.Value < DateTime.UtcNow;
}

public class GenerateEWayBillRequestDto
{
    public Guid TransportRequestId { get; set; }
    public string SupplierGstin { get; set; } = null!;
    public string RecipientGstin { get; set; } = null!;
    public string DocumentType { get; set; } = "INV";
    public string DocumentNumber { get; set; } = null!;
    public DateTime DocumentDate { get; set; }
    public string HsnCode { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public decimal TaxableAmount { get; set; }
    public decimal TotalInvoiceValue { get; set; }
    public string TransporterGstin { get; set; } = null!;
    public string VehicleNumber { get; set; } = null!;
    public decimal ApproximateDistanceKm { get; set; }
    public string FromPincode { get; set; } = null!;
    public string ToPincode { get; set; } = null!;
}

// ── ULIP Health Check ───────────────────────────────────────────

public class UlipHealthDto
{
    public bool IsConnected { get; set; }
    public bool HasValidToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public string? StagingUrl { get; set; }
    public string? Message { get; set; }
}
