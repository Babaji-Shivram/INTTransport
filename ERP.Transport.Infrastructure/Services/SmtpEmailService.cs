using System.Net;
using System.Net.Mail;
using ERP.Transport.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Infrastructure.Services;

/// <summary>
/// SMTP-based email sender. Configuration via SmtpSettings in appsettings.json.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var smtpHost = _config["SmtpSettings:Host"] ?? "localhost";
        var smtpPort = int.Parse(_config["SmtpSettings:Port"] ?? "587");
        var smtpUser = _config["SmtpSettings:Username"] ?? "";
        var smtpPass = _config["SmtpSettings:Password"] ?? "";
        var fromEmail = _config["SmtpSettings:FromEmail"] ?? "noreply@transport.local";
        var fromName = _config["SmtpSettings:FromName"] ?? "Transport Management System";
        var enableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"] ?? "true");

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = enableSsl
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = message.Subject,
            Body = message.HtmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(message.To);

        if (!string.IsNullOrEmpty(message.Cc))
            mailMessage.CC.Add(message.Cc);

        if (message.Attachment != null)
        {
            var ms = new MemoryStream(message.Attachment.Content);
            var attachment = new Attachment(ms, message.Attachment.FileName, message.Attachment.ContentType);
            mailMessage.Attachments.Add(attachment);
        }

        try
        {
            await client.SendMailAsync(mailMessage, ct);
            _logger.LogInformation("Email sent to {To} — Subject: {Subject}", message.To, message.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} — Subject: {Subject}", message.To, message.Subject);
            throw;
        }
    }
}
