using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// The main transport job record — every transport job starts as a TransportRequest.
/// Created from CRM Enquiry, Freight Job, or Standalone.
/// </summary>
public class TransportRequest : BaseEntity
{
    // ── Identification ──────────────────────────────────────────
    /// <summary>Auto-generated: TR-{Year}-{Country}-{Seq} e.g. TR-2026-IN-000123</summary>
    public string RequestNumber { get; set; } = null!;
    public DateTime RequestDate { get; set; }

    // ── Source Linking ───────────────────────────────────────────
    public JobSource Source { get; set; }
    /// <summary>CRM Enquiry ID or Freight Job ID (nullable for standalone)</summary>
    public Guid? SourceReferenceId { get; set; }
    /// <summary>Display reference e.g. ENQ-2026-000456</summary>
    public string? SourceReferenceNumber { get; set; }

    // ── Customer ────────────────────────────────────────────────
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? GSTNumber { get; set; }

    // ── Origin / Destination ────────────────────────────────────
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

    // ── Cargo ───────────────────────────────────────────────────
    public CargoType CargoType { get; set; }
    public string? CargoDescription { get; set; }
    public decimal GrossWeightKg { get; set; }
    public int NumberOfPackages { get; set; }
    public int Container20Count { get; set; }
    public int Container40Count { get; set; }

    // ── Vehicle / Delivery ──────────────────────────────────────
    public VehicleTypeEnum VehicleTypeRequired { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
    public Priority Priority { get; set; }
    public string? SpecialInstructions { get; set; }

    // ── Organisation Scoping ────────────────────────────────────
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public string CountryCode { get; set; } = null!;
    public string? Division { get; set; }
    public string? Plant { get; set; }

    // ── Workflow ─────────────────────────────────────────────────
    public TransportStatus Status { get; set; }
    public Guid? WorkflowInstanceId { get; set; }
    public string? WorkflowStatus { get; set; }
    public Guid? WorkflowStepId { get; set; }

    /// <summary>
    /// Stores dynamic form field values from Workflow-defined fields as JSON.
    /// Enables Workflow admin to add/remove fields without requiring DB migrations.
    /// Format: { "FieldCode": "value", "FieldCode2": "value2" }
    /// </summary>
    public string? FormData { get; set; }

    // ── Consolidation ───────────────────────────────────────────
    public Guid? ConsolidatedTripId { get; set; }
    public bool IsConsolidated { get; set; }
    public ConsolidatedTrip? ConsolidatedTrip { get; set; }

    // ── Navigation ──────────────────────────────────────────────
    public ICollection<TransportRequestDetail> Details { get; set; } = new List<TransportRequestDetail>();
    public ICollection<TransportVehicle> Vehicles { get; set; } = new List<TransportVehicle>();
    public ICollection<TransportMovement> Movements { get; set; } = new List<TransportMovement>();
    public TransportDelivery? Delivery { get; set; }
    public ICollection<TransportDocument> Documents { get; set; } = new List<TransportDocument>();
    public ICollection<TransportExpense> Expenses { get; set; } = new List<TransportExpense>();
    public ICollection<TransitWarehouse> TransitWarehouses { get; set; } = new List<TransitWarehouse>();
    public ICollection<StampDuty> StampDuties { get; set; } = new List<StampDuty>();
}
