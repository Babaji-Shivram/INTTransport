using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Common;

// ═══════════════════════════════════════════════════════════════
//  Filter, Paging, Timeline, Dashboard, Consolidation DTOs
// ═══════════════════════════════════════════════════════════════

// ── Filter ──────────────────────────────────────────────────────

public class TransportJobFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public TransportStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public JobSource? Source { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public string? CountryCode { get; set; }
    public Guid? OriginLocationId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public bool? IsConsolidated { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class TransporterFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public TransporterStatus? Status { get; set; }
    public string? CountryCode { get; set; }
    public Guid? BranchId { get; set; }
}

// ── Paging ──────────────────────────────────────────────────────

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

// ── Timeline ────────────────────────────────────────────────────

public class JobTimelineDto
{
    public Guid JobId { get; set; }
    public string RequestNumber { get; set; } = null!;
    public ICollection<TimelineEntryDto> Entries { get; set; } = new List<TimelineEntryDto>();
}

public class TimelineEntryDto
{
    public DateTime Timestamp { get; set; }
    public string Event { get; set; } = null!;
    public string? Description { get; set; }
    public string? PerformedBy { get; set; }
}

// ── Dashboard ───────────────────────────────────────────────────

public class DashboardDto
{
    public PipelineFunnelDto Pipeline { get; set; } = new();
    public TodaySummaryDto TodaySummary { get; set; } = new();
    public int OverdueJobs { get; set; }
    public int PendingApprovals { get; set; }
    public ICollection<TopTransporterDto> TopTransporters { get; set; } = new List<TopTransporterDto>();
    public ICollection<BranchComparisonDto> BranchComparison { get; set; } = new List<BranchComparisonDto>();
}

public class PipelineFunnelDto
{
    public int RequestCreated { get; set; }
    public int RequestReceived { get; set; }
    public int VehicleAssigned { get; set; }
    public int RateEntered { get; set; }
    public int RateApproval { get; set; }
    public int InTransit { get; set; }
    public int InWarehouse { get; set; }
    public int Delivered { get; set; }
    public int Cleared { get; set; }
}

public class TodaySummaryDto
{
    public int NewRequests { get; set; }
    public int VehiclesOut { get; set; }
    public int DeliveriesExpected { get; set; }
}

public class TopTransporterDto
{
    public Guid TransporterId { get; set; }
    public string TransporterName { get; set; } = null!;
    public int TotalTrips { get; set; }
    public int ActiveTrips { get; set; }
    public decimal Rating { get; set; }
    public int OnTimeDeliveries { get; set; }
}

public class BranchComparisonDto
{
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public int TotalJobs { get; set; }
    public int InTransit { get; set; }
    public int Delivered { get; set; }
    public int OverdueJobs { get; set; }
    public int PendingApprovals { get; set; }
}
// ── Clearance Checklist ─────────────────────────────────────

/// <summary>
/// Pre-clearance checklist — all items must be true before a job can be cleared.
/// </summary>
public class ClearanceChecklistDto
{
    public Guid JobId { get; set; }
    public bool HasPODDocument { get; set; }
    public bool HasLRDocument { get; set; }
    public bool HasChallanDocument { get; set; }
    public bool HasRateEntry { get; set; }
    public bool HasRateApproval { get; set; }
    public bool HasDeliveryRecord { get; set; }
    public bool IsEligibleForClearance { get; set; }
    public ICollection<string> MissingItems { get; set; } = new List<string>();
}
// ── Consolidation ───────────────────────────────────────────────

public class ConsolidateJobsDto
{
    public ICollection<Guid> JobIds { get; set; } = new List<Guid>();
    public string? Remarks { get; set; }
}
