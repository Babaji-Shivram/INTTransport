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
/// Fleet vehicle master — company-owned/leased vehicle management.
/// </summary>
public interface IFleetVehicleService
{
    // ── Vehicle CRUD ────────────────────────────────────────────
    Task<FleetVehicleDto> CreateVehicleAsync(CreateFleetVehicleDto dto, Guid userId);
    Task<FleetVehicleDto?> GetVehicleByIdAsync(Guid id);
    Task<PagedResultDto<FleetVehicleListDto>> GetVehiclesAsync(FleetVehicleFilterDto filter);
    Task<FleetVehicleDto> UpdateVehicleAsync(Guid id, UpdateFleetVehicleDto dto, Guid userId);
    Task DeleteVehicleAsync(Guid id, Guid userId);

    // ── Available vehicles for assignment ────────────────────────
    Task<IEnumerable<FleetVehicleListDto>> GetAvailableVehiclesAsync(Guid branchId, string countryCode);

    // ── Driver management ───────────────────────────────────────
    Task<VehicleDriverDto> AssignDriverAsync(Guid vehicleId, AssignDriverDto dto, Guid userId);
    Task<VehicleDriverDto> UpdateDriverAsync(Guid vehicleId, Guid driverId, UpdateDriverDto dto, Guid userId);
    Task UnassignDriverAsync(Guid vehicleId, Guid driverId, Guid userId);

    // ── Daily status ────────────────────────────────────────────
    Task<VehicleDailyStatusDto> RecordDailyStatusAsync(Guid vehicleId, RecordDailyStatusDto dto, Guid userId);
    Task<IEnumerable<VehicleDailyStatusDto>> GetDailyStatusHistoryAsync(Guid vehicleId, DateTime? from, DateTime? to);

    // ── Compliance alerts ───────────────────────────────────────
    Task<IEnumerable<FleetVehicleListDto>> GetExpiringComplianceAsync(int daysAhead = 30);

    // ── Travel Log management ───────────────────────────────────
    Task<VehicleTravelLogDto> CreateTravelLogAsync(CreateVehicleTravelLogDto dto, Guid userId);
    Task<VehicleTravelLogDto?> GetTravelLogByIdAsync(Guid id);
    Task<PagedResultDto<VehicleTravelLogDto>> GetTravelLogsAsync(TravelLogFilterDto filter);
    Task<VehicleTravelLogDto> CompleteTripAsync(Guid logId, CompleteTripDto dto, Guid userId);
    Task<VehicleUsageSummaryDto> GetVehicleUsageSummaryAsync(Guid vehicleId, DateTime from, DateTime to);

    // ── Daily Expense management (Legacy: TransDailyExpense.aspx) ─
    Task<VehicleDailyExpenseDto> RecordDailyExpenseAsync(Guid vehicleId, CreateVehicleDailyExpenseDto dto, Guid userId);
    Task<VehicleDailyExpenseDto> UpdateDailyExpenseAsync(Guid vehicleId, Guid expenseId, UpdateVehicleDailyExpenseDto dto, Guid userId);
    Task DeleteDailyExpenseAsync(Guid vehicleId, Guid expenseId, Guid userId);
    Task<PagedResultDto<VehicleDailyExpenseDto>> GetDailyExpensesAsync(DailyExpenseFilterDto filter);
    Task<DailyExpenseAggregateDto> GetDailyExpenseAggregateAsync(Guid vehicleId, DateTime from, DateTime to);
}
