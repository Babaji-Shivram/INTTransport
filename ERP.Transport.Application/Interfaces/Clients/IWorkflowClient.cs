using ERP.Transport.Application.DTOs.Workflow;

namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Client for Workflow MS internal APIs — follows CRM's WorkflowClient pattern.
/// Polly retry + circuit breaker configured at DI level.
/// </summary>
public interface IWorkflowClient
{
    // ── Instance Management ─────────────────────────────────────
    Task<WorkflowInstanceResponseDto?> CreateInstanceAsync(CreateWorkflowInstanceDto request);
    Task<WorkflowInstanceResponseDto?> SubmitStepDataAsync(Guid instanceId, SubmitStepDataDto request);
    Task<WorkflowInstanceResponseDto?> AdvanceAsync(Guid instanceId, AdvanceWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> ApproveAsync(Guid instanceId, ApproveWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> RejectAsync(Guid instanceId, RejectWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> CancelAsync(Guid instanceId);

    // ── Metadata ────────────────────────────────────────────────
    Task<WorkflowStepDefinitionDto?> GetStepFieldsAsync(Guid stepId);
    Task<WorkflowStepFullDefinitionDto?> GetStepFullDefinitionAsync(Guid stepId);
    Task<WorkflowTemplateLookupDto?> LookupTemplateAsync(string templateCode, string countryCode);

    // ── Inbox ───────────────────────────────────────────────────
    Task<IEnumerable<WorkflowInboxItemDto>> GetInboxAsync(Guid userId);
}
