using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Report;

// ═══════════════════════════════════════════════════════════════
//  Transport Reports DTOs
//  Aligned with ReportsService output shapes
// ═══════════════════════════════════════════════════════════════

// ── Common ──────────────────────────────────────────────────────

public class ReportFilterDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? BranchId { get; set; }
    public string? CountryCode { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? TransporterId { get; set; }
}

public class StatusCountDto
{
    public string Status { get; set; } = null!;
    public int Count { get; set; }
}

public class VehicleTypeCountDto
{
    public VehicleTypeEnum VehicleType { get; set; }
    public int Count { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  1. Job Summary Report
// ═══════════════════════════════════════════════════════════════

public class JobSummaryReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int PendingJobs { get; set; }
    public int CancelledJobs { get; set; }
    public decimal TotalFreightValue { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<StatusCountDto> ByStatus { get; set; } = new();
    public List<VehicleTypeCountDto> ByVehicleType { get; set; } = new();
}

// ═══════════════════════════════════════════════════════════════
//  2. Daily Operations Report
// ═══════════════════════════════════════════════════════════════

public class DailyOperationsReportDto
{
    public DateTime Date { get; set; }
    public string? BranchId { get; set; }
    public int NewRequests { get; set; }
    public int JobsCreated { get; set; }
    public int JobsDispatched { get; set; }
    public int JobsCompleted { get; set; }
    public int ActiveTrips { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal FleetUtilization { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  3. Delivery Performance Report
// ═══════════════════════════════════════════════════════════════

public class DeliveryPerformanceReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDeliveries { get; set; }
    public int OnTimeDeliveries { get; set; }
    public int LateDeliveries { get; set; }
    public decimal OnTimePercentage { get; set; }
    public decimal AverageDeliveryTimeHours { get; set; }
    public decimal LongestDelayHours { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  4. Route Analysis Report
// ═══════════════════════════════════════════════════════════════

public class RouteAnalysisReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalRoutes { get; set; }
    public List<RouteMetricDto> TopRoutes { get; set; } = new();
}

public class RouteMetricDto
{
    public Guid? OriginLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public int TripCount { get; set; }
    public decimal TotalFreight { get; set; }
    public decimal AverageFreight { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  5. Empty Leg Report
// ═══════════════════════════════════════════════════════════════

public class EmptyLegReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalEmptyLegs { get; set; }
    public decimal TotalEmptyKm { get; set; }
    public decimal EstimatedFuelCost { get; set; }
    public List<EmptyLegDetailDto> EmptyLegDetails { get; set; } = new();
}

public class EmptyLegDetailDto
{
    public DateTime Date { get; set; }
    public Guid VehicleId { get; set; }
    public decimal DistanceKm { get; set; }
    public decimal FuelCost { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  6. Expense Analysis Report
// ═══════════════════════════════════════════════════════════════

public class ExpenseAnalysisReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public int ExpenseCount { get; set; }
    public List<ExpenseCategoryDto> ByCategory { get; set; } = new();
    public List<TopExpenseDto> TopExpenses { get; set; } = new();
}

public class ExpenseCategoryDto
{
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class TopExpenseDto
{
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  7. Fund Request Report
// ═══════════════════════════════════════════════════════════════

public class FundRequestReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalRequests { get; set; }
    public decimal TotalRequestedAmount { get; set; }
    public decimal TotalApprovedAmount { get; set; }
    public decimal TotalProcessedAmount { get; set; }
    public List<StatusCountDto> ByStatus { get; set; } = new();
    public decimal AverageProcessingDays { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  8. Toll Expense Report
// ═══════════════════════════════════════════════════════════════

public class TollExpenseReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalTollExpenses { get; set; }
    public int TollCount { get; set; }
    public decimal AverageTollPerTrip { get; set; }
    public List<VehicleTollDto> ByVehicle { get; set; } = new();
}

public class VehicleTollDto
{
    public Guid RequestId { get; set; }
    public decimal TotalTolls { get; set; }
    public int TollCount { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  9. Customer Billing Report
// ═══════════════════════════════════════════════════════════════

public class CustomerBillingReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int CustomerCount { get; set; }
    public List<CustomerBillingDetailDto> ByCustomer { get; set; } = new();
}

public class CustomerBillingDetailDto
{
    public Guid CustomerId { get; set; }
    public int TotalJobs { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal OutstandingAmount { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  10. Vehicle Utilization Report
// ═══════════════════════════════════════════════════════════════

public class VehicleUtilizationReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public decimal AverageUtilization { get; set; }
    public List<VehicleUtilizationDetailDto> ByVehicle { get; set; } = new();
}

public class VehicleUtilizationDetailDto
{
    public Guid VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public int TotalTrips { get; set; }
    public decimal TotalKm { get; set; }
    public decimal TotalHours { get; set; }
    public decimal UtilizationPercentage { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  11. Maintenance Cost Report
// ═══════════════════════════════════════════════════════════════

public class MaintenanceCostReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalWorkOrders { get; set; }
    public int CompletedWorkOrders { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal TotalPartsCost { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<MaintenanceTypeCostDto> ByMaintenanceType { get; set; } = new();
    public List<VehicleMaintenanceCostDto> TopVehiclesByCost { get; set; } = new();
}

public class MaintenanceTypeCostDto
{
    public string MaintenanceType { get; set; } = null!;
    public int Count { get; set; }
    public decimal TotalCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
}

public class VehicleMaintenanceCostDto
{
    public Guid VehicleId { get; set; }
    public int WorkOrderCount { get; set; }
    public decimal TotalCost { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  12. Transporter Performance Report
// ═══════════════════════════════════════════════════════════════

public class TransporterPerformanceReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalTransporters { get; set; }
    public int ActiveTransporters { get; set; }
    public decimal AverageRating { get; set; }
    public decimal AverageOnTimePercentage { get; set; }
    public List<TransporterMetricDto> TopPerformers { get; set; } = new();
    public List<TransporterMetricDto> NeedsImprovement { get; set; } = new();
}

public class TransporterMetricDto
{
    public Guid TransporterId { get; set; }
    public string TransporterName { get; set; } = null!;
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int OnTimeDeliveries { get; set; }
    public decimal OnTimePercentage { get; set; }
    public decimal AverageRating { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ComplianceScore { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  13. Branch Comparison Report
// ═══════════════════════════════════════════════════════════════

public class BranchComparisonReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalBranches { get; set; }
    public List<BranchMetricDto> ByBranch { get; set; } = new();
    public string? TopBranchByRevenue { get; set; }
    public string? TopBranchByVolume { get; set; }
}

public class BranchMetricDto
{
    public string BranchId { get; set; } = null!;
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public decimal OnTimePercentage { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageJobValue { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  14. Daily Status Report (Legacy: TransDSR.aspx)
// ═══════════════════════════════════════════════════════════════

public class DsrFilterDto : ReportFilterDto
{
    public bool IncludeTransport { get; set; } = true;
    public bool IncludeFleet { get; set; } = true;
    public bool IncludeMaintenance { get; set; } = true;
    public bool IncludeExpenses { get; set; } = true;
}

public class DailyStatusReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    // ── Transport Module ────────────────────────────────────────
    public int TotalActiveJobs { get; set; }
    public int JobsInTransit { get; set; }
    public int JobsInWarehouse { get; set; }
    public int JobsDeliveredToday { get; set; }
    public int JobsClearedToday { get; set; }
    public int PendingApprovals { get; set; }

    // ── Fleet Module ────────────────────────────────────────────
    public int TotalFleetVehicles { get; set; }
    public int VehiclesOnTrip { get; set; }
    public int VehiclesAvailable { get; set; }
    public int VehiclesUnderMaintenance { get; set; }

    // ── Maintenance Module ──────────────────────────────────────
    public int ActiveWorkOrders { get; set; }
    public int CompletedWorkOrdersToday { get; set; }
    public decimal MaintenanceCostToday { get; set; }

    // ── Expenses ────────────────────────────────────────────────
    public decimal TotalExpensesToday { get; set; }
    public int PendingExpenseApprovals { get; set; }

    public List<DsrJobSummaryDto> JobDetails { get; set; } = new();
}

public class DsrJobSummaryDto
{
    public Guid JobId { get; set; }
    public string RequestNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string Status { get; set; } = null!;
    public string? VehicleNumber { get; set; }
    public string? TransporterName { get; set; }
    public string? CurrentLocation { get; set; }
    public DateTime? ExpectedDelivery { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  15. Labour Report (Legacy: ReportLabour.aspx)
// ═══════════════════════════════════════════════════════════════

public class LabourReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalWorkOrders { get; set; }
    public decimal TotalLabourCost { get; set; }
    public decimal TotalLabourHours { get; set; }
    public decimal AverageLabourCostPerOrder { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<LabourDetailDto> ByWorkOrder { get; set; } = new();
}

public class LabourDetailDto
{
    public Guid WorkOrderId { get; set; }
    public string WorkOrderNumber { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string MaintenanceType { get; set; } = null!;
    public decimal LabourCost { get; set; }
    public int LabourHours { get; set; }
    public string? ServiceProviderName { get; set; }
    public DateTime CompletedDate { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  16. Vehicle Closing Report (Legacy: ReportVehicleClosing.aspx)
// ═══════════════════════════════════════════════════════════════

public class VehicleClosingReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalVehicles { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalMaintenanceCost { get; set; }
    public decimal NetProfit { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<VehicleClosingDetailDto> ByVehicle { get; set; } = new();
}

public class VehicleClosingDetailDto
{
    public Guid VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public int TotalTrips { get; set; }
    public decimal TotalKm { get; set; }
    public decimal Revenue { get; set; }
    public decimal FuelCost { get; set; }
    public decimal TollCost { get; set; }
    public decimal MaintenanceCost { get; set; }
    public decimal OtherExpenses { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public decimal CostPerKm { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  17. Weekly Trip Report (Legacy: WeeklyTripReport.aspx)
// ═══════════════════════════════════════════════════════════════

public class WeeklyTripReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalKm { get; set; }
    public decimal TotalFreight { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<WeeklyTripDetailDto> ByWeek { get; set; } = new();
}

public class WeeklyTripDetailDto
{
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int WeekNumber { get; set; }
    public int TripCount { get; set; }
    public decimal TotalKm { get; set; }
    public decimal TotalFreight { get; set; }
    public int CompletedTrips { get; set; }
    public int CancelledTrips { get; set; }
    public decimal OnTimePercentage { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  18. Vehicle Daily Expense Report
// ═══════════════════════════════════════════════════════════════

public class VehicleDailyExpenseReportDto
{
    public DateTime ReportDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalRecords { get; set; }
    public decimal GrandTotal { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public List<DailyExpenseCategorySummaryDto> ByCategory { get; set; } = new();
    public List<VehicleExpenseSummaryDto> ByVehicle { get; set; } = new();
}

public class DailyExpenseCategorySummaryDto
{
    public string Category { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public int RecordCount { get; set; }
    public decimal Percentage { get; set; }
}

public class VehicleExpenseSummaryDto
{
    public Guid VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public int DayCount { get; set; }
    public decimal TotalFuel { get; set; }
    public decimal TotalToll { get; set; }
    public decimal TotalOther { get; set; }
    public decimal GrandTotal { get; set; }
}
