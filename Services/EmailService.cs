
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using myapp.Models;

namespace myapp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailSettingsService _settingsService;

        public EmailService(IEmailSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var settings = await _settingsService.GetEmailSettingsAsync();

            if (string.IsNullOrEmpty(settings.SmtpServer) || string.IsNullOrEmpty(settings.SmtpUser))
            {
                // Fallback to console if settings are not configured
                Console.WriteLine("--- EMAIL SERVICE (NOT CONFIGURED) ---");
                Console.WriteLine($"To: {to}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Body: {body}");
                Console.WriteLine("--------------------------------------");
                return;
            }

            using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
            {
                client.EnableSsl = true; // This is a common requirement
                client.Credentials = new NetworkCredential(settings.SmtpUser, settings.SmtpPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings.SmtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true, // Set to true if you send HTML emails
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
