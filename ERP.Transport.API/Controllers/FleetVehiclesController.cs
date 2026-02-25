using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Extensions;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Fleet vehicle master — company-owned/leased vehicle management.
/// </summary>
public class FleetVehiclesController : TransportBaseController
{
    private readonly IFleetVehicleService _fleetService;

    public FleetVehiclesController(IFleetVehicleService fleetService)
    {
        _fleetService = fleetService;
    }

    // ── Vehicle CRUD ────────────────────────────────────────────

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FleetVehicleDto>>> Create(
        [FromBody] CreateFleetVehicleDto dto)
    {
        var result = await _fleetService.CreateVehicleAsync(dto, CurrentUserId);
        return OkResponse(result, "Fleet vehicle registered");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<FleetVehicleListDto>>>> GetAll(
        [FromQuery] FleetVehicleFilterDto filter)
    {
        var result = await _fleetService.GetVehiclesAsync(filter);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<FleetVehicleDto>>> GetById(Guid id)
    {
        var result = await _fleetService.GetVehicleByIdAsync(id);
        if (result == null)
            return NotFoundResponse<FleetVehicleDto>("Fleet vehicle not found");
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<FleetVehicleDto>>> Update(
        Guid id, [FromBody] UpdateFleetVehicleDto dto)
    {
        var result = await _fleetService.UpdateVehicleAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Fleet vehicle updated");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id)
    {
        await _fleetService.DeleteVehicleAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Fleet vehicle deactivated");
    }

    // ── Available vehicles ──────────────────────────────────────

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FleetVehicleListDto>>>> GetAvailable(
        [FromQuery] Guid branchId, [FromQuery] string countryCode = "IN")
    {
        var result = await _fleetService.GetAvailableVehiclesAsync(branchId, countryCode);
        return OkResponse(result);
    }

    // ── Compliance alerts ───────────────────────────────────────

    [HttpGet("expiring-compliance")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FleetVehicleListDto>>>> GetExpiringCompliance(
        [FromQuery] int daysAhead = 30)
    {
        var result = await _fleetService.GetExpiringComplianceAsync(daysAhead);
        return OkResponse(result);
    }

    // ── Driver management ───────────────────────────────────────

    [HttpPost("{id:guid}/drivers")]
    public async Task<ActionResult<ApiResponse<VehicleDriverDto>>> AssignDriver(
        Guid id, [FromBody] AssignDriverDto dto)
    {
        var result = await _fleetService.AssignDriverAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Driver assigned");
    }

    [HttpPut("{id:guid}/drivers/{driverId:guid}")]
    public async Task<ActionResult<ApiResponse<VehicleDriverDto>>> UpdateDriver(
        Guid id, Guid driverId, [FromBody] UpdateDriverDto dto)
    {
        var result = await _fleetService.UpdateDriverAsync(id, driverId, dto, CurrentUserId);
        return OkResponse(result, "Driver updated");
    }

    [HttpDelete("{id:guid}/drivers/{driverId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> UnassignDriver(Guid id, Guid driverId)
    {
        await _fleetService.UnassignDriverAsync(id, driverId, CurrentUserId);
        return OkResponse<object?>(null, "Driver unassigned");
    }

    // ── Daily status ────────────────────────────────────────────

    [HttpPost("{id:guid}/daily-status")]
    public async Task<ActionResult<ApiResponse<VehicleDailyStatusDto>>> RecordDailyStatus(
        Guid id, [FromBody] RecordDailyStatusDto dto)
    {
        var result = await _fleetService.RecordDailyStatusAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Daily status recorded");
    }

    [HttpGet("{id:guid}/daily-status")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleDailyStatusDto>>>> GetDailyStatusHistory(
        Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _fleetService.GetDailyStatusHistoryAsync(id, from, to);
        return OkResponse(result);
    }

    // ── Travel Log ──────────────────────────────────────────────

    [HttpPost("travel-logs")]
    public async Task<ActionResult<ApiResponse<VehicleTravelLogDto>>> CreateTravelLog(
        [FromBody] CreateVehicleTravelLogDto dto)
    {
        var result = await _fleetService.CreateTravelLogAsync(dto, CurrentUserId);
        return OkResponse(result, "Travel log created");
    }

    [HttpGet("travel-logs/{logId:guid}")]
    public async Task<ActionResult<ApiResponse<VehicleTravelLogDto>>> GetTravelLog(Guid logId)
    {
        var result = await _fleetService.GetTravelLogByIdAsync(logId);
        if (result == null)
            return NotFoundResponse<VehicleTravelLogDto>("Travel log not found");
        return OkResponse(result);
    }

    [HttpGet("travel-logs")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<VehicleTravelLogDto>>>> GetTravelLogs(
        [FromQuery] TravelLogFilterDto filter)
    {
        var result = await _fleetService.GetTravelLogsAsync(filter);
        return OkResponse(result);
    }

    [HttpPost("travel-logs/{logId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<VehicleTravelLogDto>>> CompleteTrip(
        Guid logId, [FromBody] CompleteTripDto dto)
    {
        var result = await _fleetService.CompleteTripAsync(logId, dto, CurrentUserId);
        return OkResponse(result, "Trip completed");
    }

    [HttpGet("{id:guid}/usage-summary")]
    public async Task<ActionResult<ApiResponse<VehicleUsageSummaryDto>>> GetUsageSummary(
        Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _fleetService.GetVehicleUsageSummaryAsync(id, from, to);
        return OkResponse(result);
    }

    // ── Daily Expenses (Legacy: VehicleDailyExpenseEntry.aspx) ──

    /// <summary>Record daily expense for a vehicle (15 fixed categories).</summary>
    [HttpPost("{id:guid}/daily-expenses")]
    public async Task<ActionResult<ApiResponse<VehicleDailyExpenseDto>>> RecordDailyExpense(
        Guid id, [FromBody] CreateVehicleDailyExpenseDto dto)
    {
        var result = await _fleetService.RecordDailyExpenseAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Daily expense recorded");
    }

    /// <summary>Update a daily expense entry.</summary>
    [HttpPut("{id:guid}/daily-expenses/{expenseId:guid}")]
    public async Task<ActionResult<ApiResponse<VehicleDailyExpenseDto>>> UpdateDailyExpense(
        Guid id, Guid expenseId, [FromBody] UpdateVehicleDailyExpenseDto dto)
    {
        var result = await _fleetService.UpdateDailyExpenseAsync(id, expenseId, dto, CurrentUserId);
        return OkResponse(result, "Daily expense updated");
    }

    /// <summary>Delete a daily expense entry.</summary>
    [HttpDelete("{id:guid}/daily-expenses/{expenseId:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteDailyExpense(Guid id, Guid expenseId)
    {
        await _fleetService.DeleteDailyExpenseAsync(id, expenseId, CurrentUserId);
        return OkResponse<object?>(null, "Daily expense deleted");
    }

    /// <summary>Get daily expenses for a vehicle (paged, with date filters).</summary>
    [HttpGet("{id:guid}/daily-expenses")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<VehicleDailyExpenseDto>>>> GetDailyExpenses(
        Guid id, [FromQuery] DailyExpenseFilterDto filter)
    {
        filter.FleetVehicleId = id;
        var result = await _fleetService.GetDailyExpensesAsync(filter);
        return OkResponse(result);
    }

    /// <summary>Get aggregated daily expense totals for a vehicle.</summary>
    [HttpGet("{id:guid}/daily-expenses/aggregate")]
    public async Task<ActionResult<ApiResponse<DailyExpenseAggregateDto>>> GetDailyExpenseAggregate(
        Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _fleetService.GetDailyExpenseAggregateAsync(id, from, to);
        return OkResponse(result);
    }
}
