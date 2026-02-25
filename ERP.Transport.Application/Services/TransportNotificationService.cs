using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Orchestrates transport-specific email notifications:
/// daily status updates, delivery confirmations, ELR distribution, and assignment alerts.
/// </summary>
public class TransportNotificationService : ITransportNotificationService
{
    private readonly IEmailService _emailService;
    private readonly IElrService _elrService;
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IRepository<TransportDelivery> _deliveryRepo;
    private readonly IRepository<TransportMovement> _movementRepo;
    private readonly IRepository<Transporter> _transporterRepo;
    private readonly ILogger<TransportNotificationService> _logger;

    public TransportNotificationService(
        IEmailService emailService,
        IElrService elrService,
        IRepository<TransportRequest> jobRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IRepository<TransportDelivery> deliveryRepo,
        IRepository<TransportMovement> movementRepo,
        IRepository<Transporter> transporterRepo,
        ILogger<TransportNotificationService> logger)
    {
        _emailService = emailService;
        _elrService = elrService;
        _jobRepo = jobRepo;
        _vehicleRepo = vehicleRepo;
        _deliveryRepo = deliveryRepo;
        _movementRepo = movementRepo;
        _transporterRepo = transporterRepo;
        _logger = logger;
    }

    // ════════════════════════════════════════════════════════════
    //  DAILY STATUS EMAIL
    // ════════════════════════════════════════════════════════════

    public async Task SendDailyStatusEmailAsync(Guid transportRequestId, CancellationToken ct = default)
    {
        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Job {transportRequestId} not found.");

        var vehicles = (await _vehicleRepo.FindAsync(v => v.TransportRequestId == transportRequestId))
            .Where(v => v.IsActive).ToList();

        var latestMovement = (await _movementRepo.FindAsync(m => m.TransportRequestId == transportRequestId))
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefault();

        var body = $@"
<html>
<body style='font-family: Arial, sans-serif; font-size: 14px;'>
    <h2 style='color: #1a56db;'>Daily Transport Status Update</h2>
    <table style='border-collapse: collapse; width: 100%; max-width: 600px;'>
        <tr><td style='padding: 6px; font-weight: bold; width: 180px;'>Job Number:</td><td style='padding: 6px;'>{job.RequestNumber}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Status:</td><td style='padding: 6px;'>{job.Status}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Origin:</td><td style='padding: 6px;'>{job.OriginLocationName} — {job.PickupCity}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Destination:</td><td style='padding: 6px;'>{job.DestinationLocationName} — {job.DropCity}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Expected Delivery:</td><td style='padding: 6px;'>{job.RequiredDeliveryDate?.ToString("dd-MMM-yyyy") ?? "—"}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Vehicle(s):</td><td style='padding: 6px;'>{string.Join(", ", vehicles.Select(v => $"{v.VehicleNumber} ({v.TransporterName})"))}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Last Location:</td><td style='padding: 6px;'>{latestMovement?.LocationName ?? "Not yet updated"}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Last Update:</td><td style='padding: 6px;'>{latestMovement?.Timestamp.ToString("dd-MMM-yyyy HH:mm") ?? "—"}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Milestone:</td><td style='padding: 6px;'>{latestMovement?.Milestone.ToString() ?? "—"}</td></tr>
    </table>
    <p style='color: #6b7280; font-size: 12px; margin-top: 16px;'>This is an automated notification from the Transport Management System.</p>
</body>
</html>";

        // NOTE: In production, the customer email would come from CRM integration or the TransportRequest.
        // For now, we log the email content. The caller should provide the recipient.
        await _emailService.SendAsync(new EmailMessage
        {
            To = job.CustomerName, // Placeholder — should be customer email from CRM
            Subject = $"Transport Status Update — {job.RequestNumber} ({job.Status})",
            HtmlBody = body
        }, ct);

        _logger.LogInformation("Daily status email sent for job {RequestNumber}", job.RequestNumber);
    }

    // ════════════════════════════════════════════════════════════
    //  DELIVERY NOTIFICATION
    // ════════════════════════════════════════════════════════════

    public async Task SendDeliveryNotificationAsync(Guid transportRequestId, CancellationToken ct = default)
    {
        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Job {transportRequestId} not found.");

        var delivery = await _deliveryRepo.FirstOrDefaultAsync(d => d.TransportRequestId == transportRequestId);
        if (delivery == null)
            throw new InvalidOperationException("No delivery record found for this job.");

        var body = $@"
<html>
<body style='font-family: Arial, sans-serif; font-size: 14px;'>
    <h2 style='color: #059669;'>Delivery Confirmation</h2>
    <p>Your shipment has been delivered. Details below:</p>
    <table style='border-collapse: collapse; width: 100%; max-width: 600px;'>
        <tr><td style='padding: 6px; font-weight: bold; width: 180px;'>Job Number:</td><td style='padding: 6px;'>{job.RequestNumber}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Delivery Date:</td><td style='padding: 6px;'>{delivery.DeliveryDate:dd-MMM-yyyy HH:mm}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Received By:</td><td style='padding: 6px;'>{delivery.ReceivedBy}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>POD Number:</td><td style='padding: 6px;'>{delivery.PODNumber ?? "—"}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Delivery Status:</td><td style='padding: 6px;'>{delivery.DeliveryStatus}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>E-Way Bill:</td><td style='padding: 6px;'>{delivery.EWayBillNumber ?? "—"}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Challan No.:</td><td style='padding: 6px;'>{delivery.ChallanNumber ?? "—"}</td></tr>
    </table>
    {(delivery.DamageNotes != null ? $"<p style='color: #dc2626;'><strong>Damage Notes:</strong> {delivery.DamageNotes}</p>" : "")}
    {(delivery.ShortDeliveryNotes != null ? $"<p style='color: #d97706;'><strong>Short Delivery:</strong> {delivery.ShortDeliveryNotes}</p>" : "")}
    <p style='color: #6b7280; font-size: 12px; margin-top: 16px;'>This is an automated notification from the Transport Management System.</p>
</body>
</html>";

        await _emailService.SendAsync(new EmailMessage
        {
            To = job.CustomerName, // Placeholder — should be customer email
            Subject = $"Delivery Confirmation — {job.RequestNumber}",
            HtmlBody = body
        }, ct);

        _logger.LogInformation("Delivery notification sent for job {RequestNumber}", job.RequestNumber);
    }

    // ════════════════════════════════════════════════════════════
    //  ELR EMAIL (WITH PDF ATTACHMENT)
    // ════════════════════════════════════════════════════════════

    public async Task SendElrEmailAsync(Guid transportRequestId, CancellationToken ct = default)
    {
        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Job {transportRequestId} not found.");

        var (pdfBytes, fileName) = await _elrService.GenerateElrPdfAsync(transportRequestId, ct);

        // Send to customer
        await _emailService.SendAsync(new EmailMessage
        {
            To = job.CustomerName, // Placeholder — customer email
            Subject = $"Electronic Lorry Receipt — {job.RequestNumber}",
            HtmlBody = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #1a56db;'>Electronic Lorry Receipt</h2>
    <p>Please find attached the Electronic Lorry Receipt (ELR) for transport job <strong>{job.RequestNumber}</strong>.</p>
    <p>This serves as the official delivery confirmation document.</p>
    <p style='color: #6b7280; font-size: 12px;'>This is an automated email from the Transport Management System.</p>
</body>
</html>",
            Attachment = new EmailAttachment
            {
                FileName = fileName,
                Content = pdfBytes,
                ContentType = "application/pdf"
            }
        }, ct);

        // Also send to transporter(s)
        var vehicles = (await _vehicleRepo.FindAsync(v => v.TransportRequestId == transportRequestId))
            .Where(v => v.IsActive).ToList();

        foreach (var vehicle in vehicles)
        {
            var transporter = await _transporterRepo.GetByIdAsync(vehicle.TransporterId);
            if (transporter?.Email != null)
            {
                await _emailService.SendAsync(new EmailMessage
                {
                    To = transporter.Email,
                    Subject = $"Electronic Lorry Receipt — {job.RequestNumber}",
                    HtmlBody = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #1a56db;'>Electronic Lorry Receipt</h2>
    <p>Please find attached the ELR for transport job <strong>{job.RequestNumber}</strong> assigned to vehicle <strong>{vehicle.VehicleNumber}</strong>.</p>
    <p style='color: #6b7280; font-size: 12px;'>This is an automated email from the Transport Management System.</p>
</body>
</html>",
                    Attachment = new EmailAttachment
                    {
                        FileName = fileName,
                        Content = pdfBytes,
                        ContentType = "application/pdf"
                    }
                }, ct);
            }
        }

        _logger.LogInformation("ELR email sent for job {RequestNumber}", job.RequestNumber);
    }

    // ════════════════════════════════════════════════════════════
    //  TRANSPORTER ASSIGNMENT ALERT
    // ════════════════════════════════════════════════════════════

    public async Task SendTransporterAssignmentAsync(
        Guid transportRequestId, Guid transportVehicleId, CancellationToken ct = default)
    {
        var job = await _jobRepo.GetByIdAsync(transportRequestId)
            ?? throw new KeyNotFoundException($"Job {transportRequestId} not found.");

        var vehicle = await _vehicleRepo.GetByIdAsync(transportVehicleId)
            ?? throw new KeyNotFoundException($"Vehicle assignment {transportVehicleId} not found.");

        var transporter = await _transporterRepo.GetByIdAsync(vehicle.TransporterId);
        if (transporter?.Email == null)
        {
            _logger.LogWarning("No email for transporter {TransporterId}", vehicle.TransporterId);
            return;
        }

        var body = $@"
<html>
<body style='font-family: Arial, sans-serif; font-size: 14px;'>
    <h2 style='color: #1a56db;'>New Transport Assignment</h2>
    <p>You have been assigned a new transport job. Details:</p>
    <table style='border-collapse: collapse; width: 100%; max-width: 600px;'>
        <tr><td style='padding: 6px; font-weight: bold; width: 180px;'>Job Number:</td><td style='padding: 6px;'>{job.RequestNumber}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Vehicle:</td><td style='padding: 6px;'>{vehicle.VehicleNumber}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Origin:</td><td style='padding: 6px;'>{job.OriginLocationName} — {job.PickupCity}, {job.PickupState}</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Destination:</td><td style='padding: 6px;'>{job.DestinationLocationName} — {job.DropCity}, {job.DropState}</td></tr>
        <tr><td style='padding: 6px; font-weight: bold;'>Cargo:</td><td style='padding: 6px;'>{job.CargoDescription} ({job.GrossWeightKg:N0} Kg)</td></tr>
        <tr style='background: #f3f4f6;'><td style='padding: 6px; font-weight: bold;'>Expected Delivery:</td><td style='padding: 6px;'>{job.RequiredDeliveryDate?.ToString("dd-MMM-yyyy") ?? "TBD"}</td></tr>
    </table>
    <p style='color: #6b7280; font-size: 12px; margin-top: 16px;'>This is an automated notification from the Transport Management System.</p>
</body>
</html>";

        await _emailService.SendAsync(new EmailMessage
        {
            To = transporter.Email,
            Subject = $"New Job Assignment — {job.RequestNumber} — Vehicle {vehicle.VehicleNumber}",
            HtmlBody = body
        }, ct);

        _logger.LogInformation("Assignment email sent to transporter {TransporterName} for job {RequestNumber}",
            vehicle.TransporterName, job.RequestNumber);
    }
}
