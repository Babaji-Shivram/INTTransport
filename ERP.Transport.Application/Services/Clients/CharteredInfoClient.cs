using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services.Clients;

/// <summary>
/// CharteredInfo API client — e-Invoice sandbox.
/// Base: https://gstsandbox.charteredinfo.com
/// Auth: GET /eivital/dec/v1.04/auth → AuthToken
/// All calls pass aspid + password + Gstin + User_name + AuthToken as query params.
/// </summary>
public class CharteredInfoClient : ICharteredInfoClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<CharteredInfoClient> _logger;

    // Token cache (same pattern as UlipClient)
    private static string? _cachedToken;
    private static DateTime _tokenExpiry = DateTime.MinValue;
    private static readonly SemaphoreSlim _tokenLock = new(1, 1);

    public CharteredInfoClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<CharteredInfoClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    // ── Auth ────────────────────────────────────────────────────

    public async Task<string?> GetAuthTokenAsync(CancellationToken ct = default)
    {
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        await _tokenLock.WaitAsync(ct);
        try
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken;

            var queryParams = BuildAuthQueryString();
            var response = await _httpClient.GetAsync($"eivital/dec/v1.04/auth?{queryParams}", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

            if (json.TryGetProperty("AuthToken", out var tokenProp))
            {
                _cachedToken = tokenProp.GetString();
                // Token typically valid for ~6 hours, refresh at 5h
                _tokenExpiry = DateTime.UtcNow.AddHours(5);
                _logger.LogInformation("CharteredInfo auth token acquired, expires at {Expiry}", _tokenExpiry);
                return _cachedToken;
            }

            _logger.LogWarning("CharteredInfo auth response missing AuthToken");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo authentication failed");
            return null;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    // ── GST Details ─────────────────────────────────────────────

    public async Task<CharteredInfoGstRawResponse?> GetGstDetailsAsync(
        string gstin, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildApiQueryString(token);
            var response = await _httpClient.GetAsync(
                $"eivital/dec/v1.03/master/gstin/{gstin}?{qs}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoGstRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo GST lookup failed for {Gstin}", gstin);
            return null;
        }
    }

    // ── Generate IRN ────────────────────────────────────────────

    public async Task<CharteredInfoIrnRawResponse?> GenerateIrnAsync(
        object invoicePayload, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildIrnQueryString(token);
            var content = new StringContent(
                JsonSerializer.Serialize(invoicePayload),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"eicore/dec/v1.03/Invoice?{qs}", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoIrnRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Generate IRN failed");
            return null;
        }
    }

    // ── Get IRN ─────────────────────────────────────────────────

    public async Task<CharteredInfoIrnRawResponse?> GetIrnAsync(
        string irn, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildApiQueryString(token, includeQrSize: true);
            var response = await _httpClient.GetAsync(
                $"eicore/dec/v1.03/Invoice/irn/{irn}?{qs}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoIrnRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Get IRN failed for {Irn}", irn);
            return null;
        }
    }

    // ── Get IRN by Document ─────────────────────────────────────

    public async Task<CharteredInfoIrnRawResponse?> GetIrnByDocAsync(
        string docType, string docNum, string docDate, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildApiQueryString(token);
            var response = await _httpClient.GetAsync(
                $"eicore/dec/v1.03/Invoice/irnbydocdetails?doctype={docType}&docnum={Uri.EscapeDataString(docNum)}&docdate={docDate}&{qs}",
                ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoIrnRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Get IRN by doc failed");
            return null;
        }
    }

    // ── Cancel IRN ──────────────────────────────────────────────

    public async Task<CharteredInfoCancelRawResponse?> CancelIrnAsync(
        string irn, string reason, string remarks, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildApiQueryString(token);
            var body = new { Irn = irn, CnlRsn = reason, CnlRem = remarks };
            var content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"eicore/dec/v1.03/Invoice/Cancel?{qs}", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoCancelRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Cancel IRN failed for {Irn}", irn);
            return null;
        }
    }

    // ── Generate E-Way Bill ─────────────────────────────────────

    public async Task<CharteredInfoEwbRawResponse?> GenerateEwbAsync(
        object ewbPayload, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildIrnQueryString(token);
            var content = new StringContent(
                JsonSerializer.Serialize(ewbPayload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"eiewb/dec/v1.03/ewaybill?{qs}", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoEwbRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Generate E-Way Bill failed");
            return null;
        }
    }

    // ── Cancel E-Way Bill ───────────────────────────────────────

    public async Task<CharteredInfoCancelRawResponse?> CancelEwbAsync(
        long ewbNo, int reasonCode, string remarks, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAuthTokenAsync(ct);
            if (token == null) return null;

            var qs = BuildIrnQueryString(token);
            var body = new { ewbNo, cancelRsnCode = reasonCode, cancelRmrk = remarks };
            var content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"ewaybillapi/dec/v1.03/ewayapi?action=CANEWB&{qs}", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CharteredInfoCancelRawResponse>(
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Cancel EWB failed for {EwbNo}", ewbNo);
            return null;
        }
    }

    // ── Dynamic QR Code ─────────────────────────────────────────

    public async Task<string?> GetDynamicQrAsync(
        DynamicQrRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var aspid = _config["CharteredInfo:AspId"];
            var password = _config["CharteredInfo:Password"];
            var gstin = _config["CharteredInfo:Gstin"];

            var qs = $"aspid={aspid}&password={password}&Gstin={gstin}" +
                     $"&UpiId={Uri.EscapeDataString(request.UpiId)}" +
                     $"&PayeeName={Uri.EscapeDataString(request.PayeeName)}" +
                     $"&BankAc={request.BankAccountNumber}" +
                     $"&Ifsc={request.IfscCode}" +
                     $"&InvNo={Uri.EscapeDataString(request.InvoiceNumber)}" +
                     $"&InvDt={Uri.EscapeDataString(request.InvoiceDate)}" +
                     $"&InvAmt={request.InvoiceAmount}" +
                     $"&QrCodeSize={request.QrCodeSize}";

            var response = await _httpClient.GetAsync($"aspapi/v1.0/dynamicqr?{qs}", ct);
            response.EnsureSuccessStatusCode();

            // Returns image bytes or base64
            var bytes = await response.Content.ReadAsByteArrayAsync(ct);
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CharteredInfo Dynamic QR generation failed");
            return null;
        }
    }

    // ── Health Check ────────────────────────────────────────────

    public async Task<CharteredInfoHealthDto> HealthCheckAsync(CancellationToken ct = default)
    {
        var result = new CharteredInfoHealthDto
        {
            SandboxUrl = _config["CharteredInfo:BaseUrl"]
                ?? "https://gstsandbox.charteredinfo.com"
        };

        try
        {
            var token = await GetAuthTokenAsync(ct);
            result.IsConnected = true;
            result.HasValidToken = !string.IsNullOrEmpty(token);
            result.TokenExpiresAt = _tokenExpiry.ToString("o");
            result.Message = result.HasValidToken
                ? "CharteredInfo API connected and authenticated"
                : "CharteredInfo API connected but authentication failed";
        }
        catch (Exception ex)
        {
            result.IsConnected = false;
            result.Message = $"CharteredInfo API unreachable: {ex.Message}";
        }

        return result;
    }

    // ── Private Helpers ─────────────────────────────────────────

    private string BuildAuthQueryString()
    {
        var aspid = _config["CharteredInfo:AspId"];
        var password = _config["CharteredInfo:Password"];
        var gstin = _config["CharteredInfo:Gstin"];
        var userName = _config["CharteredInfo:UserName"];
        var eInvPwd = _config["CharteredInfo:EInvPassword"];

        return $"aspid={aspid}&password={password}" +
               $"&Gstin={gstin}&User_name={userName}&eInvPwd={eInvPwd}";
    }

    private string BuildApiQueryString(string authToken, bool includeQrSize = false)
    {
        var aspid = _config["CharteredInfo:AspId"];
        var password = _config["CharteredInfo:Password"];
        var gstin = _config["CharteredInfo:Gstin"];
        var userName = _config["CharteredInfo:UserName"];

        var qs = $"aspid={aspid}&password={password}" +
                 $"&Gstin={gstin}&User_name={userName}&AuthToken={authToken}";

        if (includeQrSize)
            qs += "&QrCodeSize=250";

        return qs;
    }

    private string BuildIrnQueryString(string authToken)
    {
        var aspid = _config["CharteredInfo:AspId"];
        var password = _config["CharteredInfo:Password"];
        var gstin = _config["CharteredInfo:Gstin"];
        var userName = _config["CharteredInfo:UserName"];
        var eInvPwd = _config["CharteredInfo:EInvPassword"];

        return $"aspid={aspid}&password={password}" +
               $"&Gstin={gstin}&eInvPwd={eInvPwd}" +
               $"&User_name={userName}&AuthToken={authToken}&QrCodeSize=250";
    }
}
