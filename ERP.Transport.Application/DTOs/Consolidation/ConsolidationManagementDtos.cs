using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Consolidation;

// ═══════════════════════════════════════════════════════════════
//  Consolidation Management DTOs (dedicated controller)
// ═══════════════════════════════════════════════════════════════

/// <summary>Summary for consolidation list views.</summary>
public class ConsolidationSummaryDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string? DestinationLocationName { get; set; }
    public string? SharedVehicleNumber { get; set; }
    public ConsolidationStatus Status { get; set; }
    public int JobCount { get; set; }
    public decimal TotalWeight { get; set; }
    public int TotalPackages { get; set; }
    public string CountryCode { get; set; } = null!;
    public Guid BranchId { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Create a new consolidation trip.</summary>
public class CreateConsolidationRequest
{
    public ICollection<Guid> JobIds { get; set; } = new List<Guid>();
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public DateTime? PlannedDate { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>Add a job to an existing consolidation.</summary>
public class AddJobToConsolidationRequest
{
    public Guid TransportRequestId { get; set; }
}

/// <summary>Cancel a consolidation trip.</summary>
public class CancelConsolidationRequest
{
    public string Reason { get; set; } = null!;
}
