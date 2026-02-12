namespace ERP.Transport.Application.DTOs;

// ═════════════════════════════════════════════════════════════════
// CharteredInfo e-Invoice + GST DTOs
// ═════════════════════════════════════════════════════════════════

// ── Auth ────────────────────────────────────────────────────────

public class CharteredInfoAuthResponseDto
{
    public string? AuthToken { get; set; }
    public string? TokenExpiry { get; set; }
    public int Status { get; set; }
}

// ── GST Lookup ──────────────────────────────────────────────────

public class GstDetailDto
{
    public Guid Id { get; set; }
    public string Gstin { get; set; } = null!;
    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public string? BusinessType { get; set; }
    public string? GstinStatus { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string? Address { get; set; }
    public string? StateCode { get; set; }
    public string? StateName { get; set; }
    public string? Pincode { get; set; }
    public string? NatureOfBusiness { get; set; }
    public string? ConstitutionOfBusiness { get; set; }
    public string? TaxpayerType { get; set; }
    public bool? IsEInvoiceApplicable { get; set; }
    public bool? IsEWayBillApplicable { get; set; }
    public DateTime LastFetchedFromApi { get; set; }

    // Computed
    public bool IsActive => GstinStatus?.Equals("Active", StringComparison.OrdinalIgnoreCase) == true;
}

public class GstLookupRequestDto
{
    public string Gstin { get; set; } = null!;
    public bool ForceRefresh { get; set; }
}

// ── E-Invoice (IRN) ─────────────────────────────────────────────

public class EInvoiceDto
{
    public Guid Id { get; set; }
    public Guid? TransportRequestId { get; set; }
    public string? Irn { get; set; }
    public string? AckNumber { get; set; }
    public DateTime? AckDate { get; set; }
    public string? EInvoiceStatus { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? DocumentDate { get; set; }
    public string? SellerGstin { get; set; }
    public string? SellerLegalName { get; set; }
    public string? BuyerGstin { get; set; }
    public string? BuyerLegalName { get; set; }
    public decimal? TotalAssessableValue { get; set; }
    public decimal? TotalCgstValue { get; set; }
    public decimal? TotalSgstValue { get; set; }
    public decimal? TotalIgstValue { get; set; }
    public decimal? TotalInvoiceValue { get; set; }
    public long? EwbNumber { get; set; }
    public DateTime? EwbDate { get; set; }
    public DateTime? EwbValidTill { get; set; }
    public string? QrCodeImageBase64 { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CancelledDate { get; set; }

    // Computed
    public bool IsCancelled => EInvoiceStatus == "CANCELLED";
}

// ── Generate IRN Request (matches CharteredInfo schema) ─────────

public class GenerateIrnRequestDto
{
    public Guid? TransportRequestId { get; set; }

    // Transaction Details
    public string SupplyType { get; set; } = "B2B"; // B2B, B2C, SEZWP, SEZWOP, EXPWP, EXPWOP
    public string DocumentType { get; set; } = "INV"; // INV, CRN, DBN

    // Document
    public string DocumentNumber { get; set; } = null!;
    public string DocumentDate { get; set; } = null!; // DD/MM/YYYY

    // Seller
    public string SellerGstin { get; set; } = null!;
    public string SellerLegalName { get; set; } = null!;
    public string? SellerTradeName { get; set; }
    public string SellerAddress1 { get; set; } = null!;
    public string? SellerAddress2 { get; set; }
    public string SellerLocation { get; set; } = null!;
    public int SellerPincode { get; set; }
    public string SellerStateCode { get; set; } = null!;
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

    // Buyer
    public string BuyerGstin { get; set; } = null!;
    public string BuyerLegalName { get; set; } = null!;
    public string? BuyerTradeName { get; set; }
    public string? BuyerPos { get; set; }
    public string BuyerAddress1 { get; set; } = null!;
    public string? BuyerAddress2 { get; set; }
    public string BuyerLocation { get; set; } = null!;
    public int BuyerPincode { get; set; }
    public string BuyerStateCode { get; set; } = null!;
    public string? BuyerPhone { get; set; }
    public string? BuyerEmail { get; set; }

    // Items
    public List<InvoiceItemDto> Items { get; set; } = new();

    // Value Summary
    public decimal AssessableValue { get; set; }
    public decimal CgstValue { get; set; }
    public decimal SgstValue { get; set; }
    public decimal IgstValue { get; set; }
    public decimal CessValue { get; set; }
    public decimal TotalInvoiceValue { get; set; }
    public decimal? Discount { get; set; }
    public decimal? OtherCharges { get; set; }
    public decimal? RoundOffAmount { get; set; }

    // E-Way Bill (optional — generate EWB along with IRN)
    public EwbDetailsDto? EwbDetails { get; set; }
}

public class InvoiceItemDto
{
    public string SlNo { get; set; } = "1";
    public string ProductDescription { get; set; } = null!;
    public string IsService { get; set; } = "N";
    public string HsnCode { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal AssessableAmount { get; set; }
    public decimal GstRate { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal? CessRate { get; set; }
    public decimal? CessAmount { get; set; }
    public decimal TotalItemValue { get; set; }
}

public class EwbDetailsDto
{
    public string? TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public int Distance { get; set; }
    public string? TransDocNo { get; set; }
    public string? TransDocDt { get; set; }
    public string? VehNo { get; set; }
    public string VehType { get; set; } = "R"; // R=Regular, O=ODC
    public string TransMode { get; set; } = "1"; // 1=Road, 2=Rail, 3=Air, 4=Ship
}

// ── Cancel IRN ──────────────────────────────────────────────────

public class CancelIrnRequestDto
{
    public string Irn { get; set; } = null!;
    public string CancelReason { get; set; } = "1"; // 1=Duplicate, 2=DataEntryError, 3=OrderCancelled, 4=Other
    public string CancelRemarks { get; set; } = null!;
}

// ── Generate E-Way Bill (separate, after IRN) ───────────────────

public class GenerateEwbFromIrnRequestDto
{
    public string Irn { get; set; } = null!;
    public int Distance { get; set; }
    public string TransMode { get; set; } = "1";
    public string? TransporterId { get; set; }
    public string? TransporterName { get; set; }
    public string? TransDocDt { get; set; }
    public string? TransDocNo { get; set; }
    public string? VehNo { get; set; }
    public string VehType { get; set; } = "R";
}

// ── Cancel E-Way Bill ───────────────────────────────────────────

public class CancelEwbRequestDto
{
    public long EwbNumber { get; set; }
    public int CancelReasonCode { get; set; } // 1=Duplicate, 2=DataEntryError, 3=OrderCancelled, 4=Other
    public string CancelRemarks { get; set; } = null!;
}

// ── QR Code Request ─────────────────────────────────────────────

public class DynamicQrRequestDto
{
    public string UpiId { get; set; } = null!;
    public string PayeeName { get; set; } = null!;
    public string BankAccountNumber { get; set; } = null!;
    public string IfscCode { get; set; } = null!;
    public string InvoiceNumber { get; set; } = null!;
    public string InvoiceDate { get; set; } = null!;
    public decimal InvoiceAmount { get; set; }
    public int QrCodeSize { get; set; } = 200;
}

public class DynamicQrResponseDto
{
    public string? QrCodeImageBase64 { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

// ── Health ──────────────────────────────────────────────────────

public class CharteredInfoHealthDto
{
    public bool IsConnected { get; set; }
    public bool HasValidToken { get; set; }
    public string? TokenExpiresAt { get; set; }
    public string? SandboxUrl { get; set; }
    public string? Message { get; set; }
}
