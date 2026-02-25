using System.Reflection;
using ClosedXML.Excel;
using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Excel export using ClosedXML. Supports job list export and generic report export.
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly IRepository<TransportRequest> _jobRepo;

    public ExcelExportService(IRepository<TransportRequest> jobRepo)
    {
        _jobRepo = jobRepo;
    }

    // ════════════════════════════════════════════════════════════
    //  EXPORT JOBS
    // ════════════════════════════════════════════════════════════

    public async Task<(byte[] FileBytes, string FileName)> ExportJobsAsync(
        TransportJobFilterDto filter, CancellationToken ct = default)
    {
        var (items, _) = await _jobRepo.GetPagedAsync(
            predicate: null,   // will get all matching soft delete filter
            page: 1,
            pageSize: 10000);  // large page for export

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Transport Jobs");

        // Headers
        var headers = new[]
        {
            "Request Number", "Request Date", "Source", "Customer",
            "Origin", "Destination", "Cargo Type", "Vehicle Type",
            "Status", "Priority", "Delivery Date", "Branch", "Country"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
        }

        // Data rows
        var row = 2;
        foreach (var job in items)
        {
            ws.Cell(row, 1).Value = job.RequestNumber;
            ws.Cell(row, 2).Value = job.RequestDate.ToString("dd-MMM-yyyy");
            ws.Cell(row, 3).Value = job.Source.ToString();
            ws.Cell(row, 4).Value = job.CustomerName;
            ws.Cell(row, 5).Value = job.OriginLocationName;
            ws.Cell(row, 6).Value = job.DestinationLocationName;
            ws.Cell(row, 7).Value = job.CargoType.ToString();
            ws.Cell(row, 8).Value = job.VehicleTypeRequired.ToString();
            ws.Cell(row, 9).Value = job.Status.ToString();
            ws.Cell(row, 10).Value = job.Priority.ToString();
            ws.Cell(row, 11).Value = job.RequiredDeliveryDate?.ToString("dd-MMM-yyyy") ?? "";
            ws.Cell(row, 12).Value = job.BranchName;
            ws.Cell(row, 13).Value = job.CountryCode;
            row++;
        }

        ws.Columns().AdjustToContents();

        var bytes = ToBytes(wb);
        var fileName = $"TransportJobs-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
        return (bytes, fileName);
    }

    // ════════════════════════════════════════════════════════════
    //  GENERIC REPORT EXPORT
    // ════════════════════════════════════════════════════════════

    public (byte[] FileBytes, string FileName) ExportReport<T>(
        string reportName, IEnumerable<T> rows) where T : class
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(reportName.Length > 31 ? reportName[..31] : reportName);

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Headers
        for (var i = 0; i < props.Length; i++)
        {
            ws.Cell(1, i + 1).Value = SplitCamelCase(props[i].Name);
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
        }

        // Data
        var row = 2;
        foreach (var item in rows)
        {
            for (var i = 0; i < props.Length; i++)
            {
                var val = props[i].GetValue(item);
                ws.Cell(row, i + 1).SetValue(FormatValue(val));
            }
            row++;
        }

        ws.Columns().AdjustToContents();

        var bytes = ToBytes(wb);
        var fileName = $"{reportName}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
        return (bytes, fileName);
    }

    // ════════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════════

    private static byte[] ToBytes(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static string FormatValue(object? val) => val switch
    {
        null => "",
        DateTime dt => dt.ToString("dd-MMM-yyyy HH:mm"),
        decimal d => d.ToString("N2"),
        Enum e => e.ToString(),
        IEnumerable<object> list => string.Join(", ", list.Select(x => x.ToString())),
        _ => val.ToString() ?? ""
    };

    private static string SplitCamelCase(string input)
    {
        return string.Concat(input.Select((c, i) =>
            i > 0 && char.IsUpper(c) && !char.IsUpper(input[i - 1]) ? " " + c : c.ToString()));
    }
}
