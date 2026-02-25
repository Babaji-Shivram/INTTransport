using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Job;

// ═══════════════════════════════════════════════════════════════
//  Create / Update Job DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>Create a standalone transport job.</summary>
public class CreateTransportJobDto
{
    // Customer
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? GSTNumber { get; set; }

    // Origin
    public Guid? OriginLocationId { get; set; }
    public string? OriginLocationName { get; set; }
    public string PickupAddress { get; set; } = null!;
    public string? PickupCity { get; set; }
    public string? PickupState { get; set; }
    public string? PickupPincode { get; set; }

    // Destination
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

    // Vehicle
    public VehicleTypeEnum VehicleTypeRequired { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public Priority Priority { get; set; }
    public string? SpecialInstructions { get; set; }

    // Organisation
    public string? Division { get; set; }
    public string? Plant { get; set; }
}

/// <summary>Create job from CRM enquiry (MS-to-MS).</summary>
public class CreateJobFromEnquiryDto
{
    public Guid EnquiryId { get; set; }
    public string EnquiryReferenceNumber { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? GSTNumber { get; set; }
    public Guid? OriginLocationId { get; set; }
    public string? OriginLocationName { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }
    public string PickupAddress { get; set; } = null!;
    public string DropAddress { get; set; } = null!;
    public CargoType CargoType { get; set; }
    public string? CargoDescription { get; set; }
    public decimal GrossWeightKg { get; set; }
    public int NumberOfPackages { get; set; }
    public int Container20Count { get; set; }
    public int Container40Count { get; set; }
    public string? SpecialInstructions { get; set; }
    public string CountryCode { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
}

/// <summary>Create job from freight job (MS-to-MS).</summary>
public class CreateJobFromFreightDto
{
    public Guid FreightJobId { get; set; }
    public string FreightJobReference { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public Guid? PickupLocationId { get; set; }
    public string? PickupLocationName { get; set; }
    public string PickupAddress { get; set; } = null!;
    public Guid? DeliveryLocationId { get; set; }
    public string? DeliveryLocationName { get; set; }
    public string DropAddress { get; set; } = null!;
    public string? CargoDescription { get; set; }
    public decimal GrossWeightKg { get; set; }
    public int Container20Count { get; set; }
    public int Container40Count { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public string CountryCode { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
}

/// <summary>Update job fields (only allowed before certain workflow steps).</summary>
public class UpdateTransportJobDto
{
    public Guid? OriginLocationId { get; set; }
    public string? OriginLocationName { get; set; }
    public string? PickupAddress { get; set; }
    public string? PickupCity { get; set; }
    public string? PickupState { get; set; }
    public string? PickupPincode { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }
    public string? DropAddress { get; set; }
    public string? DropCity { get; set; }
    public string? DropState { get; set; }
    public string? DropPincode { get; set; }
    public CargoType? CargoType { get; set; }
    public string? CargoDescription { get; set; }
    public decimal? GrossWeightKg { get; set; }
    public int? NumberOfPackages { get; set; }
    public int? Container20Count { get; set; }
    public int? Container40Count { get; set; }
    public VehicleTypeEnum? VehicleTypeRequired { get; set; }
    public DeliveryType? DeliveryType { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public Priority? Priority { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? Division { get; set; }
    public string? Plant { get; set; }
}
