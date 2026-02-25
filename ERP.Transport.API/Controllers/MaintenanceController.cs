using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Fleet vehicle maintenance work orders.
/// </summary>
public class MaintenanceController : TransportBaseController
{
    private readonly IMaintenanceService _svc;

    public MaintenanceController(IMaintenanceService svc) => _svc = svc;

    // ════════════════════════════════════════════════════════════
    //  WORK ORDER CRUD
    // ════════════════════════════════════════════════════════════

    /// <summary>Create a maintenance work order</summary>
    [HttpPost("work-orders")]
    public async Task<ActionResult<ApiResponse<MaintenanceWorkOrderDto>>> CreateWorkOrder(
        [FromBody] CreateMaintenanceWorkOrderDto dto)
    {
        var result = await _svc.CreateWorkOrderAsync(dto, CurrentUserId);
        return OkResponse(result, "Work order created");
    }

    /// <summary>Get a work order by ID</summary>
    [HttpGet("work-orders/{id:guid}")]
    public async Task<ActionResult<ApiResponse<MaintenanceWorkOrderDto>>> GetWorkOrder(Guid id)
    {
        var result = await _svc.GetWorkOrderByIdAsync(id);
        if (result == null)
            return NotFoundResponse<MaintenanceWorkOrderDto>("Work order not found");
        return OkResponse(result);
    }

    /// <summary>Get paged work orders with filters</summary>
    [HttpGet("work-orders")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<MaintenanceWorkOrderListDto>>>> GetWorkOrders(
        [FromQuery] MaintenanceFilterDto filter)
    {
        return OkResponse(await _svc.GetWorkOrdersAsync(filter));
    }

    /// <summary>Update a work order</summary>
    [HttpPut("work-orders/{id:guid}")]
    public async Task<ActionResult<ApiResponse<MaintenanceWorkOrderDto>>> UpdateWorkOrder(
        Guid id, [FromBody] UpdateMaintenanceWorkOrderDto dto)
    {
        var result = await _svc.UpdateWorkOrderAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Work order updated");
    }

    /// <summary>Complete a work order</summary>
    [HttpPost("work-orders/{id:guid}/complete")]
    public async Task<ActionResult<ApiResponse<MaintenanceWorkOrderDto>>> CompleteWorkOrder(
        Guid id, [FromBody] CompleteMaintenanceDto dto)
    {
        var result = await _svc.CompleteWorkOrderAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Work order completed");
    }

    /// <summary>Cancel a work order</summary>
    [HttpPost("work-orders/{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<object?>>> CancelWorkOrder(Guid id)
    {
        await _svc.CancelWorkOrderAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Work order cancelled");
    }

    // ════════════════════════════════════════════════════════════
    //  PARTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Add a part to a work order</summary>
    [HttpPost("work-orders/{workOrderId:guid}/parts")]
    public async Task<ActionResult<ApiResponse<MaintenancePartDto>>> AddPart(
        Guid workOrderId, [FromBody] AddMaintenancePartDto dto)
    {
        var result = await _svc.AddPartAsync(workOrderId, dto, CurrentUserId);
        return OkResponse(result, "Part added");
    }

    /// <summary>Remove a part from a work order</summary>
    [HttpDelete("work-orders/{workOrderId:guid}/parts/{partId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeletePart(Guid workOrderId, Guid partId)
    {
        await _svc.DeletePartAsync(workOrderId, partId, CurrentUserId);
        return OkResponse<object?>(null, "Part removed");
    }

    // ════════════════════════════════════════════════════════════
    //  DOCUMENTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Add a document to a work order</summary>
    [HttpPost("work-orders/{workOrderId:guid}/documents")]
    public async Task<ActionResult<ApiResponse<MaintenanceDocumentDto>>> AddDocument(
        Guid workOrderId, [FromBody] CreateMaintenanceDocumentDto dto)
    {
        var result = await _svc.AddDocumentAsync(workOrderId, dto, CurrentUserId);
        return OkResponse(result, "Document added");
    }

    /// <summary>Get all documents for a work order</summary>
    [HttpGet("work-orders/{workOrderId:guid}/documents")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaintenanceDocumentDto>>>> GetDocuments(
        Guid workOrderId)
    {
        return OkResponse(await _svc.GetDocumentsAsync(workOrderId));
    }

    /// <summary>Delete a document from a work order</summary>
    [HttpDelete("work-orders/{workOrderId:guid}/documents/{documentId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteDocument(
        Guid workOrderId, Guid documentId)
    {
        await _svc.DeleteDocumentAsync(workOrderId, documentId, CurrentUserId);
        return OkResponse<object?>(null, "Document removed");
    }

    // ════════════════════════════════════════════════════════════
    //  QUERIES
    // ════════════════════════════════════════════════════════════

    /// <summary>Get upcoming maintenance (next N days)</summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaintenanceWorkOrderListDto>>>> GetUpcoming(
        [FromQuery] int daysAhead = 7)
    {
        return OkResponse(await _svc.GetUpcomingMaintenanceAsync(daysAhead));
    }

    /// <summary>Get overdue maintenance work orders</summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaintenanceWorkOrderListDto>>>> GetOverdue()
    {
        return OkResponse(await _svc.GetOverdueMaintenanceAsync());
    }

    /// <summary>Get maintenance history for a vehicle</summary>
    [HttpGet("vehicles/{vehicleId:guid}/history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaintenanceWorkOrderListDto>>>> GetVehicleHistory(
        Guid vehicleId)
    {
        return OkResponse(await _svc.GetVehicleMaintenanceHistoryAsync(vehicleId));
    }
}
