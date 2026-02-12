using AutoMapper;
using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.Interfaces;
using ERP.Transport.Application.Interfaces.Clients;
using ERP.Transport.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace ERP.Transport.Application.Services;

/// <summary>
/// CharteredInfo business logic — GST lookup with caching, e-Invoice (IRN), E-Way Bill.
/// </summary>
public class CharteredInfoService : ICharteredInfoService
{
    private readonly ICharteredInfoClient _client;
    private readonly IRepository<GstDetail> _gstRepo;
    private readonly IRepository<EInvoice> _invoiceRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CharteredInfoService> _logger;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public CharteredInfoService(
        ICharteredInfoClient client,
        IRepository<GstDetail> gstRepo,
        IRepository<EInvoice> invoiceRepo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<CharteredInfoService> logger)
    {
        _client = client;
        _gstRepo = gstRepo;
        _invoiceRepo = invoiceRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    // ── GST Lookup (cached) ─────────────────────────────────────

    public async Task<GstDetailDto> LookupGstAsync(GstLookupRequestDto request)
    {
        var gstin = request.Gstin.Trim().ToUpperInvariant();

        if (!request.ForceRefresh)
        {
            var cached = await _gstRepo.FirstOrDefaultAsync(g => g.Gstin == gstin);
            if (cached != null && (DateTime.UtcNow - cached.LastFetchedFromApi) < CacheTtl)
            {
                _logger.LogDebug("GST cache hit for {Gstin}", gstin);
                return _mapper.Map<GstDetailDto>(cached);
            }
        }

        var raw = await _client.GetGstDetailsAsync(gstin);
        if (raw == null)
            throw new InvalidOperationException($"CharteredInfo GST lookup failed for {gstin}");

        var existing = await _gstRepo.FirstOrDefaultAsync(g => g.Gstin == gstin);
        var entity = existing ?? new GstDetail();

        entity.Gstin = gstin;
        entity.LegalName = raw.LegalName;
        entity.TradeName = raw.TradeName;
        entity.GstinStatus = raw.Status;
        entity.RegistrationDate = ParseDate(raw.DtReg);
        entity.CancellationDate = ParseDate(raw.DtDreg);
        entity.Address = BuildAddress(raw);
        entity.StateCode = raw.AddrStcd;
        entity.Pincode = raw.AddrPncd;
        entity.NatureOfBusiness = raw.Nba;
        entity.ConstitutionOfBusiness = raw.Ctb;
        entity.TaxpayerType = raw.TxpType;
        entity.IsEInvoiceApplicable = raw.EinvoiceStatus?.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        entity.LastFetchedFromApi = DateTime.UtcNow;
        entity.RawApiResponse = JsonSerializer.Serialize(raw);

        if (existing == null)
            await _gstRepo.AddAsync(entity);

        await _uow.SaveChangesAsync();
        return _mapper.Map<GstDetailDto>(entity);
    }

    // ── Generate IRN (e-Invoice) ────────────────────────────────

    public async Task<EInvoiceDto> GenerateIrnAsync(GenerateIrnRequestDto request, Guid userId)
    {
        // Build CharteredInfo invoice payload matching NIC schema
        var payload = BuildInvoicePayload(request);
        var payloadJson = JsonSerializer.Serialize(payload);

        var raw = await _client.GenerateIrnAsync(payload);
        if (raw == null)
            throw new InvalidOperationException("CharteredInfo Generate IRN failed");

        var entity = new EInvoice
        {
            TransportRequestId = request.TransportRequestId,
            Irn = raw.Irn,
            AckNumber = raw.AckNo,
            AckDate = ParseDateTime(raw.AckDt),
            SignedInvoice = raw.SignedInvoice,
            SignedQrCode = raw.SignedQRCode,
            QrCodeImageBase64 = raw.QrCodeImage,
            EInvoiceStatus = raw.Status == 1 ? "GENERATED" : "FAILED",
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            DocumentDate = ParseDate(request.DocumentDate),
            SellerGstin = request.SellerGstin,
            SellerLegalName = request.SellerLegalName,
            SellerTradeName = request.SellerTradeName,
            BuyerGstin = request.BuyerGstin,
            BuyerLegalName = request.BuyerLegalName,
            BuyerTradeName = request.BuyerTradeName,
            TotalAssessableValue = request.AssessableValue,
            TotalCgstValue = request.CgstValue,
            TotalSgstValue = request.SgstValue,
            TotalIgstValue = request.IgstValue,
            TotalCessValue = request.CessValue,
            TotalInvoiceValue = request.TotalInvoiceValue,
            EwbNumber = raw.EwbNo,
            EwbDate = ParseDateTime(raw.EwbDt),
            EwbValidTill = ParseDateTime(raw.EwbValidTill),
            RawRequest = payloadJson,
            RawResponse = JsonSerializer.Serialize(raw),
            CreatedBy = userId
        };

        await _invoiceRepo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return _mapper.Map<EInvoiceDto>(entity);
    }

    // ── Get IRN ─────────────────────────────────────────────────

    public async Task<EInvoiceDto?> GetIrnAsync(string irn)
    {
        // Check local DB first
        var local = await _invoiceRepo.FirstOrDefaultAsync(e => e.Irn == irn);
        if (local != null)
            return _mapper.Map<EInvoiceDto>(local);

        // Fetch from CharteredInfo
        var raw = await _client.GetIrnAsync(irn);
        if (raw == null) return null;

        // Don't persist remote-only lookups, just return DTO
        return new EInvoiceDto
        {
            Irn = raw.Irn,
            AckNumber = raw.AckNo,
            AckDate = ParseDateTime(raw.AckDt),
            EInvoiceStatus = "GENERATED",
            EwbNumber = raw.EwbNo,
            QrCodeImageBase64 = raw.QrCodeImage
        };
    }

    // ── Get IRN by Document ─────────────────────────────────────

    public async Task<EInvoiceDto?> GetIrnByDocumentAsync(
        string docType, string docNum, string docDate)
    {
        var local = await _invoiceRepo.FirstOrDefaultAsync(
            e => e.DocumentType == docType &&
                 e.DocumentNumber == docNum);
        if (local != null)
            return _mapper.Map<EInvoiceDto>(local);

        var raw = await _client.GetIrnByDocAsync(docType, docNum, docDate);
        if (raw == null) return null;

        return new EInvoiceDto
        {
            Irn = raw.Irn,
            AckNumber = raw.AckNo,
            AckDate = ParseDateTime(raw.AckDt),
            EInvoiceStatus = "GENERATED"
        };
    }

    // ── Cancel IRN ──────────────────────────────────────────────

    public async Task<EInvoiceDto> CancelIrnAsync(CancelIrnRequestDto request, Guid userId)
    {
        var entity = await _invoiceRepo.FirstOrDefaultAsync(e => e.Irn == request.Irn)
            ?? throw new InvalidOperationException($"E-Invoice not found for IRN: {request.Irn}");

        var raw = await _client.CancelIrnAsync(request.Irn, request.CancelReason, request.CancelRemarks)
            ?? throw new InvalidOperationException("CharteredInfo Cancel IRN API failed");

        entity.EInvoiceStatus = "CANCELLED";
        entity.CancelReason = request.CancelRemarks;
        entity.CancelRemarks = request.CancelRemarks;
        entity.CancelledDate = ParseDateTime(raw.CancelDate) ?? DateTime.UtcNow;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return _mapper.Map<EInvoiceDto>(entity);
    }

    // ── Generate E-Way Bill from IRN ────────────────────────────

    public async Task<EInvoiceDto> GenerateEwbFromIrnAsync(
        GenerateEwbFromIrnRequestDto request, Guid userId)
    {
        var entity = await _invoiceRepo.FirstOrDefaultAsync(e => e.Irn == request.Irn)
            ?? throw new InvalidOperationException($"E-Invoice not found for IRN: {request.Irn}");

        var payload = new
        {
            request.Irn,
            request.Distance,
            request.TransMode,
            TransId = request.TransporterId,
            TransName = request.TransporterName,
            request.TransDocDt,
            request.TransDocNo,
            request.VehNo,
            request.VehType
        };

        var raw = await _client.GenerateEwbAsync(payload)
            ?? throw new InvalidOperationException("CharteredInfo Generate EWB API failed");

        entity.EwbNumber = raw.EwbNo;
        entity.EwbDate = ParseDateTime(raw.EwbDt);
        entity.EwbValidTill = ParseDateTime(raw.EwbValidTill);
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return _mapper.Map<EInvoiceDto>(entity);
    }

    // ── Cancel E-Way Bill ───────────────────────────────────────

    public async Task CancelEwbAsync(CancelEwbRequestDto request)
    {
        var result = await _client.CancelEwbAsync(
            request.EwbNumber, request.CancelReasonCode, request.CancelRemarks)
            ?? throw new InvalidOperationException("CharteredInfo Cancel EWB API failed");

        // Update local record if exists
        var entity = await _invoiceRepo.FirstOrDefaultAsync(
            e => e.EwbNumber == request.EwbNumber);
        if (entity != null)
        {
            entity.EwbNumber = null; // EWB cancelled
            entity.UpdatedDate = DateTime.UtcNow;
            await _uow.SaveChangesAsync();
        }
    }

    // ── Get by Transport Job ────────────────────────────────────

    public async Task<IEnumerable<EInvoiceDto>> GetByTransportJobAsync(Guid transportRequestId)
    {
        var entities = await _invoiceRepo.FindAsync(
            e => e.TransportRequestId == transportRequestId);
        return _mapper.Map<IEnumerable<EInvoiceDto>>(entities);
    }

    // ── Dynamic QR Code ─────────────────────────────────────────

    public async Task<DynamicQrResponseDto> GetDynamicQrAsync(DynamicQrRequestDto request)
    {
        var base64 = await _client.GetDynamicQrAsync(request);
        return new DynamicQrResponseDto
        {
            QrCodeImageBase64 = base64,
            Success = !string.IsNullOrEmpty(base64),
            Message = string.IsNullOrEmpty(base64)
                ? "QR code generation failed" : "QR code generated successfully"
        };
    }

    // ── Health ──────────────────────────────────────────────────

    public Task<CharteredInfoHealthDto> HealthCheckAsync()
        => _client.HealthCheckAsync();

    // ── Private Helpers ─────────────────────────────────────────

    private static object BuildInvoicePayload(GenerateIrnRequestDto r)
    {
        var payload = new
        {
            Version = "1.1",
            TranDtls = new
            {
                TaxSch = "GST",
                SupTyp = r.SupplyType,
                RegRev = "N",
                IgstOnIntra = "N"
            },
            DocDtls = new
            {
                Typ = r.DocumentType,
                No = r.DocumentNumber,
                Dt = r.DocumentDate
            },
            SellerDtls = new
            {
                Gstin = r.SellerGstin,
                LglNm = r.SellerLegalName,
                TrdNm = r.SellerTradeName,
                Addr1 = r.SellerAddress1,
                Addr2 = r.SellerAddress2,
                Loc = r.SellerLocation,
                Pin = r.SellerPincode,
                Stcd = r.SellerStateCode,
                Ph = r.SellerPhone,
                Em = r.SellerEmail
            },
            BuyerDtls = new
            {
                Gstin = r.BuyerGstin,
                LglNm = r.BuyerLegalName,
                TrdNm = r.BuyerTradeName,
                Pos = r.BuyerPos ?? r.BuyerStateCode,
                Addr1 = r.BuyerAddress1,
                Addr2 = r.BuyerAddress2,
                Loc = r.BuyerLocation,
                Pin = r.BuyerPincode,
                Stcd = r.BuyerStateCode,
                Ph = r.BuyerPhone,
                Em = r.BuyerEmail
            },
            ItemList = r.Items.Select(item => new
            {
                item.SlNo,
                PrdDesc = item.ProductDescription,
                IsServc = item.IsService,
                HsnCd = item.HsnCode,
                Qty = item.Quantity,
                item.Unit,
                UnitPrice = item.UnitPrice,
                TotAmt = item.TotalAmount,
                item.Discount,
                AssAmt = item.AssessableAmount,
                GstRt = item.GstRate,
                IgstAmt = item.IgstAmount,
                CgstAmt = item.CgstAmount,
                SgstAmt = item.SgstAmount,
                CesRt = item.CessRate ?? 0m,
                CesAmt = item.CessAmount ?? 0m,
                TotItemVal = item.TotalItemValue
            }).ToArray(),
            ValDtls = new
            {
                AssVal = r.AssessableValue,
                CgstVal = r.CgstValue,
                SgstVal = r.SgstValue,
                IgstVal = r.IgstValue,
                CesVal = r.CessValue,
                Discount = r.Discount ?? 0m,
                OthChrg = r.OtherCharges ?? 0m,
                RndOffAmt = r.RoundOffAmount ?? 0m,
                TotInvVal = r.TotalInvoiceValue
            },
            EwbDtls = r.EwbDetails != null ? new
            {
                TransId = r.EwbDetails.TransporterId,
                TransName = r.EwbDetails.TransporterName,
                r.EwbDetails.Distance,
                r.EwbDetails.TransDocNo,
                r.EwbDetails.TransDocDt,
                VehNo = r.EwbDetails.VehNo,
                r.EwbDetails.VehType,
                r.EwbDetails.TransMode
            } : null
        };

        return payload;
    }

    private static string? BuildAddress(CharteredInfoGstRawResponse raw)
    {
        var parts = new[]
        {
            raw.AddrFlno, raw.AddrBno, raw.AddrBnm,
            raw.AddrSt, raw.AddrLoc, raw.AddrDst
        }.Where(p => !string.IsNullOrWhiteSpace(p));

        return parts.Any() ? string.Join(", ", parts) : null;
    }

    private static DateTime? ParseDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;

        // Try dd/MM/yyyy first (CharteredInfo format)
        if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
            return dt1;

        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt2))
            return dt2;

        return null;
    }

    private static DateTime? ParseDateTime(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;

        // Try dd/MM/yyyy hh:mm:ss tt
        if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
            return dt1;

        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt2))
            return dt2;

        return null;
    }
}
