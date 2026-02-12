using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ERP.Transport.API.Clients;

/// <summary>
/// Direct ULIP staging API client.
/// Base: https://www.ulipstaging.dpiit.gov.in/ulip/v1.0.0
/// Token is cached and refreshed when expired.
/// </summary>
public class UlipClient : IUlipClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UlipClient> _logger;
    private readonly IConfiguration _configuration;

    private static string? _cachedToken;
    private static DateTime _tokenExpiresAt = DateTime.MinValue;
    private static readonly SemaphoreSlim _tokenLock = new(1, 1);

    public UlipClient(
        HttpClient httpClient,
        ILogger<UlipClient> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    // ── Token Management ────────────────────────────────────────

    public async Task<string?> GetTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiresAt > DateTime.UtcNow)
            return _cachedToken;

        await _tokenLock.WaitAsync(ct);
        try
        {
            // Double-check after acquiring lock
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiresAt > DateTime.UtcNow)
                return _cachedToken;

            var username = _configuration["Ulip:Username"];
            var password = _configuration["Ulip:Password"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("ULIP credentials not configured (Ulip:Username / Ulip:Password)");
                return null;
            }

            var loginPayload = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("user/login", loginPayload, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            // ULIP returns token in "response" field
            if (doc.RootElement.TryGetProperty("response", out var tokenElement))
            {
                _cachedToken = tokenElement.GetString();
                // ULIP tokens typically valid for 24h; refresh at 23h
                _tokenExpiresAt = DateTime.UtcNow.AddHours(23);
                _logger.LogInformation("ULIP token refreshed, expires at {ExpiresAt}", _tokenExpiresAt);
                return _cachedToken;
            }

            _logger.LogError("ULIP login response missing 'response' field: {Json}", json);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to obtain ULIP token");
            return null;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    // ── VAHAN/01 ────────────────────────────────────────────────

    public async Task<UlipVahanRawResponse?> GetVehicleDetailsAsync(
        string vehicleNumber, CancellationToken ct = default)
    {
        try
        {
            var token = await GetTokenAsync(ct);
            if (token == null) return null;

            var request = BuildUlipRequest("VAHAN/01", new { vehiclenumber = vehicleNumber });
            AddAuthHeader(request, token);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            _logger.LogDebug("VAHAN response for {VehicleNumber}: {Json}", vehicleNumber, json);

            return ParseUlipResponse<UlipVahanRawResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "VAHAN lookup failed for {VehicleNumber}", vehicleNumber);
            return null;
        }
    }

    // ── SARATHI/01 ──────────────────────────────────────────────

    public async Task<UlipSarathiRawResponse?> VerifyDriverLicenseAsync(
        string licenseNumber, string dateOfBirth, CancellationToken ct = default)
    {
        try
        {
            var token = await GetTokenAsync(ct);
            if (token == null) return null;

            var request = BuildUlipRequest("SARATHI/01",
                new { dlnumber = licenseNumber, dob = dateOfBirth });
            AddAuthHeader(request, token);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            _logger.LogDebug("SARATHI response for {LicenseNumber}: {Json}", licenseNumber, json);

            return ParseUlipResponse<UlipSarathiRawResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SARATHI verification failed for {LicenseNumber}", licenseNumber);
            return null;
        }
    }

    // ── FASTAG/01 ───────────────────────────────────────────────

    public async Task<UlipFastagRawResponse?> GetFASTagTransactionsAsync(
        string vehicleNumber, CancellationToken ct = default)
    {
        try
        {
            var token = await GetTokenAsync(ct);
            if (token == null) return null;

            var request = BuildUlipRequest("FASTAG/01",
                new { vehiclenumber = vehicleNumber });
            AddAuthHeader(request, token);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            _logger.LogDebug("FASTAG response for {VehicleNumber}: {Json}", vehicleNumber, json);

            return ParseUlipResponse<UlipFastagRawResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FASTAG lookup failed for {VehicleNumber}", vehicleNumber);
            return null;
        }
    }

    // ── TOLL/01 ─────────────────────────────────────────────────

    public async Task<UlipTollRawResponse?> GetTollPlazaAsync(
        string plazaId, CancellationToken ct = default)
    {
        try
        {
            var token = await GetTokenAsync(ct);
            if (token == null) return null;

            var request = BuildUlipRequest("TOLL/01", new { plazaid = plazaId });
            AddAuthHeader(request, token);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            return ParseUlipResponse<UlipTollRawResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TOLL lookup failed for plaza {PlazaId}", plazaId);
            return null;
        }
    }

    // ── Health Check ────────────────────────────────────────────

    public async Task<UlipHealthDto> HealthCheckAsync(CancellationToken ct = default)
    {
        var health = new UlipHealthDto
        {
            StagingUrl = _httpClient.BaseAddress?.ToString()
        };

        try
        {
            var token = await GetTokenAsync(ct);
            health.IsConnected = true;
            health.HasValidToken = !string.IsNullOrEmpty(token);
            health.TokenExpiresAt = _tokenExpiresAt;
            health.Message = health.HasValidToken ? "ULIP connected and authenticated" : "Connected but authentication failed";
        }
        catch (Exception ex)
        {
            health.IsConnected = false;
            health.HasValidToken = false;
            health.Message = $"ULIP unreachable: {ex.Message}";
        }

        return health;
    }

    // ── Helpers ─────────────────────────────────────────────────

    private HttpRequestMessage BuildUlipRequest(string apiCode, object parameters)
    {
        // ULIP uses POST with api code + parameters envelope
        var payload = new
        {
            api = apiCode,
            parameters
        };

        var json = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, "ULIP/api")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return request;
    }

    private static void AddAuthHeader(HttpRequestMessage request, string token)
    {
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    private static T? ParseUlipResponse<T>(string json) where T : class
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("response", out var responseElement))
            {
                var responseJson = responseElement.GetRawText();
                return JsonSerializer.Deserialize<T>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            // Some ULIP APIs return data at root level
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }
}
