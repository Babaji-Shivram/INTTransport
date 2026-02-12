namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Stamp Duty DTOs (Legacy: insStampDutyDetail, updStampDutyAmnt)
// ═══════════════════════════════════════════════════════════════

public class StampDutyDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = null!;
    public Guid? TransportRequestId { get; set; }
    public Guid? TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string? DocumentType { get; set; }
    public decimal StampDutyAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime DutyDate { get; set; }
    public string? StateCode { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? ReceiptDocumentUrl { get; set; }
    public string? Remarks { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? PaidByName { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public DateTime CreatedDate { get; set; }
}

public class CreateStampDutyDto
{
    public Guid? TransportRequestId { get; set; }
    public Guid? TransporterId { get; set; }
    public string? DocumentType { get; set; }
    public decimal StampDutyAmount { get; set; }
    public DateTime DutyDate { get; set; }
    public string? StateCode { get; set; }
    public string? Remarks { get; set; }
    public Guid BranchId { get; set; }
    public string CountryCode { get; set; } = "IN";
}

public class UpdateStampDutyDto
{
    public decimal? StampDutyAmount { get; set; }
    public string? Remarks { get; set; }
}

public class RecordStampDutyPaymentDto
{
    public decimal PaidAmount { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? ReceiptDocumentUrl { get; set; }
    public string? PaidByName { get; set; }
}

public class StampDutyFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? TransportRequestId { get; set; }
    public Guid? TransporterId { get; set; }
    public bool? IsPaid { get; set; }
    public string? StateCode { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? BranchId { get; set; }
}
