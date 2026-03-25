using System.Net.Http.Json;
using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services.Clients;

public class FreightClient : IFreightClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FreightClient> _logger;

    public FreightClient(IHttpClientFactory httpClientFactory, ILogger<FreightClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendCallbackAsync(Guid freightJobId, TransportCallbackPayload payload, CancellationToken ct = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FreightService");
            var response = await client.PostAsJsonAsync(
                $"internal/freight/jobs/{freightJobId}/transport-callback", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "FreightClient callback failed with {StatusCode} for freight job {FreightJobId}",
                    response.StatusCode, freightJobId);
            }
            else
            {
                _logger.LogInformation(
                    "FreightClient callback sent for freight job {FreightJobId}: Event={Event}",
                    freightJobId, payload.Event);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "FreightClient callback HTTP error for freight job {FreightJobId}", freightJobId);
        }
    }
}
