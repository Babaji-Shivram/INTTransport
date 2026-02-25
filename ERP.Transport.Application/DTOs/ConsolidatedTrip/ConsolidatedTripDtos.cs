using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.ConsolidatedTrip;

// ═══════════════════════════════════════════════════════════════
//  Consolidated Trip DTOs
// ═══════════════════════════════════════════════════════════════

public class ConsolidatedTripDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public Guid? DestinationLocationId { get; set; }
    public string? DestinationLocationName { get; set; }
    public Guid? SharedVehicleId { get; set; }
    public string? SharedVehicleNumber { get; set; }
    public ConsolidationStatus Status { get; set; }
    public string? Remarks { get; set; }
    public int JobCount { get; set; }
    public string CountryCode { get; set; } = null!;
    public Guid BranchId { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<TransportJobListDto> Jobs { get; set; } = new List<TransportJobListDto>();
    public ICollection<ConsolidatedVehicleDto> Vehicles { get; set; } = new List<ConsolidatedVehicleDto>();
    public ICollection<ConsolidatedExpenseDto> Expenses { get; set; } = new List<ConsolidatedExpenseDto>();
    public ICollection<ConsolidatedStopDeliveryDto> StopDeliveries { get; set; } = new List<ConsolidatedStopDeliveryDto>();
}

public class ConsolidatedTripListDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public string? DestinationLocationName { get; set; }
    public string? SharedVehicleNumber { get; set; }
    public ConsolidationStatus Status { get; set; }
    public int JobCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class AssignConsolidatedVehicleDto
{
    public Guid VehicleId { get; set; }
    public string VehicleNumber { get; set; } = null!;
}

// ═══════════════════════════════════════════════════════════════
//  Consolidated Vehicle DTOs
// ═══════════════════════════════════════════════════════════════

public class ConsolidatedVehicleDto
{
    public Guid Id { get; set; }
    public Guid ConsolidatedTripId { get; set; }
    public Guid TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public decimal? FreightRate { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? OtherCharges { get; set; }
    public decimal? TotalRate { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateConsolidatedVehicleDto
{
    public Guid TransporterId { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public decimal? FreightRate { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? OtherCharges { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
    public string? MemoCopyUrl { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  Consolidated Expense DTOs
// ═══════════════════════════════════════════════════════════════

public class ConsolidatedExpenseDto
{
    public Guid Id { get; set; }
    public Guid ConsolidatedTripId { get; set; }
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Remarks { get; set; }
}

public class CreateConsolidatedExpenseDto
{
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Remarks { get; set; }
}

// ═══════════════════════════════════════════════════════════════
//  Consolidated Stop Delivery DTOs
// ═══════════════════════════════════════════════════════════════

public class ConsolidatedStopDeliveryDto
{
    public Guid Id { get; set; }
    public Guid ConsolidatedTripId { get; set; }
    public Guid TransportRequestId { get; set; }
    public string? RequestNumber { get; set; }
    public int StopSequence { get; set; }
    public string? LocationName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Pincode { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? ReceivedBy { get; set; }
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }
    public string? Remarks { get; set; }
}

public class RecordStopDeliveryDto
{
    public DateTime? ActualArrival { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? ReceivedBy { get; set; }
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }
    public string? Remarks { get; set; }
}
