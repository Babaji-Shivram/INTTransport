using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Warehouse;

namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// CharteredInfo API client — e-Invoice (IRN), GST lookup, E-Way Bill, QR Code.
/// Sandbox: https://gstsandbox.charteredinfo.com
/// </summary>
public interface ICharteredInfoClient
{
    /// <summary>Authenticate and get AuthToken.</summary>
    Task<string?> GetAuthTokenAsync(CancellationToken ct = default);

    /// <summary>GET /eivital/dec/v1.03/master/gstin/{gstin} — GST details lookup.</summary>
    Task<CharteredInfoGstRawResponse?> GetGstDetailsAsync(string gstin, CancellationToken ct = default);

    /// <summary>POST /eicore/dec/v1.03/Invoice — Generate IRN (e-Invoice).</summary>
    Task<CharteredInfoIrnRawResponse?> GenerateIrnAsync(object invoicePayload, CancellationToken ct = default);

    /// <summary>GET /eicore/dec/v1.03/Invoice/irn/{irn} — Get IRN details.</summary>
    Task<CharteredInfoIrnRawResponse?> GetIrnAsync(string irn, CancellationToken ct = default);

    /// <summary>GET /eicore/dec/v1.03/Invoice/irnbydocdetails — Get IRN by document.</summary>
    Task<CharteredInfoIrnRawResponse?> GetIrnByDocAsync(
        string docType, string docNum, string docDate, CancellationToken ct = default);

    /// <summary>POST /eicore/dec/v1.03/Invoice/Cancel — Cancel an IRN.</summary>
    Task<CharteredInfoCancelRawResponse?> CancelIrnAsync(
        string irn, string reason, string remarks, CancellationToken ct = default);

    /// <summary>POST /eiewb/dec/v1.03/ewaybill — Generate E-Way Bill from IRN.</summary>
    Task<CharteredInfoEwbRawResponse?> GenerateEwbAsync(object ewbPayload, CancellationToken ct = default);

    /// <summary>POST /ewaybillapi/dec/v1.03/ewayapi?action=CANEWB — Cancel E-Way Bill.</summary>
    Task<CharteredInfoCancelRawResponse?> CancelEwbAsync(
        long ewbNo, int reasonCode, string remarks, CancellationToken ct = default);

    /// <summary>GET /aspapi/v1.0/dynamicqr — Generate dynamic QR code.</summary>
    Task<string?> GetDynamicQrAsync(DynamicQrRequestDto request, CancellationToken ct = default);

    /// <summary>Health check — connectivity + token validation.</summary>
    Task<CharteredInfoHealthDto> HealthCheckAsync(CancellationToken ct = default);
}

// ── Raw Response Models ─────────────────────────────────────────

/// <summary>CharteredInfo standard API envelope.</summary>
public class CharteredInfoApiResponse
{
    public int Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? InfoDtls { get; set; }
}

public class CharteredInfoGstRawResponse
{
    // GSTIN details as returned by NIC
    public string? Gstin { get; set; }
    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public string? AddrBnm { get; set; }
    public string? AddrBno { get; set; }
    public string? AddrFlno { get; set; }
    public string? AddrSt { get; set; }
    public string? AddrLoc { get; set; }
    public string? AddrDst { get; set; }
    public string? AddrStcd { get; set; }
    public string? AddrPncd { get; set; }
    public string? Status { get; set; }
    public string? BlkStatus { get; set; }
    public string? DtReg { get; set; }
    public string? DtDreg { get; set; }
    public string? CtjCd { get; set; }
    public string? Ctb { get; set; } // Constitution of business
    public string? Nba { get; set; } // Nature of business activities
    public string? TxpType { get; set; }
    public string? EinvoiceStatus { get; set; }
}

public class CharteredInfoIrnRawResponse : CharteredInfoApiResponse
{
    public string? Irn { get; set; }
    public string? AckNo { get; set; }
    public string? AckDt { get; set; }
    public string? SignedInvoice { get; set; }
    public string? SignedQRCode { get; set; }
    public long? EwbNo { get; set; }
    public string? EwbDt { get; set; }
    public string? EwbValidTill { get; set; }
    public string? QRCodeUrl { get; set; }
    public string? QrCodeImage { get; set; } // Base64
    // Raw JSON if needed
    public string? Remarks { get; set; }
}

public class CharteredInfoCancelRawResponse : CharteredInfoApiResponse
{
    public string? Irn { get; set; }
    public string? CancelDate { get; set; }
}

public class CharteredInfoEwbRawResponse : CharteredInfoApiResponse
{
    public long? EwbNo { get; set; }
    public string? EwbDt { get; set; }
    public string? EwbValidTill { get; set; }
}
