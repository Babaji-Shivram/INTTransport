namespace ERP.Transport.Domain.Enums;

/// <summary>
/// Status of transporter fund (advance) request.
/// </summary>
public enum FundRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Processed = 3
}
