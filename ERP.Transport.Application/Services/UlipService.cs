using AutoMapper;
using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.Interfaces;
using ERP.Transport.Application.Interfaces.Clients;
using ERP.Transport.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ERP.Transport.Application.Services;

/// <summary>
/// ULIP business logic — caches lookups in DB, maps raw ULIP responses to domain entities.
/// Cache TTL: 24 hours for VAHAN/SARATHI, no cache for FASTag (real-time).
/// </summary>
public class UlipService : IUlipService
{
    private readonly IUlipClient _ulipClient;
    private readonly IRepository<VehicleDetail> _vehicleRepo;
    private readonly IRepository<DriverLicenseDetail> _driverRepo;
    private readonly IRepository<FASTagTransaction> _fastagRepo;
    private readonly IRepository<TollPlaza> _tollRepo;
    private readonly IRepository<EWayBill> _ewayBillRepo;
    private readonly IRepository<TransportRequest> _transportRequestRepo;
    private readonly IRepository<TransportVehicle> _transportVehicleRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<UlipService> _logger;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public UlipService(
        IUlipClient ulipClient,
        IRepository<VehicleDetail> vehicleRepo,
        IRepository<DriverLicenseDetail> driverRepo,
        IRepository<FASTagTransaction> fastagRepo,
        IRepository<TollPlaza> tollRepo,
        IRepository<EWayBill> ewayBillRepo,
        IRepository<TransportRequest> transportRequestRepo,
        IRepository<TransportVehicle> transportVehicleRepo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<UlipService> logger)
    {
        _ulipClient = ulipClient;
        _vehicleRepo = vehicleRepo;
        _driverRepo = driverRepo;
        _fastagRepo = fastagRepo;
        _tollRepo = tollRepo;
        _ewayBillRepo = ewayBillRepo;
        _transportRequestRepo = transportRequestRepo;
        _transportVehicleRepo = transportVehicleRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    // ── VAHAN — Vehicle Lookup ──────────────────────────────────

    public async Task<VehicleDetailDto> LookupVehicleAsync(
        VehicleLookupRequestDto request, CancellationToken ct = default)
    {
        var normalizedNumber = NormalizeVehicleNumber(request.VehicleNumber);

        // Check DB cache first (unless force refresh)
        if (!request.ForceRefresh)
        {
            var cached = (await _vehicleRepo.FindAsync(
                v => v.VehicleNumber == normalizedNumber)).FirstOrDefault();

            if (cached != null && (DateTime.UtcNow - cached.LastFetchedFromUlip) < CacheTtl)
            {
                _logger.LogDebug("VAHAN cache hit for {VehicleNumber}", normalizedNumber);
                return _mapper.Map<VehicleDetailDto>(cached);
            }
        }

        // Call ULIP
        var raw = await _ulipClient.GetVehicleDetailsAsync(normalizedNumber, ct);
        if (raw == null)
            throw new InvalidOperationException($"ULIP VAHAN lookup failed for {normalizedNumber}");

        // Upsert in DB
        var existing = (await _vehicleRepo.FindAsync(
            v => v.VehicleNumber == normalizedNumber)).FirstOrDefault();

        var entity = existing ?? new VehicleDetail();
        MapVahanToEntity(raw, entity, normalizedNumber);

        if (existing == null)
            await _vehicleRepo.AddAsync(entity);

        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<VehicleDetailDto>(entity);
    }

    // ── SARATHI — Driver License ────────────────────────────────

    public async Task<DriverLicenseDetailDto> VerifyDriverLicenseAsync(
        DriverLicenseVerifyRequestDto request, CancellationToken ct = default)
    {
        var normalizedDl = request.LicenseNumber.Trim().ToUpperInvariant();

        if (!request.ForceRefresh)
        {
            var cached = (await _driverRepo.FindAsync(
                d => d.LicenseNumber == normalizedDl)).FirstOrDefault();

            if (cached != null && (DateTime.UtcNow - cached.LastFetchedFromUlip) < CacheTtl)
            {
                _logger.LogDebug("SARATHI cache hit for {LicenseNumber}", normalizedDl);
                return _mapper.Map<DriverLicenseDetailDto>(cached);
            }
        }

        var dobStr = request.DateOfBirth.ToString("yyyy-MM-dd");
        var raw = await _ulipClient.VerifyDriverLicenseAsync(normalizedDl, dobStr, ct);
        if (raw == null)
            throw new InvalidOperationException($"ULIP SARATHI verification failed for {normalizedDl}");

        var existing = (await _driverRepo.FindAsync(
            d => d.LicenseNumber == normalizedDl)).FirstOrDefault();

        var entity = existing ?? new DriverLicenseDetail();
        MapSarathiToEntity(raw, entity, normalizedDl, request.DateOfBirth);

        if (existing == null)
            await _driverRepo.AddAsync(entity);

        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<DriverLicenseDetailDto>(entity);
    }

    // ── FASTAG — Toll Tracking (always real-time) ───────────────

    public async Task<FASTagLookupResponseDto> GetFASTagTransactionsAsync(
        string vehicleNumber, Guid? transportRequestId = null, CancellationToken ct = default)
    {
        var normalized = NormalizeVehicleNumber(vehicleNumber);
        var raw = await _ulipClient.GetFASTagTransactionsAsync(normalized, ct);

        var result = new FASTagLookupResponseDto { VehicleNumber = normalized };

        if (raw?.Transactions != null)
        {
            foreach (var txn in raw.Transactions)
            {
                var entity = new FASTagTransaction
                {
                    VehicleNumber = normalized,
                    TagId = txn.TagId,
                    TollPlazaName = txn.PlazaName,
                    TollPlazaId = txn.PlazaId,
                    LaneDirection = txn.LaneDirection,
                    TransactionDateTime = ParseDate(txn.TxnDateTime),
                    TransactionId = txn.TxnId,
                    TransactionStatus = txn.TxnStatus,
                    TransactionAmount = txn.TxnAmount,
                    PlazaState = txn.PlazaState,
                    PlazaHighway = txn.PlazaHighway,
                    Latitude = txn.Latitude,
                    Longitude = txn.Longitude,
                    VehicleClassAtToll = txn.VehicleClass,
                    TransportRequestId = transportRequestId,
                    FetchedFromUlip = DateTime.UtcNow
                };

                await _fastagRepo.AddAsync(entity);
                result.Transactions.Add(_mapper.Map<FASTagTransactionDto>(entity));
            }

            await _uow.SaveChangesAsync(ct);

            result.TotalTransactions = result.Transactions.Count;
            result.TotalTollAmount = result.Transactions.Sum(t => t.TransactionAmount ?? 0);
            var lastTxn = result.Transactions
                .OrderByDescending(t => t.TransactionDateTime).FirstOrDefault();
            result.LastKnownLocation = lastTxn?.TollPlazaName;
            result.LastTransactionTime = lastTxn?.TransactionDateTime;
        }

        return result;
    }

    // ── TOLL — Plaza Details ────────────────────────────────────

    public async Task<TollPlazaDto?> GetTollPlazaAsync(
        string plazaId, CancellationToken ct = default)
    {
        // Check cache
        var cached = (await _tollRepo.FindAsync(
            t => t.PlazaCode == plazaId)).FirstOrDefault();

        if (cached != null && (DateTime.UtcNow - cached.LastFetchedFromUlip) < CacheTtl)
            return _mapper.Map<TollPlazaDto>(cached);

        var raw = await _ulipClient.GetTollPlazaAsync(plazaId, ct);
        if (raw == null) return null;

        var entity = cached ?? new TollPlaza();
        entity.PlazaCode = raw.PlazaCode ?? plazaId;
        entity.PlazaName = raw.PlazaName;
        entity.State = raw.State;
        entity.Highway = raw.Highway;
        entity.Direction = raw.Direction;
        entity.Latitude = raw.Latitude;
        entity.Longitude = raw.Longitude;
        entity.SingleJourneyRate = raw.SingleJourneyRate;
        entity.ReturnJourneyRate = raw.ReturnJourneyRate;
        entity.MonthlyPassRate = raw.MonthlyPassRate;
        entity.VehicleClassApplicable = raw.VehicleClass;
        entity.LastFetchedFromUlip = DateTime.UtcNow;

        if (cached == null)
            await _tollRepo.AddAsync(entity);

        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<TollPlazaDto>(entity);
    }

    // ── E-Way Bill ──────────────────────────────────────────────

    public async Task<EWayBillDto> GenerateEWayBillAsync(
        GenerateEWayBillRequestDto request, Guid userId, CancellationToken ct = default)
    {
        // For now, create a local record. Real NIC/E-Way Bill API integration
        // will be added when CharteredInfo collection is provided.
        var entity = new EWayBill
        {
            TransportRequestId = request.TransportRequestId,
            EWayBillStatus = "PENDING",
            SupplierGstin = request.SupplierGstin,
            RecipientGstin = request.RecipientGstin,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            DocumentDate = request.DocumentDate,
            HsnCode = request.HsnCode,
            ProductDescription = request.ProductDescription,
            TaxableAmount = request.TaxableAmount,
            TotalInvoiceValue = request.TotalInvoiceValue,
            TransporterGstin = request.TransporterGstin,
            VehicleNumber = request.VehicleNumber,
            ApproximateDistanceKm = request.ApproximateDistanceKm,
            FromPincode = request.FromPincode,
            ToPincode = request.ToPincode,
            CreatedBy = userId
        };

        await _ewayBillRepo.AddAsync(entity);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<EWayBillDto>(entity);
    }

    /// <summary>
    /// Auto-fill E-Way Bill fields from a transport job — uses job's customer GST,
    /// pickup/drop pincodes, cargo info, and first assigned vehicle + transporter details.
    /// </summary>
    public async Task<EWayBillDto> GenerateEWayBillFromJobAsync(
        Guid transportRequestId, Guid userId, CancellationToken ct = default)
    {
        var job = await _transportRequestRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Transport job {transportRequestId} not found");

        // Get the first active vehicle assignment for vehicle number + transporter GST
        var vehicles = await _transportVehicleRepo.FindAsync(
            v => v.TransportRequestId == transportRequestId && v.IsActive);
        var vehicle = vehicles.FirstOrDefault();

        var entity = new EWayBill
        {
            TransportRequestId = transportRequestId,
            EWayBillStatus = "PENDING",

            // Auto-fill from job
            SupplierGstin = job.GSTNumber ?? string.Empty,
            RecipientGstin = job.GSTNumber ?? string.Empty,  // same party or overridden
            DocumentType = "INV",
            DocumentNumber = job.RequestNumber,
            DocumentDate = job.RequestDate,
            ProductDescription = job.CargoDescription ?? job.CargoType.ToString(),
            FromPincode = job.PickupPincode ?? string.Empty,
            ToPincode = job.DropPincode ?? string.Empty,

            // Auto-fill from vehicle assignment
            VehicleNumber = vehicle?.VehicleNumber ?? string.Empty,
            TransporterGstin = string.Empty, // Will be filled from transporter master

            CreatedBy = userId
        };

        await _ewayBillRepo.AddAsync(entity);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Auto-generated E-Way Bill for job {JobId} ({RequestNumber})",
            transportRequestId, job.RequestNumber);

        return _mapper.Map<EWayBillDto>(entity);
    }

    public async Task<EWayBillDto?> GetEWayBillByJobAsync(
        Guid transportRequestId, CancellationToken ct = default)
    {
        var entity = (await _ewayBillRepo.FindAsync(
            e => e.TransportRequestId == transportRequestId)).FirstOrDefault();

        return entity == null ? null : _mapper.Map<EWayBillDto>(entity);
    }

    // ── Health ──────────────────────────────────────────────────

    public Task<UlipHealthDto> HealthCheckAsync(CancellationToken ct = default)
        => _ulipClient.HealthCheckAsync(ct);

    // ── Private Helpers ─────────────────────────────────────────

    private static string NormalizeVehicleNumber(string vehicleNumber)
        => vehicleNumber.Trim().ToUpperInvariant().Replace(" ", "").Replace("-", "");

    private static DateTime? ParseDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;
        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt))
            return dt;
        return null;
    }

    private static void MapVahanToEntity(
        UlipVahanRawResponse raw, VehicleDetail entity, string vehicleNumber)
    {
        entity.VehicleNumber = vehicleNumber;
        entity.OwnerName = raw.Rc_owner_name;
        entity.FatherName = raw.Rc_f_name;
        entity.PresentAddress = raw.Rc_present_address;
        entity.PermanentAddress = raw.Rc_permanent_address;
        entity.VehicleClass = raw.Rc_vh_class_desc;
        entity.VehicleCategory = raw.Rc_vch_catg;
        entity.MakerModel = raw.Rc_maker_model;
        entity.MakerDescription = raw.Rc_maker_desc;
        entity.BodyType = raw.Rc_body_type_desc;
        entity.FuelType = raw.Rc_fuel_desc;
        entity.Color = raw.Rc_color;
        entity.SeatingCapacity = raw.Rc_seat_cap;
        entity.GrossVehicleWeight = raw.Rc_gvw;
        entity.UnladenWeight = raw.Rc_unld_wt;
        entity.NumberOfCylinders = raw.Rc_no_cyl;
        entity.EngineNumber = raw.Rc_eng_no;
        entity.ChassisNumber = raw.Rc_chasi_no;
        entity.RegisteringAuthority = raw.Rc_registered_at;
        entity.RegistrationState = raw.Rc_registration_state;
        entity.RegistrationDate = ParseDate(raw.Rc_regn_dt);
        entity.FitnessUpto = ParseDate(raw.Rc_fit_upto);
        entity.InsuranceUpto = ParseDate(raw.Rc_insurance_upto);
        entity.InsuranceCompany = raw.Rc_insurance_comp;
        entity.InsurancePolicyNumber = raw.Rc_insurance_policy_no;
        entity.PucValidUpto = ParseDate(raw.Rc_pucc_upto);
        entity.TaxValidUpto = ParseDate(raw.Rc_tax_upto);
        entity.PermitValidUpto = ParseDate(raw.Rc_permit_valid_upto);
        entity.PermitType = raw.Rc_permit_type;
        entity.NationalPermitNumber = raw.Rc_np_no;
        entity.NationalPermitUpto = ParseDate(raw.Rc_np_upto);
        entity.VehicleStatus = raw.Rc_status;
        entity.IsBlacklisted = raw.Rc_blacklist_status?.Equals("Y", StringComparison.OrdinalIgnoreCase);
        entity.IsFinanced = !string.IsNullOrEmpty(raw.Rc_financer);
        entity.FinancerName = raw.Rc_financer;
        entity.LastFetchedFromUlip = DateTime.UtcNow;
        entity.RawApiResponse = System.Text.Json.JsonSerializer.Serialize(raw);
    }

    private static void MapSarathiToEntity(
        UlipSarathiRawResponse raw, DriverLicenseDetail entity,
        string licenseNumber, DateTime dob)
    {
        entity.LicenseNumber = licenseNumber;
        entity.DateOfBirth = dob;
        entity.HolderName = raw.Name;
        entity.FatherOrHusbandName = raw.Swd;
        entity.Address = raw.Address;
        entity.State = raw.State;
        entity.PinCode = raw.Pincode;
        entity.BloodGroup = raw.Blood_grp;
        entity.Gender = raw.Gender;
        entity.PhotoUrl = raw.Photo;
        entity.IssueDate = ParseDate(raw.Issue_dt);
        entity.ValidFrom = ParseDate(raw.Valid_from);
        entity.ValidTo = ParseDate(raw.Valid_to);
        entity.IssuingAuthority = raw.Authority;
        entity.VehicleClassesAuthorized = raw.Cov_details;
        entity.HasHazardousGoodsEndorsement = raw.Hazardous?.Equals("Y", StringComparison.OrdinalIgnoreCase);
        entity.HasHillDrivingEndorsement = raw.Hill?.Equals("Y", StringComparison.OrdinalIgnoreCase);
        entity.LicenseStatus = raw.Status;
        entity.IsSuspended = raw.Suspended?.Equals("Y", StringComparison.OrdinalIgnoreCase);
        entity.IsRevoked = raw.Revoked?.Equals("Y", StringComparison.OrdinalIgnoreCase);
        entity.LastFetchedFromUlip = DateTime.UtcNow;
        entity.RawApiResponse = System.Text.Json.JsonSerializer.Serialize(raw);
    }
}
