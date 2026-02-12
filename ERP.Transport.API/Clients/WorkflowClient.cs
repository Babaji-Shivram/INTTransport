using ERP.Transport.Application.DTOs.Workflow;
using ERP.Transport.Application.Interfaces.Clients;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ERP.Transport.API.Clients;

/// <summary>
/// Typed HttpClient calling Workflow MS — mirrors CRM's WorkflowClient.
/// Polly retry + circuit breaker configured at DI level.
/// </summary>
public class WorkflowClient : IWorkflowClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkflowClient> _logger;

    public WorkflowClient(HttpClient httpClient, ILogger<WorkflowClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // ── Instance Management ────────────────────────────────────

    public async Task<WorkflowInstanceResponseDto?> CreateInstanceAsync(
        CreateWorkflowInstanceDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/workflow/internal/instances", request);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create workflow instance for {TemplateCode}",
                request.TemplateCode);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> SubmitStepDataAsync(
        Guid instanceId, SubmitStepDataDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/v1/workflow/internal/instances/{instanceId}/submit", request);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit step data for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> AdvanceAsync(
        Guid instanceId, AdvanceWorkflowDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/v1/workflow/internal/instances/{instanceId}/advance", request);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to advance workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> ApproveAsync(
        Guid instanceId, ApproveWorkflowDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/v1/workflow/internal/instances/{instanceId}/approve", request);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> RejectAsync(
        Guid instanceId, RejectWorkflowDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/v1/workflow/internal/instances/{instanceId}/reject", request);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject workflow for instance {InstanceId}", instanceId);
            return null;
        }
    }

    public async Task<WorkflowInstanceResponseDto?> CancelAsync(Guid instanceId)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"api/v1/workflow/internal/instances/{instanceId}/cancel", null);
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WorkflowInstanceResponseDto>();
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
            return await _httpClient.GetFromJsonAsync<WorkflowStepDefinitionDto>(
                $"api/v1/workflow/internal/steps/{stepId}/fields");
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
            return await _httpClient.GetFromJsonAsync<WorkflowStepFullDefinitionDto>(
                $"api/v1/workflow/internal/steps/{stepId}/full-definition");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get step full definition for {StepId}", stepId);
            return null;
        }
    }

    public async Task<WorkflowTemplateLookupDto?> LookupTemplateAsync(
        string templateCode, string countryCode)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WorkflowTemplateLookupDto>(
                $"api/v1/workflow/internal/templates/lookup?code={Uri.EscapeDataString(templateCode)}&country={Uri.EscapeDataString(countryCode)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lookup template {TemplateCode}/{CountryCode}",
                templateCode, countryCode);
            return null;
        }
    }

    // ── Inbox ──────────────────────────────────────────────────

    public async Task<IEnumerable<WorkflowInboxItemDto>> GetInboxAsync(Guid userId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WorkflowInboxItemDto>>(
                $"api/v1/workflow/internal/inbox?userId={userId}");
            return result ?? Enumerable.Empty<WorkflowInboxItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get workflow inbox for user {UserId}", userId);
            return Enumerable.Empty<WorkflowInboxItemDto>();
        }
    }
}
