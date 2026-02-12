using ERP.Transport.Application.DTOs;
using ERP.Transport.Application.DTOs.Workflow;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// Transport job service — core CRUD + lifecycle operations.
/// </summary>
public interface ITransportJobService
{
    // ── CRUD ────────────────────────────────────────────────────
    Task<TransportJobDto> CreateJobAsync(CreateTransportJobDto dto, Guid userId, string countryCode, Guid branchId);
    Task<TransportJobDto> CreateJobFromEnquiryAsync(CreateJobFromEnquiryDto dto, Guid userId);
    Task<TransportJobDto> CreateJobFromFreightAsync(CreateJobFromFreightDto dto, Guid userId);
    Task<TransportJobDto?> GetJobByIdAsync(Guid id);
    Task<TransportJobDto> UpdateJobAsync(Guid id, UpdateTransportJobDto dto, Guid userId);
    Task DeleteJobAsync(Guid id, Guid userId);

    // ── Lifecycle ───────────────────────────────────────────────
    Task<TransportJobDto> ReceiveJobAsync(Guid id, Guid userId);
    Task<TransportJobDto> AssignVehicleAsync(Guid jobId, AssignVehicleDto dto, Guid userId);
    Task<TransportJobDto> UpdateVehicleAssignmentAsync(Guid jobId, Guid vehicleId, UpdateVehicleAssignmentDto dto, Guid userId);
    Task<TransportJobDto> EnterRateAsync(Guid jobId, Guid vehicleId, EnterRateDto dto, Guid userId);
    Task<TransportJobDto> SubmitRateForApprovalAsync(Guid jobId, Guid userId);
    Task<TransportJobDto> AddMovementAsync(Guid jobId, AddMovementDto dto, Guid userId);
    Task<TransportJobDto> RecordDeliveryAsync(Guid jobId, RecordDeliveryDto dto, Guid userId);
    Task<TransportJobDto> ClearJobAsync(Guid jobId, Guid userId);

    // ── Expenses ────────────────────────────────────────────────
    Task<IEnumerable<TransportExpenseDto>> GetExpensesAsync(Guid jobId);
    Task<ExpenseSummaryDto> GetExpenseSummaryAsync(Guid jobId);
    Task<TransportExpenseDto> AddExpenseAsync(Guid jobId, CreateExpenseDto dto, Guid userId);
    Task<TransportExpenseDto> UpdateExpenseAsync(Guid jobId, Guid expenseId, UpdateExpenseDto dto, Guid userId);
    Task DeleteExpenseAsync(Guid jobId, Guid expenseId, Guid userId);

    // ── Fund Requests ───────────────────────────────────────────
    Task<IEnumerable<VehicleFundRequestDto>> GetFundRequestsAsync(Guid jobId);
    Task<VehicleFundRequestDto> CreateFundRequestAsync(Guid jobId, Guid vehicleId, CreateFundRequestDto dto, Guid userId);
    Task<VehicleFundRequestDto> ProcessFundRequestAsync(Guid fundRequestId, ProcessFundRequestDto dto, Guid userId);

    // ── Clearance Checklist ─────────────────────────────────────
    Task<ClearanceChecklistDto> GetClearanceChecklistAsync(Guid jobId);

    // ── Workflow Callback ───────────────────────────────────────
    Task HandleWorkflowCallbackAsync(Guid jobId, WorkflowCallbackDto dto);

    // ── Query ───────────────────────────────────────────────────
    Task<PagedResultDto<TransportJobListDto>> GetJobsAsync(TransportJobFilterDto filter);
    Task<IEnumerable<TransportJobListDto>> GetQueueAsync(TransportJobFilterDto filter);
    Task<IEnumerable<TransportJobListDto>> GetPendingApprovalAsync(Guid userId);
    Task<JobTimelineDto> GetJobTimelineAsync(Guid jobId);

    // ── Documents ───────────────────────────────────────────────
    Task<TransportDocumentDto> UploadDocumentAsync(Guid jobId, UploadDocumentDto dto, Guid userId);

    // ── Consolidation ───────────────────────────────────────────
    Task<ConsolidatedTripDto> ConsolidateJobsAsync(ConsolidateJobsDto dto, Guid userId);
    Task<ConsolidatedTripDto?> GetConsolidatedTripAsync(Guid tripId);
    Task<ConsolidatedTripDto> AssignConsolidatedVehicleAsync(Guid tripId, AssignConsolidatedVehicleDto dto, Guid userId);
    Task<ConsolidatedVehicleDto> AddConsolidatedVehicleAsync(Guid tripId, CreateConsolidatedVehicleDto dto, Guid userId);
    Task<ConsolidatedExpenseDto> AddConsolidatedExpenseAsync(Guid tripId, CreateConsolidatedExpenseDto dto, Guid userId);
    Task<IEnumerable<ConsolidatedExpenseDto>> GetConsolidatedExpensesAsync(Guid tripId);
    Task<ConsolidatedStopDeliveryDto> RecordStopDeliveryAsync(Guid tripId, Guid stopId, RecordStopDeliveryDto dto, Guid userId);

    // ── Bulk Operations ─────────────────────────────────────────
    Task<IEnumerable<TransportJobListDto>> BulkReceiveAsync(ICollection<Guid> jobIds, Guid userId);
    Task<IEnumerable<TransportJobListDto>> BulkClearAsync(ICollection<Guid> jobIds, Guid userId);

    // ── Transit Warehouse (Legacy: InTransitWarehouse.aspx) ─────
    Task<TransitWarehouseDto> ArriveAtWarehouseAsync(Guid jobId, ArriveAtWarehouseDto dto, Guid userId);
    Task<TransitWarehouseDto> DispatchFromWarehouseAsync(Guid jobId, Guid warehouseId, DispatchFromWarehouseDto dto, Guid userId);
    Task<IEnumerable<TransitWarehouseDto>> GetTransitWarehousesAsync(Guid jobId);

    // ── Expense Approval (Legacy: ApproveExpense.aspx) ──────────
    Task<ExpenseApprovalDto> SubmitExpenseForApprovalAsync(Guid jobId, Guid expenseId, SubmitExpenseForApprovalDto dto, Guid userId);
    Task<ExpenseApprovalDto> ApproveExpenseAsync(Guid expenseId, ApproveExpenseDto dto, Guid userId);
    Task<ExpenseApprovalDto> RejectExpenseAsync(Guid expenseId, RejectExpenseDto dto, Guid userId);
    Task<IEnumerable<PendingExpenseApprovalDto>> GetPendingExpenseApprovalsAsync(Guid userId);

    // ── Internal (MS-to-MS) ─────────────────────────────────────
    Task<TransportJobStatusDto> GetJobStatusAsync(Guid jobId);
}
