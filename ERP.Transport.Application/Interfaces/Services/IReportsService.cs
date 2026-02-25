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
/// Transport reports service contract.
/// </summary>
public interface IReportsService
{
    // ════════════════════════════════════════════════════════════
    //  OPERATIONAL REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Job summary report (counts, revenue, by status)</summary>
    Task<JobSummaryReportDto> GetJobSummaryAsync(ReportFilterDto filter);

    /// <summary>Daily operations report</summary>
    Task<DailyOperationsReportDto> GetDailyOperationsAsync(DateTime date, string? branchId = null);

    /// <summary>Delivery performance metrics</summary>
    Task<DeliveryPerformanceReportDto> GetDeliveryPerformanceAsync(ReportFilterDto filter);

    /// <summary>Route analysis (top routes, delay patterns)</summary>
    Task<RouteAnalysisReportDto> GetRouteAnalysisAsync(ReportFilterDto filter);

    /// <summary>Empty leg tracking</summary>
    Task<EmptyLegReportDto> GetEmptyLegReportAsync(ReportFilterDto filter);

    // ════════════════════════════════════════════════════════════
    //  FINANCIAL REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Expense analysis (by category, top items)</summary>
    Task<ExpenseAnalysisReportDto> GetExpenseAnalysisAsync(ReportFilterDto filter);

    /// <summary>Fund request summary</summary>
    Task<FundRequestReportDto> GetFundRequestReportAsync(ReportFilterDto filter);

    /// <summary>Toll expense breakdown</summary>
    Task<TollExpenseReportDto> GetTollExpenseReportAsync(ReportFilterDto filter);

    /// <summary>Customer billing summary</summary>
    Task<CustomerBillingReportDto> GetCustomerBillingAsync(ReportFilterDto filter);

    // ════════════════════════════════════════════════════════════
    //  FLEET REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Vehicle utilization metrics</summary>
    Task<VehicleUtilizationReportDto> GetVehicleUtilizationAsync(ReportFilterDto filter);

    /// <summary>Maintenance cost tracking</summary>
    Task<MaintenanceCostReportDto> GetMaintenanceCostReportAsync(ReportFilterDto filter);

    // ════════════════════════════════════════════════════════════
    //  PERFORMANCE REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Transporter performance (ratings, on-time, compliance)</summary>
    Task<TransporterPerformanceReportDto> GetTransporterPerformanceAsync(ReportFilterDto filter);

    /// <summary>Branch comparison</summary>
    Task<BranchComparisonReportDto> GetBranchComparisonAsync(ReportFilterDto filter);

    // ════════════════════════════════════════════════════════════
    //  LEGACY GAP REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Daily Status Report with cross-module filtering (Legacy: TransDSR.aspx)</summary>
    Task<DailyStatusReportDto> GetDailyStatusReportAsync(DsrFilterDto filter);

    /// <summary>Labour cost report (Legacy: ReportLabour.aspx)</summary>
    Task<LabourReportDto> GetLabourReportAsync(ReportFilterDto filter);

    /// <summary>Vehicle closing report — end-of-period summary (Legacy: ReportVehicleClosing.aspx)</summary>
    Task<VehicleClosingReportDto> GetVehicleClosingReportAsync(ReportFilterDto filter);

    /// <summary>Weekly trip aggregation report (Legacy: WeeklyTripReport.aspx)</summary>
    Task<WeeklyTripReportDto> GetWeeklyTripReportAsync(ReportFilterDto filter);

    /// <summary>Vehicle daily expense report — aggregated by vehicle/date range</summary>
    Task<VehicleDailyExpenseReportDto> GetVehicleDailyExpenseReportAsync(ReportFilterDto filter);
}
