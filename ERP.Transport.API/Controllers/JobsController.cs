using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.DTOs.Workflow;
using ERP.Transport.Application.Interfaces;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Transport jobs — CRUD + full lifecycle.
/// </summary>
public class JobsController : TransportBaseController
{
    private readonly ITransportJobService _jobService;
    private readonly IElrService _elrService;

    public JobsController(ITransportJobService jobService, IElrService elrService)
    {
        _jobService = jobService;
        _elrService = elrService;
    }

    // ── CRUD ────────────────────────────────────────────────────

    /// <summary>Create a standalone transport job.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> Create(
        [FromBody] CreateTransportJobDto dto)
    {
        var userContext = HttpContext.GetRequiredUserContext();
        var countryCode = userContext.Countries?.FirstOrDefault() ?? "IN";
        var branchId = Guid.TryParse(
            userContext.Branches?.FirstOrDefault(), out var bid) ? bid : Guid.Empty;

        var result = await _jobService.CreateJobAsync(dto, CurrentUserId, countryCode, branchId);
        return OkResponse(result, "Transport job created successfully");
    }

    /// <summary>Create from CRM enquiry.</summary>
    [HttpPost("from-enquiry")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> CreateFromEnquiry(
        [FromBody] CreateJobFromEnquiryDto dto)
    {
        var result = await _jobService.CreateJobFromEnquiryAsync(dto, CurrentUserId);
        return OkResponse(result, "Transport job created from enquiry");
    }

    /// <summary>Create from freight job.</summary>
    [HttpPost("from-freight")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> CreateFromFreight(
        [FromBody] CreateJobFromFreightDto dto)
    {
        var result = await _jobService.CreateJobFromFreightAsync(dto, CurrentUserId);
        return OkResponse(result, "Transport job created from freight job");
    }

    /// <summary>Get all jobs (filtered, paginated).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<TransportJobListDto>>>> GetAll(
        [FromQuery] TransportJobFilterDto filter)
    {
        var result = await _jobService.GetJobsAsync(filter);
        return OkResponse(result);
    }

    /// <summary>Get job details by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> GetById(Guid id)
    {
        var result = await _jobService.GetJobByIdAsync(id);
        if (result == null)
            return NotFoundResponse<TransportJobDto>("Transport job not found");
        return OkResponse(result);
    }

    /// <summary>Update job details (allowed before vehicle assignment).</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> Update(
        Guid id, [FromBody] UpdateTransportJobDto dto)
    {
        var result = await _jobService.UpdateJobAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Transport job updated");
    }

    /// <summary>Cancel/delete a job.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id)
    {
        await _jobService.DeleteJobAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Transport job cancelled");
    }

    // ── Lifecycle ───────────────────────────────────────────────

    /// <summary>Mark request as received by operations.</summary>
    [HttpPost("{id:guid}/receive")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> Receive(Guid id)
    {
        var result = await _jobService.ReceiveJobAsync(id, CurrentUserId);
        return OkResponse(result, "Request received");
    }

    /// <summary>Assign a vehicle to the job.</summary>
    [HttpPost("{id:guid}/vehicles")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> AssignVehicle(
        Guid id, [FromBody] AssignVehicleDto dto)
    {
        var result = await _jobService.AssignVehicleAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Vehicle assigned");
    }

    /// <summary>Update a vehicle assignment.</summary>
    [HttpPut("{id:guid}/vehicles/{vehicleId:guid}")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> UpdateVehicle(
        Guid id, Guid vehicleId, [FromBody] UpdateVehicleAssignmentDto dto)
    {
        var result = await _jobService.UpdateVehicleAssignmentAsync(
            id, vehicleId, dto, CurrentUserId);
        return OkResponse(result, "Vehicle assignment updated");
    }

    /// <summary>Enter rate for a vehicle.</summary>
    [HttpPost("{id:guid}/vehicles/{vehicleId:guid}/rate")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> EnterRate(
        Guid id, Guid vehicleId, [FromBody] EnterRateDto dto)
    {
        var result = await _jobService.EnterRateAsync(id, vehicleId, dto, CurrentUserId);
        return OkResponse(result, "Rate entered");
    }

    /// <summary>Submit rate for approval.</summary>
    [HttpPost("{id:guid}/rate/submit-approval")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> SubmitRateForApproval(Guid id)
    {
        var result = await _jobService.SubmitRateForApprovalAsync(id, CurrentUserId);
        return OkResponse(result, "Rate submitted for approval");
    }

    /// <summary>Add a movement/location update.</summary>
    [HttpPost("{id:guid}/movements")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> AddMovement(
        Guid id, [FromBody] AddMovementDto dto)
    {
        var result = await _jobService.AddMovementAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Movement recorded");
    }

    /// <summary>Record delivery + POD.</summary>
    [HttpPost("{id:guid}/delivery")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> RecordDelivery(
        Guid id, [FromBody] RecordDeliveryDto dto)
    {
        var result = await _jobService.RecordDeliveryAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Delivery recorded");
    }

    /// <summary>Clear the job (final step).</summary>
    [HttpPost("{id:guid}/clearance")]
    public async Task<ActionResult<ApiResponse<TransportJobDto>>> Clear(Guid id)
    {
        var result = await _jobService.ClearJobAsync(id, CurrentUserId);
        return OkResponse(result, "Job cleared — ready for billing");
    }

    // ── Documents ───────────────────────────────────────────────

    /// <summary>Upload a document to the job.</summary>
    [HttpPost("{id:guid}/documents")]
    public async Task<ActionResult<ApiResponse<TransportDocumentDto>>> UploadDocument(
        Guid id, [FromBody] UploadDocumentDto dto)
    {
        var result = await _jobService.UploadDocumentAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Document uploaded");
    }

    // ── Timeline ────────────────────────────────────────────────

    /// <summary>Get full job history/timeline.</summary>
    [HttpGet("{id:guid}/timeline")]
    public async Task<ActionResult<ApiResponse<JobTimelineDto>>> GetTimeline(Guid id)
    {
        var result = await _jobService.GetJobTimelineAsync(id);
        return OkResponse(result);
    }

    // ── Queue ───────────────────────────────────────────────────

    /// <summary>Get operation request queue.</summary>
    [HttpGet("queue")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportJobListDto>>>> GetQueue(
        [FromQuery] TransportJobFilterDto filter)
    {
        var result = await _jobService.GetQueueAsync(filter);
        return OkResponse(result);
    }

    /// <summary>Get jobs pending rate approval.</summary>
    [HttpGet("pending-approval")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportJobListDto>>>> GetPendingApproval()
    {
        var result = await _jobService.GetPendingApprovalAsync(CurrentUserId);
        return OkResponse(result);
    }

    // ── Consolidation ───────────────────────────────────────────

    /// <summary>Consolidate multiple jobs into one trip.</summary>
    [HttpPost("consolidate")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> Consolidate(
        [FromBody] ConsolidateJobsDto dto)
    {
        var result = await _jobService.ConsolidateJobsAsync(dto, CurrentUserId);
        return OkResponse(result, "Jobs consolidated");
    }

    /// <summary>Get a consolidated trip by ID.</summary>
    [HttpGet("consolidated-trips/{tripId:guid}")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> GetConsolidatedTrip(Guid tripId)
    {
        var result = await _jobService.GetConsolidatedTripAsync(tripId);
        if (result == null)
            return NotFoundResponse<ConsolidatedTripDto>("Consolidated trip not found");
        return OkResponse(result);
    }

    /// <summary>Assign a shared vehicle to a consolidated trip.</summary>
    [HttpPut("consolidated-trips/{tripId:guid}/vehicle")]
    public async Task<ActionResult<ApiResponse<ConsolidatedTripDto>>> AssignConsolidatedVehicle(
        Guid tripId, [FromBody] AssignConsolidatedVehicleDto dto)
    {
        var result = await _jobService.AssignConsolidatedVehicleAsync(tripId, dto, CurrentUserId);
        return OkResponse(result, "Shared vehicle assigned to consolidated trip");
    }

    /// <summary>Add a vehicle to a consolidated trip (multi-vehicle support).</summary>
    [HttpPost("consolidated-trips/{tripId:guid}/vehicles")]
    public async Task<ActionResult<ApiResponse<ConsolidatedVehicleDto>>> AddConsolidatedVehicle(
        Guid tripId, [FromBody] CreateConsolidatedVehicleDto dto)
    {
        var result = await _jobService.AddConsolidatedVehicleAsync(tripId, dto, CurrentUserId);
        return OkResponse(result, "Vehicle added to consolidated trip");
    }

    /// <summary>Add a shared expense to a consolidated trip.</summary>
    [HttpPost("consolidated-trips/{tripId:guid}/expenses")]
    public async Task<ActionResult<ApiResponse<ConsolidatedExpenseDto>>> AddConsolidatedExpense(
        Guid tripId, [FromBody] CreateConsolidatedExpenseDto dto)
    {
        var result = await _jobService.AddConsolidatedExpenseAsync(tripId, dto, CurrentUserId);
        return OkResponse(result, "Expense added to consolidated trip");
    }

    /// <summary>Get all shared expenses for a consolidated trip.</summary>
    [HttpGet("consolidated-trips/{tripId:guid}/expenses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ConsolidatedExpenseDto>>>> GetConsolidatedExpenses(Guid tripId)
    {
        var result = await _jobService.GetConsolidatedExpensesAsync(tripId);
        return OkResponse(result);
    }

    /// <summary>Record delivery at a stop within a consolidated trip.</summary>
    [HttpPut("consolidated-trips/{tripId:guid}/stops/{stopId:guid}/delivery")]
    public async Task<ActionResult<ApiResponse<ConsolidatedStopDeliveryDto>>> RecordStopDelivery(
        Guid tripId, Guid stopId, [FromBody] RecordStopDeliveryDto dto)
    {
        var result = await _jobService.RecordStopDeliveryAsync(tripId, stopId, dto, CurrentUserId);
        return OkResponse(result, "Stop delivery recorded");
    }

    // ── Bulk Operations ─────────────────────────────────────────

    /// <summary>Bulk-receive multiple jobs (mark RequestCreated → RequestReceived).</summary>
    [HttpPost("bulk-receive")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportJobListDto>>>> BulkReceive(
        [FromBody] ICollection<Guid> jobIds)
    {
        var result = await _jobService.BulkReceiveAsync(jobIds, CurrentUserId);
        return OkResponse(result, $"{result.Count()} jobs received");
    }

    /// <summary>Bulk-clear multiple delivered jobs (validates clearance checklist for each).</summary>
    [HttpPost("bulk-clearance")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportJobListDto>>>> BulkClear(
        [FromBody] ICollection<Guid> jobIds)
    {
        var result = await _jobService.BulkClearAsync(jobIds, CurrentUserId);
        return OkResponse(result, $"{result.Count()} jobs cleared");
    }

    // ── Expenses ────────────────────────────────────────────────

    /// <summary>Get all expenses for a job.</summary>
    [HttpGet("{id:guid}/expenses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportExpenseDto>>>> GetExpenses(Guid id)
    {
        var result = await _jobService.GetExpensesAsync(id);
        return OkResponse(result);
    }

    /// <summary>Get expense summary / breakdown for a job.</summary>
    [HttpGet("{id:guid}/expenses/summary")]
    public async Task<ActionResult<ApiResponse<ExpenseSummaryDto>>> GetExpenseSummary(Guid id)
    {
        var result = await _jobService.GetExpenseSummaryAsync(id);
        return OkResponse(result);
    }

    /// <summary>Add a trip expense to a job.</summary>
    [HttpPost("{id:guid}/expenses")]
    public async Task<ActionResult<ApiResponse<TransportExpenseDto>>> AddExpense(
        Guid id, [FromBody] CreateExpenseDto dto)
    {
        var result = await _jobService.AddExpenseAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Expense added");
    }

    /// <summary>Update a trip expense.</summary>
    [HttpPut("{id:guid}/expenses/{expenseId:guid}")]
    public async Task<ActionResult<ApiResponse<TransportExpenseDto>>> UpdateExpense(
        Guid id, Guid expenseId, [FromBody] UpdateExpenseDto dto)
    {
        var result = await _jobService.UpdateExpenseAsync(id, expenseId, dto, CurrentUserId);
        return OkResponse(result, "Expense updated");
    }

    /// <summary>Delete a trip expense.</summary>
    [HttpDelete("{id:guid}/expenses/{expenseId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteExpense(Guid id, Guid expenseId)
    {
        await _jobService.DeleteExpenseAsync(id, expenseId, CurrentUserId);
        return OkResponse<object?>(null, "Expense deleted");
    }

    // ── Fund Requests ───────────────────────────────────────────

    /// <summary>Get all fund requests for a job.</summary>
    [HttpGet("{id:guid}/fund-requests")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleFundRequestDto>>>> GetFundRequests(Guid id)
    {
        var result = await _jobService.GetFundRequestsAsync(id);
        return OkResponse(result);
    }

    /// <summary>Create fund request for a vehicle on a job.</summary>
    [HttpPost("{id:guid}/vehicles/{vehicleId:guid}/fund-request")]
    public async Task<ActionResult<ApiResponse<VehicleFundRequestDto>>> CreateFundRequest(
        Guid id, Guid vehicleId, [FromBody] CreateFundRequestDto dto)
    {
        var result = await _jobService.CreateFundRequestAsync(id, vehicleId, dto, CurrentUserId);
        return OkResponse(result, "Fund request created");
    }

    /// <summary>Approve / Reject / Process a fund request.</summary>
    [HttpPut("fund-requests/{fundRequestId:guid}/process")]
    public async Task<ActionResult<ApiResponse<VehicleFundRequestDto>>> ProcessFundRequest(
        Guid fundRequestId, [FromBody] ProcessFundRequestDto dto)
    {
        var result = await _jobService.ProcessFundRequestAsync(fundRequestId, dto, CurrentUserId);
        return OkResponse(result, $"Fund request {dto.Action}");
    }

    // ── Clearance Checklist ─────────────────────────────────────

    /// <summary>Get pre-clearance checklist for a job.</summary>
    [HttpGet("{id:guid}/clearance-checklist")]
    public async Task<ActionResult<ApiResponse<ClearanceChecklistDto>>> GetClearanceChecklist(Guid id)
    {
        var result = await _jobService.GetClearanceChecklistAsync(id);
        return OkResponse(result);
    }

    // ── ELR (Electronic Lorry Receipt) ────────────────────────

    /// <summary>Generate ELR PDF for a transport job.</summary>
    [HttpGet("{id:guid}/elr")]
    [Produces("application/pdf")]
    public async Task<IActionResult> GenerateElr(Guid id)
    {
        var (pdfBytes, fileName) = await _elrService.GenerateElrPdfAsync(id);
        return File(pdfBytes, "application/pdf", fileName);
    }

    // ── Workflow Callback (internal) ────────────────────────────

    /// <summary>Callback from Workflow MS on step approval/rejection.</summary>
    [HttpPost("{id:guid}/workflow-callback")]
    [Tags("Internal")]
    public async Task<ActionResult<ApiResponse<object?>>> WorkflowCallback(
        Guid id, [FromBody] WorkflowCallbackDto dto)
    {
        await _jobService.HandleWorkflowCallbackAsync(id, dto);
        return OkResponse<object?>(null, "Workflow callback processed");
    }

    // ── Transit Warehouse (Legacy: InTransitWarehouse.aspx) ─────

    /// <summary>Record arrival at a transit warehouse.</summary>
    [HttpPost("{id:guid}/warehouse-arrival")]
    public async Task<ActionResult<ApiResponse<TransitWarehouseDto>>> ArriveAtWarehouse(
        Guid id, [FromBody] ArriveAtWarehouseDto dto)
    {
        var result = await _jobService.ArriveAtWarehouseAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Arrived at warehouse");
    }

    /// <summary>Dispatch from a transit warehouse.</summary>
    [HttpPost("{id:guid}/warehouses/{warehouseId:guid}/dispatch")]
    public async Task<ActionResult<ApiResponse<TransitWarehouseDto>>> DispatchFromWarehouse(
        Guid id, Guid warehouseId, [FromBody] DispatchFromWarehouseDto dto)
    {
        var result = await _jobService.DispatchFromWarehouseAsync(id, warehouseId, dto, CurrentUserId);
        return OkResponse(result, "Dispatched from warehouse");
    }

    /// <summary>Get all transit warehouses for a job.</summary>
    [HttpGet("{id:guid}/warehouses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransitWarehouseDto>>>> GetTransitWarehouses(Guid id)
    {
        var result = await _jobService.GetTransitWarehousesAsync(id);
        return OkResponse(result);
    }

    // ── Expense Approval (Legacy: ApproveExpense.aspx) ──────────

    /// <summary>Submit an expense for HOD approval.</summary>
    [HttpPost("{id:guid}/expenses/{expenseId:guid}/submit-approval")]
    public async Task<ActionResult<ApiResponse<ExpenseApprovalDto>>> SubmitExpenseForApproval(
        Guid id, Guid expenseId, [FromBody] SubmitExpenseForApprovalDto dto)
    {
        var result = await _jobService.SubmitExpenseForApprovalAsync(id, expenseId, dto, CurrentUserId);
        return OkResponse(result, "Expense submitted for approval");
    }

    /// <summary>Approve an expense.</summary>
    [HttpPut("expenses/{expenseId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<ExpenseApprovalDto>>> ApproveExpense(
        Guid expenseId, [FromBody] ApproveExpenseDto dto)
    {
        var result = await _jobService.ApproveExpenseAsync(expenseId, dto, CurrentUserId);
        return OkResponse(result, "Expense approved");
    }

    /// <summary>Reject an expense.</summary>
    [HttpPut("expenses/{expenseId:guid}/reject")]
    public async Task<ActionResult<ApiResponse<ExpenseApprovalDto>>> RejectExpense(
        Guid expenseId, [FromBody] RejectExpenseDto dto)
    {
        var result = await _jobService.RejectExpenseAsync(expenseId, dto, CurrentUserId);
        return OkResponse(result, "Expense rejected");
    }

    /// <summary>Get all expenses pending approval.</summary>
    [HttpGet("expenses/pending-approvals")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PendingExpenseApprovalDto>>>> GetPendingExpenseApprovals()
    {
        var result = await _jobService.GetPendingExpenseApprovalsAsync(CurrentUserId);
        return OkResponse(result);
    }
}
