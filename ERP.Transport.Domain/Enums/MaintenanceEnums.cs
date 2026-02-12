namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Status of a maintenance work order.
/// </summary>
public enum MaintenanceStatus
{
    Scheduled = 0,
    InProgress = 1,
    PendingParts = 2,
    Completed = 3,
    Cancelled = 4
}

/// <summary>
/// Type of maintenance activity.
/// </summary>
public enum MaintenanceType
{
    Preventive = 0,
    Corrective = 1,
    Emergency = 2,
    Inspection = 3,
    TyreChange = 4,
    OilChange = 5,
    BrakeService = 6,
    EngineOverhaul = 7,
    BodyRepair = 8,
    ElectricalRepair = 9,
    ACService = 10,
    Other = 99
}

/// <summary>
/// Priority of maintenance work.
/// </summary>
public enum MaintenancePriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

/// <summary>
/// Type of document attached to a maintenance work order.
/// </summary>
public enum MaintenanceDocumentType
{
    ServiceReport = 0,
    Invoice = 1,
    Photo = 2,
    WarrantyCard = 3,
    InspectionReport = 4,
    InsuranceClaim = 5,
    TyreReport = 6,
    DiagnosticReport = 7,
    Other = 99
}
