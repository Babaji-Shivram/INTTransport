using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Transport Job DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>Full transport job response.</summary>
public class TransportJobDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public DateTime RequestDate { get; set; }

    // Source
    public JobSource Source { get; set; }
    public Guid? SourceReferenceId { get; set; }
    public string? SourceReferenceNumber { get; set; }

    // Customer
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? GSTNumber { get; set; }

    // Origin / Destination
    public Guid? OriginLocationId { get; set; }
    public string? OriginLocationName { get; set; }
    public string PickupAddress { get; set; } = null!;
    public string? PickupCity { get; set; }
    public string? PickupState { get; set; }
    public string? PickupPincode { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }
    public string DropAddress { get; set; } = null!;
    public string? DropCity { get; set; }
    public string? DropState { get; set; }
    public string? DropPincode { get; set; }

    // Cargo
    public CargoType CargoType { get; set; }
    public string? CargoDescription { get; set; }
    public decimal GrossWeightKg { get; set; }
    public int NumberOfPackages { get; set; }
    public int Container20Count { get; set; }
    public int Container40Count { get; set; }

    // Vehicle / Delivery
    public VehicleTypeEnum VehicleTypeRequired { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public Priority Priority { get; set; }
    public string? SpecialInstructions { get; set; }

    // Organisation
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;
    public string? Division { get; set; }
    public string? Plant { get; set; }

    // Workflow
    public TransportStatus Status { get; set; }
    public Guid? WorkflowInstanceId { get; set; }
    public string? WorkflowStatus { get; set; }

    // Consolidation
    public Guid? ConsolidatedTripId { get; set; }
    public bool IsConsolidated { get; set; }

    // Children
    public ICollection<TransportVehicleDto> Vehicles { get; set; } = new List<TransportVehicleDto>();
    public ICollection<TransportMovementDto> Movements { get; set; } = new List<TransportMovementDto>();
    public TransportDeliveryDto? Delivery { get; set; }
    public ICollection<TransportDocumentDto> Documents { get; set; } = new List<TransportDocumentDto>();

    // Audit
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

/// <summary>Light DTO for list/grid views.</summary>
public class TransportJobListDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public JobSource Source { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? OriginLocationName { get; set; }
    public string? DestinationLocationName { get; set; }
    public CargoType CargoType { get; set; }
    public VehicleTypeEnum VehicleTypeRequired { get; set; }
    public TransportStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}

/// <summary>Internal status DTO for MS-to-MS calls.</summary>
public class TransportJobStatusDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public TransportStatus Status { get; set; }
    public string? WorkflowStatus { get; set; }
}
