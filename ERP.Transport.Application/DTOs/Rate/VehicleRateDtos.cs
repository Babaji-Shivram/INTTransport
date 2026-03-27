using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Rate;

// ═══════════════════════════════════════════════════════════════
//  Vehicle Rate Management DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>Full vehicle rate response.</summary>
public class VehicleRateMasterDto
{
    public Guid Id { get; set; }
    public Guid TransportVehicleId { get; set; }
    public Guid? TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string? VehicleNumber { get; set; }

    // ── Rate Components ──────────────────────────────────────
    public decimal FreightRate { get; set; }
    public decimal DetentionCharges { get; set; }
    public decimal VaraiCharges { get; set; }
    public decimal EmptyContainerReturn { get; set; }
    public decimal TollCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal TotalRate { get; set; }
    public string CurrencyCode { get; set; } = "INR";

    // ── Extra Fields ─────────────────────────────────────────
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }

    // ── Approval ─────────────────────────────────────────────
    public bool IsApproved { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? ApprovalRemarks { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>List item for rate search results.</summary>
public class VehicleRateListDto
{
    public Guid Id { get; set; }
    public Guid TransportVehicleId { get; set; }
    public string? TransporterName { get; set; }
    public string? VehicleNumber { get; set; }
    public decimal FreightRate { get; set; }
    public decimal TotalRate { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Create a new vehicle rate entry.</summary>
public class CreateVehicleRateRequest
{
    public Guid TransportVehicleId { get; set; }
    public decimal FreightRate { get; set; }
    public decimal DetentionCharges { get; set; }
    public decimal VaraiCharges { get; set; }
    public decimal EmptyContainerReturn { get; set; }
    public decimal TollCharges { get; set; }
    public decimal OtherCharges { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }
}

/// <summary>Update an existing vehicle rate.</summary>
public class UpdateVehicleRateRequest
{
    public decimal? FreightRate { get; set; }
    public decimal? DetentionCharges { get; set; }
    public decimal? VaraiCharges { get; set; }
    public decimal? EmptyContainerReturn { get; set; }
    public decimal? TollCharges { get; set; }
    public decimal? OtherCharges { get; set; }
    public string? CurrencyCode { get; set; }
    public string? BillingInstruction { get; set; }
    public decimal? ContractPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? MarketRate { get; set; }
    public string? MemoDocumentUrl { get; set; }
}

/// <summary>Search/filter rates.</summary>
public class RateSearchRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? TransportVehicleId { get; set; }
    public Guid? TransporterId { get; set; }
    public bool? IsApproved { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? MinRate { get; set; }
    public decimal? MaxRate { get; set; }
}

/// <summary>Best rate lookup for a given vehicle assignment.</summary>
public class BestRateRequest
{
    public Guid? TransportVehicleId { get; set; }
    public Guid? TransporterId { get; set; }
}
