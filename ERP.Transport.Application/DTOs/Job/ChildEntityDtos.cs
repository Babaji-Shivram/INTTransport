using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Job;

// ═══════════════════════════════════════════════════════════════
//  Vehicle, Rate, Movement, Delivery, Document DTOs
// ═══════════════════════════════════════════════════════════════

// ── Vehicle ─────────────────────────────────────────────────────

public class TransportVehicleDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public Guid TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
    public string? MemoCopyUrl { get; set; }
    public bool IsActive { get; set; }
    public VehicleRateDto? Rate { get; set; }
    public VehicleFundRequestDto? FundRequest { get; set; }
}

public class AssignVehicleDto
{
    public Guid TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public VehicleTypeEnum VehicleType { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
}

public class UpdateVehicleAssignmentDto
{
    public string? VehicleNumber { get; set; }
    public VehicleTypeEnum? VehicleType { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string? LRNumber { get; set; }
    public DateTime? LRDate { get; set; }
}

// ── Rate ────────────────────────────────────────────────────────

public class VehicleRateDto
{
    public Guid Id { get; set; }
    public Guid TransportVehicleId { get; set; }
    public decimal FreightRate { get; set; }
    public decimal DetentionCharges { get; set; }
    public decimal VaraiCharges { get; set; }
    public decimal EmptyContainerReturn { get; set; }
    public decimal TollCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal TotalRate { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public bool IsApproved { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? ApprovalRemarks { get; set; }

    // ── Extra Fields (Legacy: VehiclePlace.aspx) ────────────────
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }
}

public class EnterRateDto
{
    public decimal FreightRate { get; set; }
    public decimal DetentionCharges { get; set; }
    public decimal VaraiCharges { get; set; }
    public decimal EmptyContainerReturn { get; set; }
    public decimal TollCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public string CurrencyCode { get; set; } = "INR";

    // ── Extra Fields (Legacy: VehiclePlace.aspx) ────────────────
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }
}

// ── Fund Request ────────────────────────────────────────────────

public class VehicleFundRequestDto
{
    public Guid Id { get; set; }
    public Guid TransportVehicleId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }
    public FundRequestStatus Status { get; set; }
    public string? Remarks { get; set; }
    public Guid? ProcessedBy { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Create an advance fund request for a vehicle.</summary>
public class CreateFundRequestDto
{
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>Approve / Reject / Process a fund request.</summary>
public class ProcessFundRequestDto
{
    public FundRequestStatus Action { get; set; }
    public string? Remarks { get; set; }
}

// ── Movement ────────────────────────────────────────────────────

public class TransportMovementDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public MovementMilestone Milestone { get; set; }
    public string? LocationName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Remarks { get; set; }
    public Guid? TransportVehicleId { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class AddMovementDto
{
    public MovementMilestone Milestone { get; set; }
    public string? LocationName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Remarks { get; set; }
    public Guid? TransportVehicleId { get; set; }
}

// ── Delivery ────────────────────────────────────────────────────

public class TransportDeliveryDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string ReceivedBy { get; set; } = null!;
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }
    public string? ChallanNumber { get; set; }
    public string? ChallanDocumentUrl { get; set; }
    public string? LRCopyUrl { get; set; }
    public string? EWayBillNumber { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? DamageNotes { get; set; }
    public string? ShortDeliveryNotes { get; set; }

    // ── Indian Compliance Fields ────────────────────────────────
    public string? LRNo { get; set; }
    public DateTime? LRDate { get; set; }
    public string? RoadPermitNo { get; set; }
    public DateTime? RoadPermitDate { get; set; }
    public string? NFormNo { get; set; }
    public DateTime? NFormDate { get; set; }
    public string? SFormNo { get; set; }
    public DateTime? SFormDate { get; set; }
    public string? OctroiReceiptNo { get; set; }
    public decimal? OctroiAmount { get; set; }
    public string? BabajiChallanNo { get; set; }
    public DateTime? BabajiChallanDate { get; set; }
    public DateTime? EmptyContReturnDate { get; set; }
    public string? DeliveryPoint { get; set; }
    public string? ContainerId { get; set; }
    public bool IsOwnFleetDelivery { get; set; }
    public string? DamageDocumentUrl { get; set; }
}

public class RecordDeliveryDto
{
    public DateTime DeliveryDate { get; set; }
    public string ReceivedBy { get; set; } = null!;
    public string? PODNumber { get; set; }
    public string? PODDocumentUrl { get; set; }
    public string? ChallanNumber { get; set; }
    public string? ChallanDocumentUrl { get; set; }
    public string? LRCopyUrl { get; set; }
    public string? EWayBillNumber { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public string? DamageNotes { get; set; }
    public string? ShortDeliveryNotes { get; set; }

    // ── Indian Compliance Fields ────────────────────────────────
    public string? LRNo { get; set; }
    public DateTime? LRDate { get; set; }
    public string? RoadPermitNo { get; set; }
    public DateTime? RoadPermitDate { get; set; }
    public string? NFormNo { get; set; }
    public DateTime? NFormDate { get; set; }
    public string? SFormNo { get; set; }
    public DateTime? SFormDate { get; set; }
    public string? OctroiReceiptNo { get; set; }
    public decimal? OctroiAmount { get; set; }
    public string? BabajiChallanNo { get; set; }
    public DateTime? BabajiChallanDate { get; set; }
    public DateTime? EmptyContReturnDate { get; set; }
    public string? DeliveryPoint { get; set; }
    public string? ContainerId { get; set; }
    public bool IsOwnFleetDelivery { get; set; }
    public string? DamageDocumentUrl { get; set; }
}

// ── Document ────────────────────────────────────────────────────

public class TransportDocumentDto
{
    public Guid Id { get; set; }
    public Guid TransportRequestId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class UploadDocumentDto
{
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public Guid? TransportVehicleId { get; set; }
}
