namespace SitoLtb.Services
{
    public interface IEmailService
    {
        Task SendContactAsync(string fromName, string fromEmail, string message);
        Task SendAsync(string toEmail, string subject, string htmlBody);
        Task SendNewsletterAsync(IEnumerable<string> recipients, string subject, string htmlBody);
        Task SendWithIcsAsync(string toEmail, string subject, string htmlBody, string icsContent, string icsFileName);
    }
}
