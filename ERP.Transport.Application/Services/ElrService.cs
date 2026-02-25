using AutoMapper;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Generates Electronic Lorry Receipt (ELR) PDF documents.
/// Combines: job details, vehicle/transporter/LR, rate, and delivery/POD info.
/// </summary>
public class ElrService : IElrService
{
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IRepository<VehicleRate> _rateRepo;
    private readonly IRepository<TransportDelivery> _deliveryRepo;
    private readonly IRepository<TransportMovement> _movementRepo;

    public ElrService(
        IRepository<TransportRequest> jobRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IRepository<VehicleRate> rateRepo,
        IRepository<TransportDelivery> deliveryRepo,
        IRepository<TransportMovement> movementRepo)
    {
        _jobRepo = jobRepo;
        _vehicleRepo = vehicleRepo;
        _rateRepo = rateRepo;
        _deliveryRepo = deliveryRepo;
        _movementRepo = movementRepo;
    }

    public async Task<(byte[] PdfBytes, string FileName)> GenerateElrPdfAsync(
        Guid transportRequestId, CancellationToken ct = default)
    {
        // ── Load data ───────────────────────────────────────────
        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Transport job {transportRequestId} not found.");

        var vehicles = (await _vehicleRepo.FindAsync(v => v.TransportRequestId == transportRequestId)).ToList();
        var delivery = await _deliveryRepo.FirstOrDefaultAsync(d => d.TransportRequestId == transportRequestId);

        // Load rates for each active vehicle
        var vehicleRates = new Dictionary<Guid, VehicleRate?>();
        foreach (var v in vehicles.Where(v => v.IsActive))
        {
            var rate = await _rateRepo.FirstOrDefaultAsync(r => r.TransportVehicleId == v.Id);
            vehicleRates[v.Id] = rate;
        }

        // Recent movements (last 10)
        var movements = (await _movementRepo.FindAsync(m => m.TransportRequestId == transportRequestId))
            .OrderByDescending(m => m.Timestamp)
            .Take(10)
            .ToList();

        // ── Build PDF ───────────────────────────────────────────
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(header => ComposeHeader(header, job));
                page.Content().Element(content => ComposeContent(content, job, vehicles, vehicleRates, delivery, movements));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        var fileName = $"ELR-{job.RequestNumber}.pdf";

        return (pdfBytes, fileName);
    }

    // ════════════════════════════════════════════════════════════
    //  HEADER
    // ════════════════════════════════════════════════════════════

    private static void ComposeHeader(IContainer container, TransportRequest job)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("ELECTRONIC LORRY RECEIPT (ELR)")
                        .FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
                    left.Item().Text("Transport Management System")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(150).AlignRight().Column(right =>
                {
                    right.Item().Text($"ELR #{job.RequestNumber}").Bold().FontSize(10);
                    right.Item().Text($"Date: {job.RequestDate:dd-MMM-yyyy}").FontSize(8);
                    right.Item().Text($"Status: {job.Status}").FontSize(8);
                });
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
        });
    }

    // ════════════════════════════════════════════════════════════
    //  CONTENT
    // ════════════════════════════════════════════════════════════

    private static void ComposeContent(
        IContainer container,
        TransportRequest job,
        List<TransportVehicle> vehicles,
        Dictionary<Guid, VehicleRate?> vehicleRates,
        TransportDelivery? delivery,
        List<TransportMovement> movements)
    {
        container.PaddingVertical(5).Column(col =>
        {
            col.Spacing(8);

            // ── Section 1: Job Details ──────────────────────────
            col.Item().Element(c => ComposeJobDetails(c, job));

            // ── Section 2: Origin / Destination ─────────────────
            col.Item().Element(c => ComposeOriginDestination(c, job));

            // ── Section 3: Cargo ────────────────────────────────
            col.Item().Element(c => ComposeCargo(c, job));

            // ── Section 4: Vehicle & Transporter Details ────────
            foreach (var vehicle in vehicles.Where(v => v.IsActive))
            {
                vehicleRates.TryGetValue(vehicle.Id, out var rate);
                col.Item().Element(c => ComposeVehicle(c, vehicle, rate));
            }

            // ── Section 5: Delivery / POD ───────────────────────
            if (delivery != null)
                col.Item().Element(c => ComposeDelivery(c, delivery));

            // ── Section 6: Movement History ─────────────────────
            if (movements.Any())
                col.Item().Element(c => ComposeMovements(c, movements));

            // ── Signature block ─────────────────────────────────
            col.Item().PaddingTop(20).Element(ComposeSignatureBlock);
        });
    }

    // ── Job Details ─────────────────────────────────────────────

    private static void ComposeJobDetails(IContainer container, TransportRequest job)
    {
        container.Section("Job Details", section =>
        {
            section.Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().InfoRow("Request No.", job.RequestNumber);
                    left.Item().InfoRow("Customer", job.CustomerName);
                    left.Item().InfoRow("GST Number", job.GSTNumber ?? "—");
                    left.Item().InfoRow("Source", job.Source.ToString());
                });
                row.RelativeItem().Column(right =>
                {
                    right.Item().InfoRow("Request Date", job.RequestDate.ToString("dd-MMM-yyyy"));
                    right.Item().InfoRow("Priority", job.Priority.ToString());
                    right.Item().InfoRow("Branch", job.BranchName ?? "—");
                    right.Item().InfoRow("Country", job.CountryCode);
                });
            });
        });
    }

    // ── Origin / Destination ────────────────────────────────────

    private static void ComposeOriginDestination(IContainer container, TransportRequest job)
    {
        container.Section("Origin / Destination", section =>
        {
            section.Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("FROM").Bold().FontSize(8).FontColor(Colors.Green.Darken2);
                    left.Item().Text(job.OriginLocationName ?? "—").Bold();
                    left.Item().Text(job.PickupAddress);
                    left.Item().Text($"{job.PickupCity}, {job.PickupState} - {job.PickupPincode}");
                });
                row.ConstantItem(30).AlignCenter().PaddingTop(10).Text("→").FontSize(16).Bold();
                row.RelativeItem().Column(right =>
                {
                    right.Item().Text("TO").Bold().FontSize(8).FontColor(Colors.Red.Darken2);
                    right.Item().Text(job.DestinationLocationName ?? "—").Bold();
                    right.Item().Text(job.DropAddress);
                    right.Item().Text($"{job.DropCity}, {job.DropState} - {job.DropPincode}");
                });
            });
        });
    }

    // ── Cargo ───────────────────────────────────────────────────

    private static void ComposeCargo(IContainer container, TransportRequest job)
    {
        container.Section("Cargo Details", section =>
        {
            section.Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().InfoRow("Cargo Type", job.CargoType.ToString());
                    left.Item().InfoRow("Description", job.CargoDescription ?? "—");
                    left.Item().InfoRow("Gross Weight (Kg)", job.GrossWeightKg.ToString("N2"));
                });
                row.RelativeItem().Column(right =>
                {
                    right.Item().InfoRow("Packages", job.NumberOfPackages.ToString());
                    right.Item().InfoRow("20ft Containers", job.Container20Count.ToString());
                    right.Item().InfoRow("40ft Containers", job.Container40Count.ToString());
                });
            });
        });
    }

    // ── Vehicle & Transporter ───────────────────────────────────

    private static void ComposeVehicle(IContainer container, TransportVehicle vehicle, VehicleRate? rate)
    {
        container.Section($"Vehicle — {vehicle.VehicleNumber}", section =>
        {
            section.Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().InfoRow("Vehicle No.", vehicle.VehicleNumber);
                    left.Item().InfoRow("Vehicle Type", vehicle.VehicleType.ToString());
                    left.Item().InfoRow("Transporter", vehicle.TransporterName ?? "—");
                    left.Item().InfoRow("LR Number", vehicle.LRNumber ?? "—");
                    left.Item().InfoRow("LR Date", vehicle.LRDate?.ToString("dd-MMM-yyyy") ?? "—");
                });
                row.RelativeItem().Column(right =>
                {
                    right.Item().InfoRow("Driver", vehicle.DriverName ?? "—");
                    right.Item().InfoRow("Driver Phone", vehicle.DriverPhone ?? "—");

                    if (rate != null)
                    {
                        right.Item().PaddingTop(3).Text("Rate Breakdown").Bold().FontSize(8);
                        right.Item().InfoRow("Freight", $"{rate.CurrencyCode} {rate.FreightRate:N2}");
                        right.Item().InfoRow("Detention", $"{rate.CurrencyCode} {rate.DetentionCharges:N2}");
                        right.Item().InfoRow("Toll", $"{rate.CurrencyCode} {rate.TollCharges:N2}");
                        right.Item().InfoRow("Other", $"{rate.CurrencyCode} {rate.OtherCharges:N2}");
                        right.Item().InfoRow("Total Rate", $"{rate.CurrencyCode} {rate.TotalRate:N2}");
                    }
                });
            });
        });
    }

    // ── Delivery / POD ──────────────────────────────────────────

    private static void ComposeDelivery(IContainer container, TransportDelivery delivery)
    {
        container.Section("Delivery & Proof of Delivery", section =>
        {
            section.Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().InfoRow("Delivery Date", delivery.DeliveryDate.ToString("dd-MMM-yyyy HH:mm"));
                    left.Item().InfoRow("Received By", delivery.ReceivedBy);
                    left.Item().InfoRow("Delivery Status", delivery.DeliveryStatus.ToString());
                    left.Item().InfoRow("E-Way Bill No.", delivery.EWayBillNumber ?? "—");
                });
                row.RelativeItem().Column(right =>
                {
                    right.Item().InfoRow("POD Number", delivery.PODNumber ?? "—");
                    right.Item().InfoRow("Challan No.", delivery.ChallanNumber ?? "—");
                    if (!string.IsNullOrEmpty(delivery.DamageNotes))
                        right.Item().InfoRow("Damage Notes", delivery.DamageNotes);
                    if (!string.IsNullOrEmpty(delivery.ShortDeliveryNotes))
                        right.Item().InfoRow("Short Delivery", delivery.ShortDeliveryNotes);
                });
            });
        });
    }

    // ── Movement History ────────────────────────────────────────

    private static void ComposeMovements(IContainer container, List<TransportMovement> movements)
    {
        container.Column(col =>
        {
            col.Item().SectionHeader("Movement History");

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(90); // Date
                    columns.RelativeColumn(2);  // Location
                    columns.RelativeColumn(1);  // Status
                    columns.RelativeColumn(2);  // Remarks
                });

                table.Header(header =>
                {
                    header.Cell().TableHeaderCell("Date/Time");
                    header.Cell().TableHeaderCell("Location");
                    header.Cell().TableHeaderCell("Status");
                    header.Cell().TableHeaderCell("Remarks");
                });

                foreach (var m in movements)
                {
                    table.Cell().TableCell(m.Timestamp.ToString("dd-MMM HH:mm"));
                    table.Cell().TableCell(m.LocationName ?? "—");
                    table.Cell().TableCell(m.Milestone.ToString());
                    table.Cell().TableCell(m.Remarks ?? "—");
                }
            });
        });
    }

    // ── Signature Block ─────────────────────────────────────────

    private static void ComposeSignatureBlock(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().PaddingTop(40).LineHorizontal(0.5f);
                col.Item().Text("Consignor Signature & Stamp").FontSize(7).AlignCenter();
            });

            row.ConstantItem(40); // spacer

            row.RelativeItem().Column(col =>
            {
                col.Item().PaddingTop(40).LineHorizontal(0.5f);
                col.Item().Text("Transporter Signature & Stamp").FontSize(7).AlignCenter();
            });

            row.ConstantItem(40); // spacer

            row.RelativeItem().Column(col =>
            {
                col.Item().PaddingTop(40).LineHorizontal(0.5f);
                col.Item().Text("Consignee Signature & Stamp").FontSize(7).AlignCenter();
            });
        });
    }

    // ════════════════════════════════════════════════════════════
    //  FOOTER
    // ════════════════════════════════════════════════════════════

    private static void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span("Generated: ").FontSize(7);
                    t.Span(DateTime.UtcNow.ToString("dd-MMM-yyyy HH:mm UTC")).FontSize(7);
                });
                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Page ").FontSize(7);
                    t.CurrentPageNumber().FontSize(7);
                    t.Span(" of ").FontSize(7);
                    t.TotalPages().FontSize(7);
                });
            });
            col.Item().Text("This is a system-generated Electronic Lorry Receipt. No physical signature required for digital copies.")
                .FontSize(6).FontColor(Colors.Grey.Medium).AlignCenter();
        });
    }
}

// ════════════════════════════════════════════════════════════════
//  QuestPDF Extension helpers for consistent styling
// ════════════════════════════════════════════════════════════════

internal static class ElrPdfExtensions
{
    /// <summary>Renders a bordered section with a title header.</summary>
    public static void Section(this IContainer container, string title, Action<IContainer> content)
    {
        container.Column(col =>
        {
            col.Item().SectionHeader(title);
            col.Item().Padding(5).Element(content);
        });
    }

    public static void SectionHeader(this IContainer container, string title)
    {
        container
            .Background(Colors.Blue.Lighten5)
            .Padding(4)
            .Text(title)
            .Bold()
            .FontSize(10)
            .FontColor(Colors.Blue.Darken3);
    }

    /// <summary>Label: Value row.</summary>
    public static void InfoRow(this IContainer container, string label, string value)
    {
        container.Row(row =>
        {
            row.ConstantItem(110).Text(label + ":").FontSize(8).FontColor(Colors.Grey.Darken2);
            row.RelativeItem().Text(value).FontSize(8);
        });
    }

    public static void TableHeaderCell(this IContainer container, string text)
    {
        container
            .Background(Colors.Grey.Lighten3)
            .Padding(3)
            .Text(text)
            .Bold()
            .FontSize(7);
    }

    public static void TableCell(this IContainer container, string text)
    {
        container
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(3)
            .Text(text)
            .FontSize(7);
    }
}
