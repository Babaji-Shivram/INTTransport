using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Transporter Master DTOs
// ═══════════════════════════════════════════════════════════════

public class TransporterDto
{
    public Guid Id { get; set; }
    public string TransporterName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PANNumber { get; set; }
    public string? GSTNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }
    public string? CountryCode { get; set; }
    public TransporterStatus Status { get; set; }
    public decimal Rating { get; set; }
    public Guid? BranchId { get; set; }
    public ICollection<TransporterKYCDto> KYCDocuments { get; set; } = new List<TransporterKYCDto>();
    public ICollection<TransporterBankDto> BankAccounts { get; set; } = new List<TransporterBankDto>();
    public DateTime CreatedDate { get; set; }
}

public class TransporterListDto
{
    public Guid Id { get; set; }
    public string TransporterName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public TransporterStatus Status { get; set; }
    public decimal Rating { get; set; }
    public string? City { get; set; }
    public string? CountryCode { get; set; }
}

public class CreateTransporterDto
{
    public string TransporterName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PANNumber { get; set; }
    public string? GSTNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }
    public string? CountryCode { get; set; }
    public Guid? BranchId { get; set; }
}

public class UpdateTransporterDto
{
    public string? TransporterName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PANNumber { get; set; }
    public string? GSTNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Pincode { get; set; }
    public TransporterStatus? Status { get; set; }
    public string? SuspensionReason { get; set; }
}

// ── KYC ─────────────────────────────────────────────────────────

public class TransporterKYCDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public bool IsVerified { get; set; }
}

public class AddKYCDocumentDto
{
    public string DocumentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? ContentType { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

// ── Bank ────────────────────────────────────────────────────────

public class TransporterBankDto
{
    public Guid Id { get; set; }
    public string BankName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? IFSCCode { get; set; }
    public string? BranchName { get; set; }
    public string? AccountHolderName { get; set; }
    public bool IsPrimary { get; set; }
}

public class AddBankAccountDto
{
    public string BankName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string? IFSCCode { get; set; }
    public string? BranchName { get; set; }
    public string? AccountHolderName { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateBankAccountDto
{
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? IFSCCode { get; set; }
    public string? BranchName { get; set; }
    public string? AccountHolderName { get; set; }
    public bool? IsPrimary { get; set; }
}

// ── Notification ────────────────────────────────────────────────

public class TransporterNotificationDto
{
    public Guid Id { get; set; }
    public Guid TransporterId { get; set; }
    public NotificationType Type { get; set; }
    public string? Destination { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateNotificationDto
{
    public NotificationType Type { get; set; }
    public string? Destination { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public class UpdateNotificationDto
{
    public string? Destination { get; set; }
    public bool? IsEnabled { get; set; }
}
