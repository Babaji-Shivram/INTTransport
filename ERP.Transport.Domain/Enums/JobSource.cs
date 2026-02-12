namespace ERP.Transport.Domain.Enums;

/// <summary>
/// How the transport job was created.
/// </summary>
public enum JobSource
{
    Standalone = 0,
    CRMEnquiry = 1,
    FreightJob = 2
}
