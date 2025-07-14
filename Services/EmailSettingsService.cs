
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using myapp.Models;
using Microsoft.Extensions.Hosting;

namespace myapp.Services
{
    public interface IEmailSettingsService
    {
        Task<EmailSettings> GetEmailSettingsAsync();
        Task UpdateEmailSettingsAsync(EmailSettings settings);
    }

    public class EmailSettingsService : IEmailSettingsService
    {
        private readonly string _filePath;
        private readonly IDataProtectionService _protectionService;

        public EmailSettingsService(IHostEnvironment env, IDataProtectionService protectionService)
        {
            _filePath = Path.Combine(env.ContentRootPath, "emailsettings.json");
            _protectionService = protectionService;
        }

        public async Task<EmailSettings> GetEmailSettingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new EmailSettings();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            var settings = JsonSerializer.Deserialize<EmailSettings>(json);

            if (!string.IsNullOrEmpty(settings.SmtpPassword))
            {
                settings.SmtpPassword = _protectionService.Decrypt(settings.SmtpPassword);
            }

            return settings;
        }

        public async Task UpdateEmailSettingsAsync(EmailSettings settings)
        {
            var settingsToStore = new EmailSettings
            {
                SmtpServer = settings.SmtpServer,
                SmtpPort = settings.SmtpPort,
                SmtpUser = settings.SmtpUser,
                SmtpPassword = !string.IsNullOrEmpty(settings.SmtpPassword)
                    ? _protectionService.Encrypt(settings.SmtpPassword)
                    : null
            };

            var json = JsonSerializer.Serialize(settingsToStore, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
