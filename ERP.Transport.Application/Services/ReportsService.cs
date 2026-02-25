using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Reports service — aggregates data for operational, financial, fleet &amp; performance reports.
/// Uses actual domain entities: TransportRequest, TransportExpense, VehicleFundRequest, etc.
/// </summary>
public class ReportsService : IReportsService
{
    private readonly IRepository<TransportRequest> _requestRepo;
    private readonly IRepository<TransportVehicle> _vehicleAssignmentRepo;
    private readonly IRepository<VehicleRate> _rateRepo;
    private readonly IRepository<TransportExpense> _expenseRepo;
    private readonly IRepository<TransportDelivery> _deliveryRepo;
    private readonly IRepository<VehicleFundRequest> _fundRequestRepo;
    private readonly IRepository<Transporter> _transporterRepo;
    private readonly IRepository<FleetVehicle> _fleetVehicleRepo;
    private readonly IRepository<VehicleTravelLog> _travelLogRepo;
    private readonly IRepository<MaintenanceWorkOrder> _maintenanceRepo;
    private readonly IRepository<VehicleDailyExpense> _dailyExpenseRepo;
    private readonly ILogger<ReportsService> _logger;

    public ReportsService(
        IRepository<TransportRequest> requestRepo,
        IRepository<TransportVehicle> vehicleAssignmentRepo,
        IRepository<VehicleRate> rateRepo,
        IRepository<TransportExpense> expenseRepo,
        IRepository<TransportDelivery> deliveryRepo,
        IRepository<VehicleFundRequest> fundRequestRepo,
        IRepository<Transporter> transporterRepo,
        IRepository<FleetVehicle> fleetVehicleRepo,
        IRepository<VehicleTravelLog> travelLogRepo,
        IRepository<MaintenanceWorkOrder> maintenanceRepo,
        IRepository<VehicleDailyExpense> dailyExpenseRepo,
        ILogger<ReportsService> logger)
    {
        _requestRepo = requestRepo;
        _vehicleAssignmentRepo = vehicleAssignmentRepo;
        _rateRepo = rateRepo;
        _expenseRepo = expenseRepo;
        _deliveryRepo = deliveryRepo;
        _fundRequestRepo = fundRequestRepo;
        _transporterRepo = transporterRepo;
        _fleetVehicleRepo = fleetVehicleRepo;
        _travelLogRepo = travelLogRepo;
        _maintenanceRepo = maintenanceRepo;
        _dailyExpenseRepo = dailyExpenseRepo;
        _logger = logger;
    }

    // ════════════════════════════════════════════════════════════
    //  OPERATIONAL REPORTS
    // ════════════════════════════════════════════════════════════

    public async Task<JobSummaryReportDto> GetJobSummaryAsync(ReportFilterDto filter)
    {
        var requests = (await _requestRepo.FindAsync(BuildRequestPredicate(filter))).ToList();

        var byStatus = requests
            .GroupBy(r => r.Status.ToString())
            .Select(g => new StatusCountDto { Status = g.Key, Count = g.Count() })
            .ToList();

        var byVehicleType = requests
            .GroupBy(r => r.VehicleTypeRequired)
            .Select(g => new VehicleTypeCountDto { VehicleType = g.Key, Count = g.Count() })
            .ToList();

        // Get freight from VehicleRate through TransportVehicle
        var requestIds = requests.Select(r => r.Id).ToHashSet();
        var vehicleAssignments = (await _vehicleAssignmentRepo.FindAsync(
            v => requestIds.Contains(v.TransportRequestId))).ToList();
        var vehicleIds = vehicleAssignments.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();
        var totalFreight = rates.Sum(r => r.ApprovedAmount ?? r.TotalRate);

        return new JobSummaryReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalJobs = requests.Count,
            CompletedJobs = requests.Count(r => r.Status == TransportStatus.Cleared),
            PendingJobs = requests.Count(r =>
                r.Status == TransportStatus.RequestCreated ||
                r.Status == TransportStatus.RequestReceived),
            CancelledJobs = requests.Count(r => r.Status == TransportStatus.Cancelled),
            TotalFreightValue = totalFreight,
            ByStatus = byStatus,
            ByVehicleType = byVehicleType
        };
    }

    public async Task<DailyOperationsReportDto> GetDailyOperationsAsync(DateTime date, string? branchId = null)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var requests = (await _requestRepo.FindAsync(r =>
            r.CreatedDate >= dayStart && r.CreatedDate < dayEnd &&
            (branchId == null || r.BranchId.ToString() == branchId))).ToList();

        var deliveries = (await _deliveryRepo.FindAsync(d =>
            d.DeliveryDate >= dayStart && d.DeliveryDate < dayEnd)).ToList();

        return new DailyOperationsReportDto
        {
            Date = dayStart,
            BranchId = branchId,
            NewRequests = requests.Count,
            JobsCreated = requests.Count,
            JobsDispatched = requests.Count(r => r.Status >= TransportStatus.InTransit),
            JobsCompleted = requests.Count(r => r.Status == TransportStatus.Cleared),
            ActiveTrips = requests.Count(r => r.Status == TransportStatus.InTransit),
            TotalRevenue = 0, // Requires freight aggregation
            TotalExpenses = 0,
            FleetUtilization = 0
        };
    }

    public async Task<DeliveryPerformanceReportDto> GetDeliveryPerformanceAsync(ReportFilterDto filter)
    {
        var deliveries = (await _deliveryRepo.FindAsync(d =>
            d.DeliveryDate >= filter.FromDate && d.DeliveryDate <= filter.ToDate)).ToList();

        // Get parent requests for delivery date comparison
        var requestIds = deliveries.Select(d => d.TransportRequestId).ToHashSet();
        var requests = (await _requestRepo.FindAsync(r => requestIds.Contains(r.Id) &&
            (filter.BranchId == null || r.BranchId == filter.BranchId) &&
            (filter.CountryCode == null || r.CountryCode == filter.CountryCode))).ToList();

        var joined = deliveries
            .Join(requests, d => d.TransportRequestId, r => r.Id, (d, r) => new { d, r })
            .ToList();

        var onTime = joined.Count(x => x.r.RequiredDeliveryDate.HasValue &&
            x.d.DeliveryDate <= x.r.RequiredDeliveryDate.Value);
        var late = joined.Count(x => x.r.RequiredDeliveryDate.HasValue &&
            x.d.DeliveryDate > x.r.RequiredDeliveryDate.Value);

        var delays = joined
            .Where(x => x.r.RequiredDeliveryDate.HasValue && x.d.DeliveryDate > x.r.RequiredDeliveryDate)
            .Select(x => (x.d.DeliveryDate - x.r.RequiredDeliveryDate!.Value).TotalHours)
            .ToList();

        return new DeliveryPerformanceReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalDeliveries = joined.Count,
            OnTimeDeliveries = onTime,
            LateDeliveries = late,
            OnTimePercentage = joined.Count > 0 ? (decimal)onTime / joined.Count * 100 : 0,
            AverageDeliveryTimeHours = delays.Count > 0 ? (decimal)delays.Average() : 0,
            LongestDelayHours = delays.Count > 0 ? (decimal)delays.Max() : 0
        };
    }

    public async Task<RouteAnalysisReportDto> GetRouteAnalysisAsync(ReportFilterDto filter)
    {
        var requests = (await _requestRepo.FindAsync(BuildRequestPredicate(filter))).ToList();

        // Get rates for freight values
        var requestIds = requests.Select(r => r.Id).ToHashSet();
        var vehicles = (await _vehicleAssignmentRepo.FindAsync(v => requestIds.Contains(v.TransportRequestId))).ToList();
        var vehicleIds = vehicles.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();

        // Build lookup: requestId → total freight
        var freightByRequest = vehicles
            .GroupJoin(rates, v => v.Id, r => r.TransportVehicleId, (v, rs) => new
            {
                v.TransportRequestId,
                Freight = rs.Sum(r => r.ApprovedAmount ?? r.TotalRate)
            })
            .GroupBy(x => x.TransportRequestId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Freight));

        var topRoutes = requests
            .GroupBy(r => new { r.OriginLocationId, r.DestinationLocationId })
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g =>
            {
                var freight = g.Sum(r => freightByRequest.GetValueOrDefault(r.Id, 0));
                return new RouteMetricDto
                {
                    OriginLocationId = g.Key.OriginLocationId,
                    DestinationLocationId = g.Key.DestinationLocationId,
                    TripCount = g.Count(),
                    TotalFreight = freight,
                    AverageFreight = g.Count() > 0 ? freight / g.Count() : 0
                };
            })
            .ToList();

        return new RouteAnalysisReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalRoutes = requests
                .Select(r => new { r.OriginLocationId, r.DestinationLocationId }).Distinct().Count(),
            TopRoutes = topRoutes
        };
    }

    public async Task<EmptyLegReportDto> GetEmptyLegReportAsync(ReportFilterDto filter)
    {
        // Empty legs: travel logs not linked to a transport request
        var travelLogs = (await _travelLogRepo.FindAsync(t =>
            t.TripDate >= filter.FromDate && t.TripDate <= filter.ToDate &&
            t.TransportRequestId == null)).ToList();

        return new EmptyLegReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalEmptyLegs = travelLogs.Count,
            TotalEmptyKm = travelLogs.Sum(t => t.DistanceKm),
            EstimatedFuelCost = travelLogs.Sum(t => t.FuelCost ?? 0),
            EmptyLegDetails = travelLogs.Select(t => new EmptyLegDetailDto
            {
                Date = t.TripDate,
                VehicleId = t.FleetVehicleId,
                DistanceKm = t.DistanceKm,
                FuelCost = t.FuelCost ?? 0
            }).ToList()
        };
    }

    // ════════════════════════════════════════════════════════════
    //  FINANCIAL REPORTS
    // ════════════════════════════════════════════════════════════

    public async Task<ExpenseAnalysisReportDto> GetExpenseAnalysisAsync(ReportFilterDto filter)
    {
        var expenses = (await _expenseRepo.FindAsync(e =>
            e.ExpenseDate >= filter.FromDate && e.ExpenseDate <= filter.ToDate)).ToList();

        // Filter by branch via TransportRequest if needed
        if (filter.BranchId.HasValue)
        {
            var requestIds = (await _requestRepo.FindAsync(r => r.BranchId == filter.BranchId))
                .Select(r => r.Id).ToHashSet();
            expenses = expenses.Where(e => requestIds.Contains(e.TransportRequestId)).ToList();
        }

        var total = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.Category.ToString())
            .Select(g => new ExpenseCategoryDto
            {
                Category = g.Key,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count(),
                Percentage = total > 0 ? g.Sum(e => e.Amount) / total * 100 : 0
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new ExpenseAnalysisReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalExpenses = total,
            ExpenseCount = expenses.Count,
            ByCategory = byCategory,
            TopExpenses = expenses
                .OrderByDescending(e => e.Amount)
                .Take(10)
                .Select(e => new TopExpenseDto
                {
                    Description = e.Remarks,
                    Category = e.Category.ToString(),
                    Amount = e.Amount,
                    Date = e.ExpenseDate
                })
                .ToList()
        };
    }

    public async Task<FundRequestReportDto> GetFundRequestReportAsync(ReportFilterDto filter)
    {
        var requests = (await _fundRequestRepo.FindAsync(f =>
            f.CreatedDate >= filter.FromDate && f.CreatedDate <= filter.ToDate)).ToList();

        var byStatus = requests
            .GroupBy(f => f.Status.ToString())
            .Select(g => new StatusCountDto { Status = g.Key, Count = g.Count() })
            .ToList();

        return new FundRequestReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalRequests = requests.Count,
            TotalRequestedAmount = requests.Sum(f => f.Amount),
            TotalApprovedAmount = requests
                .Where(f => f.Status == FundRequestStatus.Approved || f.Status == FundRequestStatus.Processed)
                .Sum(f => f.Amount),
            TotalProcessedAmount = requests
                .Where(f => f.Status == FundRequestStatus.Processed)
                .Sum(f => f.Amount),
            ByStatus = byStatus,
            AverageProcessingDays = 0 // Would calculate from ProcessedDate - CreatedDate
        };
    }

    public async Task<TollExpenseReportDto> GetTollExpenseReportAsync(ReportFilterDto filter)
    {
        var tolls = (await _expenseRepo.FindAsync(e =>
            e.Category == ExpenseCategory.TollCharges &&
            e.ExpenseDate >= filter.FromDate && e.ExpenseDate <= filter.ToDate)).ToList();

        var byRequest = tolls
            .GroupBy(e => e.TransportRequestId)
            .Select(g => new VehicleTollDto
            {
                RequestId = g.Key,
                TotalTolls = g.Sum(e => e.Amount),
                TollCount = g.Count()
            })
            .ToList();

        return new TollExpenseReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalTollExpenses = tolls.Sum(e => e.Amount),
            TollCount = tolls.Count,
            AverageTollPerTrip = byRequest.Count > 0 ? tolls.Sum(e => e.Amount) / byRequest.Count : 0,
            ByVehicle = byRequest
        };
    }

    public async Task<CustomerBillingReportDto> GetCustomerBillingAsync(ReportFilterDto filter)
    {
        var requests = (await _requestRepo.FindAsync(r =>
            r.Status == TransportStatus.Cleared &&
            r.CreatedDate >= filter.FromDate && r.CreatedDate <= filter.ToDate &&
            (filter.BranchId == null || r.BranchId == filter.BranchId))).ToList();

        // Get freight totals
        var requestIds = requests.Select(r => r.Id).ToHashSet();
        var vehicles = (await _vehicleAssignmentRepo.FindAsync(v => requestIds.Contains(v.TransportRequestId))).ToList();
        var vehicleIds = vehicles.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();

        var freightByRequest = vehicles
            .GroupJoin(rates, v => v.Id, r => r.TransportVehicleId, (v, rs) => new
            {
                v.TransportRequestId,
                Freight = rs.Sum(r => r.ApprovedAmount ?? r.TotalRate)
            })
            .GroupBy(x => x.TransportRequestId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Freight));

        var byCustomer = requests
            .GroupBy(r => r.CustomerId)
            .Select(g => new CustomerBillingDetailDto
            {
                CustomerId = g.Key,
                TotalJobs = g.Count(),
                TotalBilled = g.Sum(r => freightByRequest.GetValueOrDefault(r.Id, 0)),
                OutstandingAmount = 0
            })
            .OrderByDescending(c => c.TotalBilled)
            .Take(20)
            .ToList();

        return new CustomerBillingReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalBilled = byCustomer.Sum(c => c.TotalBilled),
            TotalCollected = 0,
            OutstandingAmount = 0,
            CustomerCount = byCustomer.Count,
            ByCustomer = byCustomer
        };
    }

    // ════════════════════════════════════════════════════════════
    //  FLEET REPORTS
    // ════════════════════════════════════════════════════════════

    public async Task<VehicleUtilizationReportDto> GetVehicleUtilizationAsync(ReportFilterDto filter)
    {
        var vehicles = (await _fleetVehicleRepo.FindAsync(v =>
            v.IsActive && !v.IsDeleted &&
            (filter.BranchId == null || v.BranchId == filter.BranchId))).ToList();

        var travelLogs = (await _travelLogRepo.FindAsync(t =>
            t.TripDate >= filter.FromDate && t.TripDate <= filter.ToDate)).ToList();

        var byVehicle = vehicles.Select(v =>
        {
            var logs = travelLogs.Where(t => t.FleetVehicleId == v.Id).ToList();
            var totalKm = logs.Sum(t => t.DistanceKm);
            var totalHours = logs
                .Where(t => t.DepartureTime.HasValue && t.ArrivalTime.HasValue)
                .Sum(t => (t.ArrivalTime!.Value - t.DepartureTime!.Value).TotalHours);

            return new VehicleUtilizationDetailDto
            {
                VehicleId = v.Id,
                RegistrationNumber = v.RegistrationNumber,
                VehicleType = v.VehicleType.ToString(),
                TotalTrips = logs.Count,
                TotalKm = totalKm,
                TotalHours = (decimal)totalHours,
                UtilizationPercentage = 0
            };
        }).OrderByDescending(v => v.TotalTrips).ToList();

        return new VehicleUtilizationReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalVehicles = vehicles.Count,
            ActiveVehicles = vehicles.Count(v => v.CurrentStatus == FleetVehicleStatus.OnTrip),
            AverageUtilization = byVehicle.Count > 0 ? byVehicle.Average(v => v.UtilizationPercentage) : 0,
            ByVehicle = byVehicle
        };
    }

    public async Task<MaintenanceCostReportDto> GetMaintenanceCostReportAsync(ReportFilterDto filter)
    {
        var workOrders = (await _maintenanceRepo.FindAsync(m =>
            m.ScheduledDate >= filter.FromDate && m.ScheduledDate <= filter.ToDate &&
            (filter.BranchId == null || m.BranchId == filter.BranchId))).ToList();

        var completed = workOrders.Where(w => w.Status == MaintenanceStatus.Completed).ToList();

        var byType = completed
            .GroupBy(w => w.MaintenanceType.ToString())
            .Select(g => new MaintenanceTypeCostDto
            {
                MaintenanceType = g.Key,
                Count = g.Count(),
                TotalCost = g.Sum(w => w.ActualCost ?? 0),
                LaborCost = g.Sum(w => w.LaborCost ?? 0),
                PartsCost = g.Sum(w => w.PartsCost ?? 0)
            })
            .ToList();

        var byVehicle = completed
            .GroupBy(w => w.FleetVehicleId)
            .Select(g => new VehicleMaintenanceCostDto
            {
                VehicleId = g.Key,
                WorkOrderCount = g.Count(),
                TotalCost = g.Sum(w => w.ActualCost ?? 0)
            })
            .OrderByDescending(v => v.TotalCost)
            .Take(10)
            .ToList();

        return new MaintenanceCostReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalWorkOrders = workOrders.Count,
            CompletedWorkOrders = completed.Count,
            TotalCost = completed.Sum(w => w.ActualCost ?? 0),
            TotalLaborCost = completed.Sum(w => w.LaborCost ?? 0),
            TotalPartsCost = completed.Sum(w => w.PartsCost ?? 0),
            ByMaintenanceType = byType,
            TopVehiclesByCost = byVehicle
        };
    }

    // ════════════════════════════════════════════════════════════
    //  PERFORMANCE REPORTS
    // ════════════════════════════════════════════════════════════

    public async Task<TransporterPerformanceReportDto> GetTransporterPerformanceAsync(ReportFilterDto filter)
    {
        var transporters = (await _transporterRepo.FindAsync(t =>
            t.Status == TransporterStatus.Active && !t.IsDeleted)).ToList();

        // Get vehicle assignments in period → links transporter to request
        var assignments = (await _vehicleAssignmentRepo.FindAsync(v =>
            v.CreatedDate >= filter.FromDate && v.CreatedDate <= filter.ToDate)).ToList();

        var assignmentRequestIds = assignments.Select(a => a.TransportRequestId).ToHashSet();
        var requests = (await _requestRepo.FindAsync(r => assignmentRequestIds.Contains(r.Id))).ToList();
        var requestMap = requests.ToDictionary(r => r.Id);

        // Get rates for revenue
        var vehicleIds = assignments.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();
        var rateByVehicleId = rates.ToDictionary(r => r.TransportVehicleId);

        // Get deliveries for on-time tracking
        var deliveries = (await _deliveryRepo.FindAsync(d =>
            assignmentRequestIds.Contains(d.TransportRequestId))).ToList();
        var deliveryByRequestId = deliveries.ToDictionary(d => d.TransportRequestId);

        var byTransporter = transporters.Select(t =>
        {
            var tAssignments = assignments.Where(a => a.TransporterId == t.Id).ToList();
            var tRequestIds = tAssignments.Select(a => a.TransportRequestId).ToHashSet();
            var tRequests = requests.Where(r => tRequestIds.Contains(r.Id)).ToList();

            var completed = tRequests.Count(r => r.Status == TransportStatus.Cleared);
            var onTime = tRequests.Count(r =>
                deliveryByRequestId.ContainsKey(r.Id) &&
                r.RequiredDeliveryDate.HasValue &&
                deliveryByRequestId[r.Id].DeliveryDate <= r.RequiredDeliveryDate.Value);

            var revenue = tAssignments.Sum(a =>
                rateByVehicleId.ContainsKey(a.Id)
                    ? rateByVehicleId[a.Id].ApprovedAmount ?? rateByVehicleId[a.Id].TotalRate
                    : 0);

            return new TransporterMetricDto
            {
                TransporterId = t.Id,
                TransporterName = t.TransporterName,
                TotalJobs = tRequests.Count,
                CompletedJobs = completed,
                OnTimeDeliveries = onTime,
                OnTimePercentage = tRequests.Count > 0 ? (decimal)onTime / tRequests.Count * 100 : 0,
                AverageRating = t.Rating,
                TotalRevenue = revenue,
                ComplianceScore = 0
            };
        })
        .Where(t => t.TotalJobs > 0)
        .OrderByDescending(t => t.TotalJobs)
        .ToList();

        return new TransporterPerformanceReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalTransporters = transporters.Count,
            ActiveTransporters = byTransporter.Count,
            AverageRating = byTransporter.Count > 0 ? byTransporter.Average(t => t.AverageRating) : 0,
            AverageOnTimePercentage = byTransporter.Count > 0 ? byTransporter.Average(t => t.OnTimePercentage) : 0,
            TopPerformers = byTransporter.Take(10).ToList(),
            NeedsImprovement = byTransporter.Where(t => t.OnTimePercentage < 80).ToList()
        };
    }

    public async Task<BranchComparisonReportDto> GetBranchComparisonAsync(ReportFilterDto filter)
    {
        var requests = (await _requestRepo.FindAsync(r =>
            r.CreatedDate >= filter.FromDate && r.CreatedDate <= filter.ToDate &&
            (filter.CountryCode == null || r.CountryCode == filter.CountryCode))).ToList();

        // Get deliveries for on-time
        var requestIds = requests.Select(r => r.Id).ToHashSet();
        var deliveries = (await _deliveryRepo.FindAsync(d =>
            requestIds.Contains(d.TransportRequestId))).ToList();
        var deliveryByRequestId = deliveries.ToDictionary(d => d.TransportRequestId);

        // Get rates for revenue
        var vehicles = (await _vehicleAssignmentRepo.FindAsync(v => requestIds.Contains(v.TransportRequestId))).ToList();
        var vehicleIds = vehicles.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();
        var freightByRequest = vehicles
            .GroupJoin(rates, v => v.Id, r => r.TransportVehicleId, (v, rs) => new
            {
                v.TransportRequestId,
                Freight = rs.Sum(r => r.ApprovedAmount ?? r.TotalRate)
            })
            .GroupBy(x => x.TransportRequestId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Freight));

        var byBranch = requests
            .GroupBy(r => r.BranchId.ToString())
            .Select(g =>
            {
                var branchRequests = g.ToList();
                var completed = branchRequests.Count(r => r.Status == TransportStatus.Cleared);
                var onTime = branchRequests.Count(r =>
                    deliveryByRequestId.ContainsKey(r.Id) &&
                    r.RequiredDeliveryDate.HasValue &&
                    deliveryByRequestId[r.Id].DeliveryDate <= r.RequiredDeliveryDate.Value);
                var totalRevenue = branchRequests.Sum(r => freightByRequest.GetValueOrDefault(r.Id, 0));

                return new BranchMetricDto
                {
                    BranchId = g.Key,
                    TotalJobs = branchRequests.Count,
                    CompletedJobs = completed,
                    OnTimePercentage = branchRequests.Count > 0
                        ? (decimal)onTime / branchRequests.Count * 100 : 0,
                    TotalRevenue = totalRevenue,
                    AverageJobValue = branchRequests.Count > 0
                        ? totalRevenue / branchRequests.Count : 0
                };
            })
            .OrderByDescending(b => b.TotalRevenue)
            .ToList();

        return new BranchComparisonReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalBranches = byBranch.Count,
            ByBranch = byBranch,
            TopBranchByRevenue = byBranch.FirstOrDefault()?.BranchId,
            TopBranchByVolume = byBranch.OrderByDescending(b => b.TotalJobs).FirstOrDefault()?.BranchId
        };
    }

    // ════════════════════════════════════════════════════════════
    //  LEGACY GAP REPORTS (DSR, Labour, Vehicle Closing, Weekly Trip, Daily Expense)
    // ════════════════════════════════════════════════════════════

    public async Task<DailyStatusReportDto> GetDailyStatusReportAsync(DsrFilterDto filter)
    {
        var requests = (await _requestRepo.FindAsync(BuildRequestPredicate(filter))).ToList();
        var requestIds = requests.Select(r => r.Id).ToHashSet();

        var dto = new DailyStatusReportDto
        {
            ReportDate = filter.FromDate,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate
        };

        // Transport metrics
        if (filter.IncludeTransport)
        {
            dto.TotalActiveJobs = requests.Count(r => r.Status != TransportStatus.Cancelled && r.Status != TransportStatus.Cleared);
            dto.JobsInTransit = requests.Count(r => r.Status == TransportStatus.InTransit);
            dto.JobsInWarehouse = requests.Count(r => r.Status == TransportStatus.InWarehouse);
            dto.JobsDeliveredToday = requests.Count(r => r.Status == TransportStatus.Delivered);
            dto.JobsClearedToday = requests.Count(r => r.Status == TransportStatus.Cleared);
            dto.PendingApprovals = requests.Count(r => r.Status == TransportStatus.RateApproval);
        }

        // Fleet metrics
        if (filter.IncludeFleet)
        {
            var allVehicles = (await _fleetVehicleRepo.FindAsync(v => !v.IsDeleted)).ToList();
            dto.TotalFleetVehicles = allVehicles.Count;
            dto.VehiclesOnTrip = allVehicles.Count(v => v.CurrentStatus == FleetVehicleStatus.OnTrip);
            dto.VehiclesAvailable = allVehicles.Count(v => v.IsActive && v.CurrentStatus == FleetVehicleStatus.Available);
            dto.VehiclesUnderMaintenance = allVehicles.Count(v => v.CurrentStatus == FleetVehicleStatus.UnderMaintenance);
        }

        // Maintenance metrics
        if (filter.IncludeMaintenance)
        {
            var workOrders = (await _maintenanceRepo.FindAsync(w =>
                w.CreatedDate >= filter.FromDate && w.CreatedDate <= filter.ToDate)).ToList();
            dto.ActiveWorkOrders = workOrders.Count(w => w.Status == MaintenanceStatus.InProgress);
            dto.CompletedWorkOrdersToday = workOrders.Count(w =>
                w.Status == MaintenanceStatus.Completed && w.CompletedDate?.Date == filter.FromDate.Date);
            dto.MaintenanceCostToday = workOrders.Sum(w => w.ActualCost ?? 0);
        }

        // Expense metrics
        if (filter.IncludeExpenses)
        {
            var expenses = (await _expenseRepo.FindAsync(e =>
                e.ExpenseDate >= filter.FromDate && e.ExpenseDate <= filter.ToDate &&
                requestIds.Contains(e.TransportRequestId))).ToList();
            dto.TotalExpensesToday = expenses.Sum(e => e.Amount);
            dto.PendingExpenseApprovals = expenses.Count(e =>
                e.ApprovalStatus == ExpenseApprovalStatus.Pending);
        }

        // Job summaries (top 50)
        dto.JobDetails = requests.Take(50).Select(r => new DsrJobSummaryDto
        {
            JobId = r.Id,
            RequestNumber = r.RequestNumber,
            CustomerName = r.CustomerName,
            Status = r.Status.ToString()
        }).ToList();

        return dto;
    }

    public async Task<LabourReportDto> GetLabourReportAsync(ReportFilterDto filter)
    {
        // Labour from maintenance work orders (service provider labour)
        var workOrders = (await _maintenanceRepo.FindAsync(w =>
            w.CreatedDate >= filter.FromDate && w.CreatedDate <= filter.ToDate &&
            w.Status == MaintenanceStatus.Completed)).ToList();

        if (filter.BranchId.HasValue)
        {
            // Filter by fleet vehicle branch (simplified)
            var branchVehicleIds = (await _fleetVehicleRepo.FindAsync(v =>
                v.BranchId == filter.BranchId && !v.IsDeleted))
                .Select(v => v.Id).ToHashSet();
            workOrders = workOrders.Where(w => branchVehicleIds.Contains(w.FleetVehicleId)).ToList();
        }

        var vehicles = (await _fleetVehicleRepo.FindAsync(v => !v.IsDeleted)).ToDictionary(v => v.Id);

        var details = workOrders.Select(w =>
        {
            vehicles.TryGetValue(w.FleetVehicleId, out var vehicle);
            return new LabourDetailDto
            {
                WorkOrderId = w.Id,
                WorkOrderNumber = w.WorkOrderNumber,
                VehicleId = w.FleetVehicleId,
                RegistrationNumber = vehicle?.RegistrationNumber,
                MaintenanceType = w.MaintenanceType.ToString(),
                LabourCost = w.ActualCost ?? 0,
                LabourHours = (int)(w.ActualHours ?? 0),
                ServiceProviderName = w.ServiceProviderName,
                CompletedDate = w.CompletedDate ?? w.CreatedDate
            };
        }).ToList();

        return new LabourReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalWorkOrders = details.Count,
            TotalLabourCost = details.Sum(d => d.LabourCost),
            TotalLabourHours = details.Sum(d => d.LabourHours),
            AverageLabourCostPerOrder = details.Count > 0 ? details.Sum(d => d.LabourCost) / details.Count : 0,
            ByWorkOrder = details
        };
    }

    public async Task<VehicleClosingReportDto> GetVehicleClosingReportAsync(ReportFilterDto filter)
    {
        var vehicles = (await _fleetVehicleRepo.FindAsync(v => !v.IsDeleted)).ToList();
        var travelLogs = (await _travelLogRepo.FindAsync(t =>
            t.TripDate >= filter.FromDate && t.TripDate <= filter.ToDate)).ToList();
        var maintenanceOrders = (await _maintenanceRepo.FindAsync(m =>
            m.CreatedDate >= filter.FromDate && m.CreatedDate <= filter.ToDate)).ToList();

        var details = vehicles.Select(v =>
        {
            var logs = travelLogs.Where(t => t.FleetVehicleId == v.Id).ToList();
            var maint = maintenanceOrders.Where(m => m.FleetVehicleId == v.Id).ToList();
            var fuelCost = logs.Sum(t => t.FuelCost ?? 0);
            var maintCost = maint.Sum(m => m.ActualCost ?? 0);
            var tollCost = 0m; // Would need expense join for tolls

            return new VehicleClosingDetailDto
            {
                VehicleId = v.Id,
                RegistrationNumber = v.RegistrationNumber,
                VehicleType = v.VehicleType.ToString(),
                TotalTrips = logs.Count,
                TotalKm = logs.Sum(t => t.DistanceKm),
                Revenue = 0, // Revenue per vehicle requires Rate join
                FuelCost = fuelCost,
                TollCost = tollCost,
                MaintenanceCost = maintCost,
                OtherExpenses = 0,
                TotalExpenses = fuelCost + maintCost + tollCost,
                NetProfit = 0 - fuelCost - maintCost, // Revenue=0 placeholder
                CostPerKm = logs.Sum(t => t.DistanceKm) > 0
                    ? (fuelCost + maintCost) / logs.Sum(t => t.DistanceKm) : 0
            };
        }).ToList();

        return new VehicleClosingReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalVehicles = details.Count,
            TotalRevenue = details.Sum(d => d.Revenue),
            TotalExpenses = details.Sum(d => d.TotalExpenses),
            TotalMaintenanceCost = details.Sum(d => d.MaintenanceCost),
            NetProfit = details.Sum(d => d.NetProfit),
            ByVehicle = details
        };
    }

    public async Task<WeeklyTripReportDto> GetWeeklyTripReportAsync(ReportFilterDto filter)
    {
        var travelLogs = (await _travelLogRepo.FindAsync(t =>
            t.TripDate >= filter.FromDate && t.TripDate <= filter.ToDate)).ToList();

        // Get freight from transport requests
        var requestIds = travelLogs.Where(t => t.TransportRequestId.HasValue)
            .Select(t => t.TransportRequestId!.Value).Distinct().ToHashSet();
        var vehicleAssignments = (await _vehicleAssignmentRepo.FindAsync(v =>
            requestIds.Contains(v.TransportRequestId))).ToList();
        var vehicleIds = vehicleAssignments.Select(v => v.Id).ToHashSet();
        var rates = (await _rateRepo.FindAsync(r => vehicleIds.Contains(r.TransportVehicleId))).ToList();
        var totalFreight = rates.Sum(r => r.ApprovedAmount ?? r.TotalRate);

        // Group by week
        var byWeek = travelLogs
            .GroupBy(t => System.Globalization.CultureInfo.CurrentCulture.Calendar
                .GetWeekOfYear(t.TripDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var weekLogs = g.OrderBy(t => t.TripDate).ToList();
                return new WeeklyTripDetailDto
                {
                    WeekStartDate = weekLogs.First().TripDate.Date,
                    WeekEndDate = weekLogs.Last().TripDate.Date,
                    WeekNumber = g.Key,
                    TripCount = weekLogs.Count,
                    TotalKm = weekLogs.Sum(t => t.DistanceKm),
                    TotalFreight = 0, // distribute evenly as approx
                    CompletedTrips = weekLogs.Count(t => t.ArrivalTime.HasValue),
                    CancelledTrips = 0,
                    OnTimePercentage = weekLogs.Count > 0
                        ? (decimal)weekLogs.Count(t => t.ArrivalTime.HasValue) / weekLogs.Count * 100 : 0
                };
            }).ToList();

        return new WeeklyTripReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalTrips = travelLogs.Count,
            TotalKm = travelLogs.Sum(t => t.DistanceKm),
            TotalFreight = totalFreight,
            ByWeek = byWeek
        };
    }

    public async Task<VehicleDailyExpenseReportDto> GetVehicleDailyExpenseReportAsync(ReportFilterDto filter)
    {
        var dailyExpenses = (await _dailyExpenseRepo.FindAsync(e =>
            e.ExpenseDate >= filter.FromDate && e.ExpenseDate <= filter.ToDate &&
            !e.IsDeleted)).ToList();

        var grandTotal = dailyExpenses.Sum(e => e.TotalAmount);

        // Category totals
        var categorySummaries = new List<DailyExpenseCategorySummaryDto>
        {
            new() { Category = "Fuel", TotalAmount = dailyExpenses.Sum(e => e.Fuel), RecordCount = dailyExpenses.Count(e => e.Fuel > 0) },
            new() { Category = "Fuel2", TotalAmount = dailyExpenses.Sum(e => e.Fuel2), RecordCount = dailyExpenses.Count(e => e.Fuel2 > 0) },
            new() { Category = "TollCharges", TotalAmount = dailyExpenses.Sum(e => e.TollCharges), RecordCount = dailyExpenses.Count(e => e.TollCharges > 0) },
            new() { Category = "Fines", TotalAmount = dailyExpenses.Sum(e => e.Fines), RecordCount = dailyExpenses.Count(e => e.Fines > 0) },
            new() { Category = "Parking", TotalAmount = dailyExpenses.Sum(e => e.Parking), RecordCount = dailyExpenses.Count(e => e.Parking > 0) },
            new() { Category = "Garage", TotalAmount = dailyExpenses.Sum(e => e.Garage), RecordCount = dailyExpenses.Count(e => e.Garage > 0) },
            new() { Category = "Bhatta", TotalAmount = dailyExpenses.Sum(e => e.Bhatta), RecordCount = dailyExpenses.Count(e => e.Bhatta > 0) },
            new() { Category = "ODCOverweight", TotalAmount = dailyExpenses.Sum(e => e.ODCOverweight), RecordCount = dailyExpenses.Count(e => e.ODCOverweight > 0) },
            new() { Category = "OtherCharges", TotalAmount = dailyExpenses.Sum(e => e.OtherCharges), RecordCount = dailyExpenses.Count(e => e.OtherCharges > 0) },
        };
        // Compute percentage
        foreach (var cat in categorySummaries)
            cat.Percentage = grandTotal > 0 ? cat.TotalAmount / grandTotal * 100 : 0;

        // Per-vehicle summary
        var vehicles = (await _fleetVehicleRepo.FindAsync(v => !v.IsDeleted)).ToDictionary(v => v.Id);
        var vehicleSummaries = dailyExpenses
            .GroupBy(e => e.FleetVehicleId)
            .Select(g =>
            {
                vehicles.TryGetValue(g.Key, out var vehicle);
                return new VehicleExpenseSummaryDto
                {
                    VehicleId = g.Key,
                    RegistrationNumber = vehicle?.RegistrationNumber ?? "Unknown",
                    DayCount = g.Select(e => e.ExpenseDate.Date).Distinct().Count(),
                    TotalFuel = g.Sum(e => e.Fuel + e.Fuel2),
                    TotalToll = g.Sum(e => e.TollCharges),
                    TotalOther = g.Sum(e => e.Fines + e.Parking + e.Garage + e.Bhatta +
                        e.ODCOverweight + e.OtherCharges + e.Xerox + e.VaraiUnloading +
                        e.EmptyContainer + e.DamageContainer),
                    GrandTotal = g.Sum(e => e.TotalAmount)
                };
            }).OrderByDescending(v => v.GrandTotal).ToList();

        return new VehicleDailyExpenseReportDto
        {
            ReportDate = DateTime.UtcNow,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalRecords = dailyExpenses.Count,
            GrandTotal = grandTotal,
            ByCategory = categorySummaries,
            ByVehicle = vehicleSummaries
        };
    }

    // ════════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════════

    private static System.Linq.Expressions.Expression<Func<TransportRequest, bool>> BuildRequestPredicate(
        ReportFilterDto filter)
    {
        return r =>
            r.CreatedDate >= filter.FromDate && r.CreatedDate <= filter.ToDate &&
            (filter.BranchId == null || r.BranchId == filter.BranchId) &&
            (filter.CountryCode == null || r.CountryCode == filter.CountryCode) &&
            (filter.CustomerId == null || r.CustomerId == filter.CustomerId);
    }
}
