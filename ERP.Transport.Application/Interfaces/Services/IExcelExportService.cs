using ERP.Transport.Application.DTOs.Common;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Excel export service — generates .xlsx files from transport data / reports.
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Export a list of transport jobs to Excel.
    /// </summary>
    Task<(byte[] FileBytes, string FileName)> ExportJobsAsync(
        TransportJobFilterDto filter, CancellationToken ct = default);

    /// <summary>
    /// Export a generic report (job summary, expenses, etc.) to Excel.
    /// Takes a report name and a collection of row objects.
    /// </summary>
    (byte[] FileBytes, string FileName) ExportReport<T>(
        string reportName, IEnumerable<T> rows) where T : class;
}
