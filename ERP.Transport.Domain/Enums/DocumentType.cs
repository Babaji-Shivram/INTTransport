namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Type of document uploaded against a transport job.
/// </summary>
public enum DocumentType
{
    POD = 0,
    LR = 1,
    Challan = 2,
    MemoCopy = 3,
    EWayBill = 4,
    Invoice = 5,
    Insurance = 6,
    RCBook = 7,
    Other = 99
}
