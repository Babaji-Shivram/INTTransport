using ERP.Transport.Application.DTOs.Workflow;

namespace ERP.Transport.Application.Interfaces.Clients;

/// <summary>
/// Client for Workflow MS internal APIs.
/// All mutation methods require userId for the { UserId, Payload } wrapper pattern.
/// Polly retry + circuit breaker configured at DI level.
/// </summary>
public interface IWorkflowClient
{
    // ── Instance Management ─────────────────────────────────────
    Task<WorkflowInstanceResponseDto?> CreateInstanceAsync(Guid userId, CreateWorkflowInstanceDto request);
    Task<WorkflowInstanceResponseDto?> SubmitStepDataAsync(Guid instanceId, Guid userId, SubmitStepDataDto request);
    Task<WorkflowInstanceResponseDto?> AdvanceAsync(Guid instanceId, Guid userId, AdvanceWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> ApproveAsync(Guid instanceId, Guid userId, ApproveWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> RejectAsync(Guid instanceId, Guid userId, RejectWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> AssignAsync(Guid instanceId, Guid userId, AssignWorkflowDto request);
    Task<WorkflowInstanceResponseDto?> CancelAsync(Guid instanceId, Guid userId, CancelWorkflowDto? request = null);

    // ── Metadata ────────────────────────────────────────────────
    Task<WorkflowStepDefinitionDto?> GetStepFieldsAsync(Guid stepId);
    Task<WorkflowStepFullDefinitionDto?> GetStepFullDefinitionAsync(Guid stepId);
    Task<WorkflowTemplateLookupDto?> LookupTemplateAsync(string module, string entity, string action, string? countryCode = null);

    // ── Inbox ───────────────────────────────────────────────────
    Task<IEnumerable<WorkflowInboxItemDto>> GetInboxAsync(Guid userId);
}
