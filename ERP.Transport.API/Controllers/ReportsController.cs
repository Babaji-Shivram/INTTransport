using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using EPR.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Transport.API.Controllers;

/// <summary>
/// Transport reports and analytics.
/// </summary>
public class ReportsController : TransportBaseController
{
    private readonly IReportsService _svc;
    private readonly IExcelExportService _excelExport;

    public ReportsController(IReportsService svc, IExcelExportService excelExport)
    {
        _svc = svc;
        _excelExport = excelExport;
    }

    // ════════════════════════════════════════════════════════════
    //  OPERATIONAL
    // ════════════════════════════════════════════════════════════

    /// <summary>Job summary report</summary>
    [HttpGet("job-summary")]
    public async Task<ActionResult<ApiResponse<JobSummaryReportDto>>> GetJobSummary(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetJobSummaryAsync(filter));

    /// <summary>Daily operations report</summary>
    [HttpGet("daily-operations")]
    public async Task<ActionResult<ApiResponse<DailyOperationsReportDto>>> GetDailyOperations(
        [FromQuery] DateTime date, [FromQuery] string? branchId = null)
        => OkResponse(await _svc.GetDailyOperationsAsync(date, branchId));

    /// <summary>Delivery performance</summary>
    [HttpGet("delivery-performance")]
    public async Task<ActionResult<ApiResponse<DeliveryPerformanceReportDto>>> GetDeliveryPerformance(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetDeliveryPerformanceAsync(filter));

    /// <summary>Route analysis</summary>
    [HttpGet("route-analysis")]
    public async Task<ActionResult<ApiResponse<RouteAnalysisReportDto>>> GetRouteAnalysis(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetRouteAnalysisAsync(filter));

    /// <summary>Empty leg tracking</summary>
    [HttpGet("empty-legs")]
    public async Task<ActionResult<ApiResponse<EmptyLegReportDto>>> GetEmptyLegReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetEmptyLegReportAsync(filter));

    // ════════════════════════════════════════════════════════════
    //  FINANCIAL
    // ════════════════════════════════════════════════════════════

    /// <summary>Expense analysis</summary>
    [HttpGet("expense-analysis")]
    public async Task<ActionResult<ApiResponse<ExpenseAnalysisReportDto>>> GetExpenseAnalysis(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetExpenseAnalysisAsync(filter));

    /// <summary>Fund request summary</summary>
    [HttpGet("fund-requests")]
    public async Task<ActionResult<ApiResponse<FundRequestReportDto>>> GetFundRequestReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetFundRequestReportAsync(filter));

    /// <summary>Toll expense breakdown</summary>
    [HttpGet("toll-expenses")]
    public async Task<ActionResult<ApiResponse<TollExpenseReportDto>>> GetTollExpenseReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetTollExpenseReportAsync(filter));

    /// <summary>Customer billing</summary>
    [HttpGet("customer-billing")]
    public async Task<ActionResult<ApiResponse<CustomerBillingReportDto>>> GetCustomerBilling(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetCustomerBillingAsync(filter));

    // ════════════════════════════════════════════════════════════
    //  FLEET
    // ════════════════════════════════════════════════════════════

    /// <summary>Vehicle utilization</summary>
    [HttpGet("vehicle-utilization")]
    public async Task<ActionResult<ApiResponse<VehicleUtilizationReportDto>>> GetVehicleUtilization(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetVehicleUtilizationAsync(filter));

    /// <summary>Maintenance costs</summary>
    [HttpGet("maintenance-costs")]
    public async Task<ActionResult<ApiResponse<MaintenanceCostReportDto>>> GetMaintenanceCostReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetMaintenanceCostReportAsync(filter));

    // ════════════════════════════════════════════════════════════
    //  PERFORMANCE
    // ════════════════════════════════════════════════════════════

    /// <summary>Transporter performance</summary>
    [HttpGet("transporter-performance")]
    public async Task<ActionResult<ApiResponse<TransporterPerformanceReportDto>>> GetTransporterPerformance(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetTransporterPerformanceAsync(filter));

    /// <summary>Branch comparison</summary>
    [HttpGet("branch-comparison")]
    public async Task<ActionResult<ApiResponse<BranchComparisonReportDto>>> GetBranchComparison(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetBranchComparisonAsync(filter));

    // ════════════════════════════════════════════════════════════
    //  LEGACY GAP REPORTS
    // ════════════════════════════════════════════════════════════

    /// <summary>Daily Status Report (DSR) — consolidated view across modules.</summary>
    [HttpGet("daily-status")]
    public async Task<ActionResult<ApiResponse<DailyStatusReportDto>>> GetDailyStatusReport(
        [FromQuery] DsrFilterDto filter)
        => OkResponse(await _svc.GetDailyStatusReportAsync(filter));

    /// <summary>Labour / loading-unloading report.</summary>
    [HttpGet("labour")]
    public async Task<ActionResult<ApiResponse<LabourReportDto>>> GetLabourReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetLabourReportAsync(filter));

    /// <summary>Vehicle closing report — per-vehicle P&amp;L.</summary>
    [HttpGet("vehicle-closing")]
    public async Task<ActionResult<ApiResponse<VehicleClosingReportDto>>> GetVehicleClosingReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetVehicleClosingReportAsync(filter));

    /// <summary>Weekly trip report — trips, freight, durations.</summary>
    [HttpGet("weekly-trips")]
    public async Task<ActionResult<ApiResponse<WeeklyTripReportDto>>> GetWeeklyTripReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetWeeklyTripReportAsync(filter));

    /// <summary>Vehicle daily expense report — category-wise aggregation.</summary>
    [HttpGet("vehicle-daily-expenses")]
    public async Task<ActionResult<ApiResponse<VehicleDailyExpenseReportDto>>> GetVehicleDailyExpenseReport(
        [FromQuery] ReportFilterDto filter)
        => OkResponse(await _svc.GetVehicleDailyExpenseReportAsync(filter));

    // ════════════════════════════════════════════════════════════
    //  EXCEL EXPORT
    // ════════════════════════════════════════════════════════════

    /// <summary>Export transport jobs to Excel (.xlsx)</summary>
    [HttpGet("export/jobs")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportJobs([FromQuery] TransportJobFilterDto filter)
    {
        var (bytes, fileName) = await _excelExport.ExportJobsAsync(filter);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>Export job summary report to Excel</summary>
    [HttpGet("export/job-summary")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportJobSummary([FromQuery] ReportFilterDto filter)
    {
        var report = await _svc.GetJobSummaryAsync(filter);
        var (bytes, fileName) = _excelExport.ExportReport("JobSummary", report.ByStatus);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>Export expense analysis to Excel</summary>
    [HttpGet("export/expense-analysis")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportExpenseAnalysis([FromQuery] ReportFilterDto filter)
    {
        var report = await _svc.GetExpenseAnalysisAsync(filter);
        var (bytes, fileName) = _excelExport.ExportReport("ExpenseAnalysis", report.ByCategory);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>Export customer billing to Excel</summary>
    [HttpGet("export/customer-billing")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportCustomerBilling([FromQuery] ReportFilterDto filter)
    {
        var report = await _svc.GetCustomerBillingAsync(filter);
        var (bytes, fileName) = _excelExport.ExportReport("CustomerBilling", report.ByCustomer);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>Export transporter performance to Excel</summary>
    [HttpGet("export/transporter-performance")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportTransporterPerformance([FromQuery] ReportFilterDto filter)
    {
        var report = await _svc.GetTransporterPerformanceAsync(filter);
        var (bytes, fileName) = _excelExport.ExportReport("TransporterPerformance", report.TopPerformers);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
