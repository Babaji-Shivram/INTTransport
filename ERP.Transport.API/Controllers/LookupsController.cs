using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Domain.Enums;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Transport lookup / master table management.
/// Provides CRUD for configurable code-value pairs
/// (VehicleType, ExpenseCategory, VehicleStatus, MaintenanceCategory).
/// </summary>
public class LookupsController : TransportBaseController
{
    private readonly ILookupService _svc;

    public LookupsController(ILookupService svc) => _svc = svc;

    // ════════════════════════════════════════════════════════════
    //  CRUD
    // ════════════════════════════════════════════════════════════

    /// <summary>Create a lookup entry</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransportLookupDto>>> Create(
        [FromBody] CreateTransportLookupDto dto)
    {
        var result = await _svc.CreateAsync(dto, CurrentUserId);
        return OkResponse(result, "Lookup created");
    }

    /// <summary>Get a lookup by ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransportLookupDto>>> GetById(Guid id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null)
            return NotFoundResponse<TransportLookupDto>("Lookup not found");
        return OkResponse(result);
    }

    /// <summary>Get all lookups for a category (e.g. VehicleType, ExpenseCategory)</summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> GetByCategory(
        LookupCategory category, [FromQuery] string? countryCode = null, [FromQuery] bool activeOnly = true)
    {
        return OkResponse(await _svc.GetByCategoryAsync(category, countryCode, activeOnly));
    }

    /// <summary>Search lookups with filters</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> Search(
        [FromQuery] LookupFilterDto filter)
    {
        return OkResponse(await _svc.SearchAsync(filter));
    }

    /// <summary>Update a lookup entry</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransportLookupDto>>> Update(
        Guid id, [FromBody] UpdateTransportLookupDto dto)
    {
        var result = await _svc.UpdateAsync(id, dto, CurrentUserId);
        return OkResponse(result, "Lookup updated");
    }

    /// <summary>Delete a lookup entry</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id)
    {
        await _svc.DeleteAsync(id, CurrentUserId);
        return OkResponse<object?>(null, "Lookup deleted");
    }

    /// <summary>Seed default lookup values (idempotent)</summary>
    [HttpPost("seed")]
    public async Task<ActionResult<ApiResponse<object?>>> Seed()
    {
        await _svc.SeedDefaultsAsync(CurrentUserId);
        return OkResponse<object?>(null, "Default lookups seeded");
    }

    // ════════════════════════════════════════════════════════════
    //  CONVENIENCE SHORTCUTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Get all vehicle types</summary>
    [HttpGet("vehicle-types")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> GetVehicleTypes(
        [FromQuery] string? countryCode = null)
    {
        return OkResponse(await _svc.GetByCategoryAsync(LookupCategory.VehicleType, countryCode));
    }

    /// <summary>Get all expense categories</summary>
    [HttpGet("expense-categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> GetExpenseCategories(
        [FromQuery] string? countryCode = null)
    {
        return OkResponse(await _svc.GetByCategoryAsync(LookupCategory.ExpenseCategory, countryCode));
    }

    /// <summary>Get all vehicle statuses</summary>
    [HttpGet("vehicle-statuses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> GetVehicleStatuses()
    {
        return OkResponse(await _svc.GetByCategoryAsync(LookupCategory.VehicleStatus));
    }

    /// <summary>Get all maintenance categories</summary>
    [HttpGet("maintenance-categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TransportLookupDto>>>> GetMaintenanceCategories()
    {
        return OkResponse(await _svc.GetByCategoryAsync(LookupCategory.MaintenanceCategory));
    }
}
