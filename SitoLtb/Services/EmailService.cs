using System.Net;
using System.Net.Mail;

namespace SitoLtb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendContactAsync(string fromName, string fromEmail, string message)
        {
            var contactTo = _config["Smtp:ContactTo"] ?? _config["Smtp:FromEmail"] ?? "";
            var subject = $"Messaggio dal sito — {fromName}";
            var html = $"""
                <p><strong>Da:</strong> {System.Net.WebUtility.HtmlEncode(fromName)} &lt;{System.Net.WebUtility.HtmlEncode(fromEmail)}&gt;</p>
                <hr/>
                <p>{System.Net.WebUtility.HtmlEncode(message).Replace("\n", "<br/>")}</p>
                """;
            await SendAsync(contactTo, subject, html);
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var client = BuildClient();
            using var msg = BuildMessage(toEmail, subject, htmlBody);
            try
            {
                await client.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore invio email a {To}", toEmail);
                throw;
            }
        }

        public async Task SendNewsletterAsync(IEnumerable<string> recipients, string subject, string htmlBody)
        {
            var client = BuildClient();
            foreach (var to in recipients)
            {
                try
                {
                    using var msg = BuildMessage(to, subject, htmlBody);
                    await client.SendMailAsync(msg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore invio newsletter a {To}", to);
                }
            }
        }

        private SmtpClient BuildClient()
        {
            var host = _config["Smtp:Host"] ?? "localhost";
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) && ssl;
            var user = _config["Smtp:Username"] ?? "";
            var pass = _config["Smtp:Password"] ?? "";

            return new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        public async Task SendWithIcsAsync(string toEmail, string subject, string htmlBody,
            string icsContent, string icsFileName)
        {
            var client = BuildClient();
            using var msg = BuildMessage(toEmail, subject, htmlBody);
            var icsBytes = System.Text.Encoding.UTF8.GetBytes(icsContent);
            var stream   = new MemoryStream(icsBytes);
            var att      = new Attachment(stream, icsFileName, "text/calendar");
            msg.Attachments.Add(att);
            try   { await client.SendMailAsync(msg); }
            catch (Exception ex) { _logger.LogError(ex, "Errore invio email+ics a {To}", toEmail); throw; }
        }

        private MailMessage BuildMessage(string to, string subject, string htmlBody)
        {
            var fromEmail = _config["Smtp:FromEmail"] ?? "noreply@example.com";
            var fromName  = _config["Smtp:FromName"]  ?? "LTB";
            return new MailMessage
            {
                From       = new MailAddress(fromEmail, fromName),
                Subject    = subject,
                Body       = htmlBody,
                IsBodyHtml = true,
                To         = { to }
            };
        }
    }
}
