using System.Text.Json;
using ERP.Transport.Application.DTOs.Workflow;
using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ERP.Transport.Application.Services.Clients;

/// <summary>
/// Typed HttpClient calling Workflow MS internal APIs.
/// Sends { UserId, Payload } wrapper for all mutations.
/// Unwraps ApiResponse&lt;T&gt; from Workflow responses.
/// Polly retry + circuit breaker configured at DI level.
/// </summary>
public class WorkflowClient : IWorkflowClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkflowClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public WorkflowClient(HttpClient httpClient, ILogger<WorkflowClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // ── Instance Management ────────────────────────────────────

    public async Task<WorkflowInstanceResponseDto?> CreateInstanceAsync(
        Guid userId, CreateWorkflowInstanceDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                "internal/workflow/instances", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "CreateInstance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create workflow instance for BusinessKey={BusinessKey}",
                request.BusinessKey);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> SubmitStepDataAsync(
        Guid instanceId, Guid userId, SubmitStepDataDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/submit-step", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "SubmitStepData");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit step data for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> AdvanceAsync(
        Guid instanceId, Guid userId, AdvanceWorkflowDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/advance", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "Advance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to advance workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> ApproveAsync(
        Guid instanceId, Guid userId, ApproveWorkflowDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/approve", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "Approve");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> RejectAsync(
        Guid instanceId, Guid userId, RejectWorkflowDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/reject", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "Reject");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> AssignAsync(
        Guid instanceId, Guid userId, AssignWorkflowDto request)
    {
        try
        {
            var body = WrapPayload(userId, request);
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/assign", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "Assign");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign workflow step for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> CancelAsync(
        Guid instanceId, Guid userId, CancelWorkflowDto? request = null)
    {
        try
        {
            var body = WrapPayload(userId, request ?? new CancelWorkflowDto());
            var response = await _httpClient.PostAsJsonAsync(
                $"internal/workflow/instances/{instanceId}/cancel", body, JsonOptions);
            return await ReadDataAsync<WorkflowInstanceResponseDto>(response, "Cancel");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    // ── Metadata ───────────────────────────────────────────────

    public async Task<WorkflowStepDefinitionDto?> GetStepFieldsAsync(Guid stepId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"internal/workflow/metadata/steps/{stepId}/fields");
            return await ReadDataAsync<WorkflowStepDefinitionDto>(response, "GetStepFields");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get step fields for {StepId}", stepId);
            return null;
        }
    }

    public async Task<WorkflowStepFullDefinitionDto?> GetStepFullDefinitionAsync(Guid stepId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"internal/workflow/metadata/steps/{stepId}/full-definition");
            return await ReadDataAsync<WorkflowStepFullDefinitionDto>(response, "GetStepFullDefinition");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get step full definition for {StepId}", stepId);
            return null;
        }
    }

    public async Task<WorkflowTemplateLookupDto?> LookupTemplateAsync(
        string module, string entity, string action, string? countryCode = null)
    {
        try
        {
            var request = new { Module = module, Entity = entity, Action = action, CountryCode = countryCode };
            var response = await _httpClient.PostAsJsonAsync(
                "internal/workflow/metadata/templates/lookup", request, JsonOptions);
            return await ReadDataAsync<WorkflowTemplateLookupDto>(response, "LookupTemplate");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lookup template {Module}/{Entity}/{Action}",
                module, entity, action);
            return null;
        }
    }

    // ── Inbox ──────────────────────────────────────────────────

    public async Task<IEnumerable<WorkflowInboxItemDto>> GetInboxAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"internal/workflow/inbox/{userId}");
            var result = await ReadDataAsync<List<WorkflowInboxItemDto>>(response, "GetInbox");
            return result ?? Enumerable.Empty<WorkflowInboxItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get workflow inbox for user {UserId}", userId);
            return Enumerable.Empty<WorkflowInboxItemDto>();
        }
    }

    // ── Helpers ─────────────────────────────────────────────────

    private static object WrapPayload<T>(Guid userId, T payload) =>
        new { UserId = userId, Payload = payload };

    private async Task<T?> ReadDataAsync<T>(HttpResponseMessage response, string operation)
    {
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Workflow {Operation} returned {StatusCode}", operation, response.StatusCode);
            return default;
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var root = doc.RootElement;
        if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean()
            && root.TryGetProperty("data", out var dataProp))
        {
            return JsonSerializer.Deserialize<T>(dataProp.GetRawText(), JsonOptions);
        }

        if (root.TryGetProperty("message", out var msgProp))
        {
            _logger.LogWarning("Workflow {Operation} failed: {Message}", operation, msgProp.GetString());
        }

        return default;
    }
}
