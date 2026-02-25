namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Electronic Lorry Receipt (ELR) PDF generation service.
/// Produces a PDF document combining job details, vehicle details, and delivery proof.
/// </summary>
public interface IElrService
{
    /// <summary>
    /// Generate ELR PDF for a specific transport job.
    /// </summary>
    /// <returns>PDF file bytes and suggested filename.</returns>
    Task<(byte[] PdfBytes, string FileName)> GenerateElrPdfAsync(Guid transportRequestId, CancellationToken ct = default);
}
