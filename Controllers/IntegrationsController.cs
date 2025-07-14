
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using myapp.DTOs;
using myapp.Models;
using myapp.Services;
using Microsoft.AspNetCore.Authorization;
using System;

namespace myapp.Controllers
{
    [ApiController]
    [Route("api/integrations")]
    [Authorize] // Placeholder for permission-based authorization, e.g., [Authorize(Policy = "ManageIntegrations")]
    public class IntegrationsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IEmailSettingsService _emailSettingsService;

        public IntegrationsController(IEmailService emailService, IEmailSettingsService emailSettingsService)
        {
            _emailService = emailService;
            _emailSettingsService = emailSettingsService;
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailDto testEmailDto)
        {
            try
            {
                var subject = "Teste de Configuração de E-mail";
                var body = "Se você recebeu este e-mail, a configuração do seu servidor SMTP está funcionando corretamente.";
                await _emailService.SendEmailAsync(testEmailDto.RecipientEmail, subject, body);
                return Ok(new { message = "Test email sent successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return StatusCode(500, new { message = "Failed to send test email.", error = ex.Message });
            }
        }

        [HttpGet("email-settings")]
        public async Task<ActionResult<EmailSettings>> GetEmailSettings()
        {
            var settings = await _emailSettingsService.GetEmailSettingsAsync();
            // Ensure the password is not returned, even if decrypted
            settings.SmtpPassword = null; 
            return Ok(settings);
        }

        [HttpPut("email-settings")]
        public async Task<IActionResult> UpdateEmailSettings([FromBody] EmailSettings settings)
        {
            if (settings == null)
            {
                return BadRequest();
            }

            await _emailSettingsService.UpdateEmailSettingsAsync(settings);
            return NoContent();
        }
    }
}
