using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Dashboard aggregation service.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IRepository<Transporter> _transporterRepo;

    public DashboardService(
        IRepository<TransportRequest> jobRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IRepository<Transporter> transporterRepo)
    {
        _jobRepo = jobRepo;
        _vehicleRepo = vehicleRepo;
        _transporterRepo = transporterRepo;
    }

    public async Task<DashboardDto> GetDashboardAsync(
        Guid userId, string? countryCode, Guid? branchId)
    {
        var today = DateTime.UtcNow.Date;

        var pipeline = new PipelineFunnelDto
        {
            RequestCreated = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.RequestCreated &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            RequestReceived = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.RequestReceived &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            VehicleAssigned = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.VehicleAssigned &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            RateEntered = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.RateEntered &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            RateApproval = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.RateApproval &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            InTransit = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.InTransit &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            InWarehouse = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.InWarehouse &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            Delivered = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.Delivered &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            Cleared = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.Cleared &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId))
        };

        var todaySummary = new TodaySummaryDto
        {
            NewRequests = await _jobRepo.CountAsync(j =>
                j.RequestDate >= today &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            VehiclesOut = await _jobRepo.CountAsync(j =>
                j.Status == TransportStatus.InTransit &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId)),
            DeliveriesExpected = await _jobRepo.CountAsync(j =>
                j.RequiredDeliveryDate.HasValue && j.RequiredDeliveryDate.Value.Date == today &&
                j.Status < TransportStatus.Delivered &&
                (countryCode == null || j.CountryCode == countryCode) &&
                (branchId == null || j.BranchId == branchId))
        };

        var overdueJobs = await _jobRepo.CountAsync(j =>
            j.RequiredDeliveryDate.HasValue && j.RequiredDeliveryDate.Value < today &&
            j.Status < TransportStatus.Delivered &&
            (countryCode == null || j.CountryCode == countryCode) &&
            (branchId == null || j.BranchId == branchId));

        var pendingApprovals = await _jobRepo.CountAsync(j =>
            j.Status == TransportStatus.RateApproval &&
            (countryCode == null || j.CountryCode == countryCode) &&
            (branchId == null || j.BranchId == branchId));

        // ── Top Transporters (by trip count) ────────────────────
        var allActiveVehicles = await _vehicleRepo.FindAsync(v =>
            v.IsActive &&
            (countryCode == null || v.TransportRequest.CountryCode == countryCode));

        var transporterGroups = allActiveVehicles
            .GroupBy(v => v.TransporterId)
            .Select(g => new
            {
                TransporterId = g.Key,
                TransporterName = g.First().TransporterName ?? "Unknown",
                TotalTrips = g.Count(),
                ActiveTrips = g.Count(v => v.TransportRequest.Status == TransportStatus.InTransit)
            })
            .OrderByDescending(t => t.TotalTrips)
            .Take(10)
            .ToList();

        var topTransporters = new List<TopTransporterDto>();
        foreach (var tg in transporterGroups)
        {
            var transporter = await _transporterRepo.GetByIdAsync(tg.TransporterId);
            topTransporters.Add(new TopTransporterDto
            {
                TransporterId = tg.TransporterId,
                TransporterName = transporter?.TransporterName ?? tg.TransporterName,
                TotalTrips = tg.TotalTrips,
                ActiveTrips = tg.ActiveTrips,
                Rating = transporter?.Rating ?? 0
            });
        }

        // ── Branch Comparison ───────────────────────────────────
        var allJobs = await _jobRepo.FindAsync(j =>
            (countryCode == null || j.CountryCode == countryCode) &&
            !j.IsDeleted);

        var branchComparison = allJobs
            .GroupBy(j => new { j.BranchId, j.BranchName })
            .Select(g => new BranchComparisonDto
            {
                BranchId = g.Key.BranchId,
                BranchName = g.Key.BranchName,
                TotalJobs = g.Count(),
                InTransit = g.Count(j => j.Status == TransportStatus.InTransit),
                Delivered = g.Count(j => j.Status == TransportStatus.Delivered ||
                                         j.Status == TransportStatus.Cleared),
                OverdueJobs = g.Count(j => j.RequiredDeliveryDate.HasValue &&
                                            j.RequiredDeliveryDate.Value < today &&
                                            j.Status < TransportStatus.Delivered),
                PendingApprovals = g.Count(j => j.Status == TransportStatus.RateApproval)
            })
            .OrderByDescending(b => b.TotalJobs)
            .ToList();

        return new DashboardDto
        {
            Pipeline = pipeline,
            TodaySummary = todaySummary,
            OverdueJobs = overdueJobs,
            PendingApprovals = pendingApprovals,
            TopTransporters = topTransporters,
            BranchComparison = branchComparison
        };
    }
}
