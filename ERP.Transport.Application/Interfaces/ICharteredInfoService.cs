using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces;

/// <summary>
/// CharteredInfo business logic — e-Invoice (IRN), GST lookup, E-Way Bill generation.
/// </summary>
public interface ICharteredInfoService
{
    /// <summary>Lookup GSTIN details (cached 24h).</summary>
    Task<GstDetailDto> LookupGstAsync(GstLookupRequestDto request);

    /// <summary>Generate e-Invoice (IRN) via CharteredInfo.</summary>
    Task<EInvoiceDto> GenerateIrnAsync(GenerateIrnRequestDto request, Guid userId);

    /// <summary>Get IRN details by IRN hash.</summary>
    Task<EInvoiceDto?> GetIrnAsync(string irn);

    /// <summary>Get IRN by document details.</summary>
    Task<EInvoiceDto?> GetIrnByDocumentAsync(string docType, string docNum, string docDate);

    /// <summary>Cancel an IRN.</summary>
    Task<EInvoiceDto> CancelIrnAsync(CancelIrnRequestDto request, Guid userId);

    /// <summary>Generate E-Way Bill from an existing IRN.</summary>
    Task<EInvoiceDto> GenerateEwbFromIrnAsync(GenerateEwbFromIrnRequestDto request, Guid userId);

    /// <summary>Cancel E-Way Bill.</summary>
    Task CancelEwbAsync(CancelEwbRequestDto request);

    /// <summary>Get E-Invoice(s) for a transport job.</summary>
    Task<IEnumerable<EInvoiceDto>> GetByTransportJobAsync(Guid transportRequestId);

    /// <summary>Generate dynamic QR code.</summary>
    Task<DynamicQrResponseDto> GetDynamicQrAsync(DynamicQrRequestDto request);

    /// <summary>Health check.</summary>
    Task<CharteredInfoHealthDto> HealthCheckAsync();
}
