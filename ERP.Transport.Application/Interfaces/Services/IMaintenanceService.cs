using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Warehouse;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Maintenance work order service — CRUD + parts tracking.
/// </summary>
public interface IMaintenanceService
{
    // ── Work Order CRUD ─────────────────────────────────────────
    Task<MaintenanceWorkOrderDto> CreateWorkOrderAsync(CreateMaintenanceWorkOrderDto dto, Guid userId);
    Task<MaintenanceWorkOrderDto?> GetWorkOrderByIdAsync(Guid id);
    Task<PagedResultDto<MaintenanceWorkOrderListDto>> GetWorkOrdersAsync(MaintenanceFilterDto filter);
    Task<MaintenanceWorkOrderDto> UpdateWorkOrderAsync(Guid id, UpdateMaintenanceWorkOrderDto dto, Guid userId);
    Task<MaintenanceWorkOrderDto> CompleteWorkOrderAsync(Guid id, CompleteMaintenanceDto dto, Guid userId);
    Task CancelWorkOrderAsync(Guid id, Guid userId);

    // ── Parts Management ────────────────────────────────────────
    Task<MaintenancePartDto> AddPartAsync(Guid workOrderId, AddMaintenancePartDto dto, Guid userId);
    Task DeletePartAsync(Guid workOrderId, Guid partId, Guid userId);

    // ── Documents Management ────────────────────────────────────
    Task<MaintenanceDocumentDto> AddDocumentAsync(Guid workOrderId, CreateMaintenanceDocumentDto dto, Guid userId);
    Task<IEnumerable<MaintenanceDocumentDto>> GetDocumentsAsync(Guid workOrderId);
    Task DeleteDocumentAsync(Guid workOrderId, Guid documentId, Guid userId);

    // ── Queries ─────────────────────────────────────────────────
    Task<IEnumerable<MaintenanceWorkOrderListDto>> GetUpcomingMaintenanceAsync(int daysAhead = 7);
    Task<IEnumerable<MaintenanceWorkOrderListDto>> GetOverdueMaintenanceAsync();
    Task<IEnumerable<MaintenanceWorkOrderListDto>> GetVehicleMaintenanceHistoryAsync(Guid vehicleId);
}
