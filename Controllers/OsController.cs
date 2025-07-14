
using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.DTOs;
using myapp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using myapp.Services;

namespace myapp.Controllers
{
    [ApiController]
    [Route("api/os")]
    [Authorize]
    public class OsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogOSService _logOSService;
        private readonly IEmailService _emailService;
        private readonly ISystemSettingsService _systemSettingsService;

        public OsController(ApplicationDbContext context, ILogOSService logOSService, IEmailService emailService, ISystemSettingsService systemSettingsService)
        {
            _context = context;
            _logOSService = logOSService;
            _emailService = emailService;
            _systemSettingsService = systemSettingsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdemDeServico>>> GetOrdensDeServico(
            [FromQuery] Guid? statusId,
            [FromQuery] Guid? clientId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.OrdensDeServico.AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(os => os.StatusId == statusId.Value);
            }

            if (clientId.HasValue)
            {
                query = query.Where(os => os.ClientId == clientId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(os => os.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(os => os.CreatedAt <= endDate.Value);
            }

            return await query.Include(o => o.Client).Include(o => o.Status).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdemDeServico>> GetOrdemDeServico(int id)
        {
            var os = await _context.OrdensDeServico
                .Include(o => o.Client)
                .Include(o => o.Status)
                .Include(o => o.CreatedByUser)
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (os == null)
            {
                return NotFound();
            }

            return os;
        }

        [HttpPost]
        public async Task<ActionResult<OrdemDeServico>> CreateOrdemDeServico(OrdemDeServicoCreateDto osDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var os = new OrdemDeServico
            {
                ClientId = osDto.ClientId,
                Equipment = osDto.Equipment,
                Brand = osDto.Brand,
                Model = osDto.Model,
                SerialNumber = osDto.SerialNumber,
                ProblemDescription = osDto.ProblemDescription,
                StatusId = _systemSettingsService.GetDefaultStatusId(),
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OrdensDeServico.Add(os);
            await _context.SaveChangesAsync();

            await _logOSService.CreateLogAsync(os.Id, userId, "Ordem de Serviço criada.");

            return CreatedAtAction(nameof(GetOrdemDeServico), new { id = os.Id }, os);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrdemDeServico(int id, OrdemDeServicoUpdateDto osDto)
        {
            var os = await _context.OrdensDeServico.FindAsync(id);

            if (os == null)
            {
                return NotFound();
            }
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var changes = new List<string>();

            if (osDto.Equipment != null && os.Equipment != osDto.Equipment) { changes.Add("Equipamento alterado."); os.Equipment = osDto.Equipment; }
            if (osDto.Brand != null && os.Brand != osDto.Brand) { changes.Add("Marca alterada."); os.Brand = osDto.Brand; }
            if (osDto.Model != null && os.Model != osDto.Model) { changes.Add("Modelo alterado."); os.Model = osDto.Model; }
            if (osDto.SerialNumber != null && os.SerialNumber != osDto.SerialNumber) { changes.Add("Número de série alterado."); os.SerialNumber = osDto.SerialNumber; }
            if (osDto.ProblemDescription != null && os.ProblemDescription != osDto.ProblemDescription) { changes.Add("Descrição do problema alterada."); os.ProblemDescription = osDto.ProblemDescription; }

            if (changes.Count > 0)
            {
                os.UpdatedAt = DateTime.UtcNow;
                await _logOSService.CreateLogAsync(os.Id, userId, string.Join(" ", changes));
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOsStatusDto statusDto)
        {
            var os = await _context.OrdensDeServico.Include(o => o.Client).FirstOrDefaultAsync(o => o.Id == id);

            if (os == null)
            {
                return NotFound();
            }
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }
            
            var oldStatus = await _context.Status.FindAsync(os.StatusId);
            var newStatus = await _context.Status.FindAsync(statusDto.StatusId);
            if (newStatus == null) return BadRequest("Invalid StatusId.");

            os.StatusId = statusDto.StatusId;
            os.UpdatedAt = DateTime.UtcNow;

            var logDescription = $"Status alterado de '{oldStatus?.Name}' para '{newStatus.Name}'.";
            if (!string.IsNullOrEmpty(statusDto.Notes))
            {
                logDescription += $" Notas: {statusDto.Notes}";
            }
            
            await _logOSService.CreateLogAsync(os.Id, userId, logDescription);

            if (newStatus.TriggersEmail)
            {
                var subject = $"Atualização da sua Ordem de Serviço: {os.Id}";
                var body = $"Olá {os.Client.Name},<br/><br/>O status da sua ordem de serviço foi atualizado para: <strong>{newStatus.Name}</strong>.<br/><br/><em>{statusDto.Notes}</em>";
                await _emailService.SendEmailAsync(os.Client.Email, subject, body);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/solution")]
        public async Task<IActionResult> UpdateSolution(int id, SolutionUpdateDto solutionDto)
        {
            var os = await _context.OrdensDeServico.Include(o => o.Client).FirstOrDefaultAsync(o => o.Id == id);

            if (os == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var wasSolutionEmpty = string.IsNullOrEmpty(os.TechnicalSolution);
            os.TechnicalSolution = solutionDto.TechnicalSolution;
            os.UpdatedAt = DateTime.UtcNow;
            
            await _logOSService.CreateLogAsync(os.Id, userId, "Solução técnica preenchida/atualizada.");

            // Must save changes before calling another method that might also save
            await _context.SaveChangesAsync();

            if (wasSolutionEmpty && !string.IsNullOrEmpty(os.TechnicalSolution))
            {
                var readyForDeliveryStatusId = _systemSettingsService.GetReadyForDeliveryStatusId();
                if (os.StatusId != readyForDeliveryStatusId)
                {
                    await UpdateStatus(id, new UpdateOsStatusDto { StatusId = readyForDeliveryStatusId, Notes = "Solução técnica preenchida." });
                }
            }

            return NoContent();
        }
    }
}
