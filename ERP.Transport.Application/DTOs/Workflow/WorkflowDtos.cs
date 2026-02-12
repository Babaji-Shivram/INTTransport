namespace ERP.Transport.Application.DTOs.Workflow;

// ═══════════════════════════════════════════════════════════════
//  Workflow client DTOs — mirrors Workflow MS internal API
// ═══════════════════════════════════════════════════════════════

public class CreateWorkflowInstanceDto
{
    public string TemplateCode { get; set; } = null!;
    public string BusinessKey { get; set; } = null!;
    public string? CountryCode { get; set; }
    public Guid? InitiatorUserId { get; set; }
}

public class SubmitStepDataDto
{
    public Guid StepId { get; set; }
    public Dictionary<string, object?> Data { get; set; } = new();
}

public class AdvanceWorkflowDto
{
    public Guid? UserId { get; set; }
    public string? Remarks { get; set; }
}

public class ApproveWorkflowDto
{
    public Guid StepId { get; set; }
    public Guid UserId { get; set; }
    public string? Remarks { get; set; }
}

public class RejectWorkflowDto
{
    public Guid StepId { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; } = null!;
}

// ── Response DTOs ───────────────────────────────────────────────

public class WorkflowInstanceResponseDto
{
    public Guid InstanceId { get; set; }
    public string? Status { get; set; }
    public Guid? CurrentStepId { get; set; }
    public string? CurrentStepCode { get; set; }
    public string? CurrentStepName { get; set; }
}

public class WorkflowStepDefinitionDto
{
    public Guid StepId { get; set; }
    public string StepCode { get; set; } = null!;
    public ICollection<WorkflowFieldDto> Fields { get; set; } = new List<WorkflowFieldDto>();
}

public class WorkflowStepFullDefinitionDto
{
    public Guid StepId { get; set; }
    public string StepCode { get; set; } = null!;
    public string StepName { get; set; } = null!;
    public string StepType { get; set; } = null!;
    public int? SlaHours { get; set; }
    public ICollection<WorkflowFieldDto> Fields { get; set; } = new List<WorkflowFieldDto>();
}

public class WorkflowFieldDto
{
    public string FieldKey { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string DataType { get; set; } = null!;
    public bool IsRequired { get; set; }
    public string? ValidationRules { get; set; }
    public string? Options { get; set; }
    public string? SourceApi { get; set; }
    public int DisplayOrder { get; set; }
}

public class WorkflowTemplateLookupDto
{
    public Guid TemplateId { get; set; }
    public string TemplateCode { get; set; } = null!;
    public string TemplateName { get; set; } = null!;
}

public class WorkflowInboxItemDto
{
    public Guid InstanceId { get; set; }
    public string BusinessKey { get; set; } = null!;
    public string? StepName { get; set; }
    public string? Status { get; set; }
    public DateTime? DueDate { get; set; }
}

// ── Callback DTOs ───────────────────────────────────────────────

/// <summary>
/// Callback from Workflow MS when a step is approved/rejected.
/// Posted to POST /internal/jobs/{id}/workflow-callback.
/// </summary>
public class WorkflowCallbackDto
{
    public Guid InstanceId { get; set; }
    public string BusinessKey { get; set; } = null!;
    public string Action { get; set; } = null!;  // "approved" | "rejected"
    public Guid? StepId { get; set; }
    public string? StepCode { get; set; }
    public Guid? ActorUserId { get; set; }
    public string? Remarks { get; set; }
    public string? NewStatus { get; set; }
}
