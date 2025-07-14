
using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.DTOs;
using myapp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;

namespace myapp.Controllers
{
    [ApiController]
    [Route("api/status")]
    [Authorize] // Placeholder for permission-based authorization
    public class StatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatus()
        {
            return await _context.Status.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatus(Guid id)
        {
            var status = await _context.Status.FindAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            return status;
        }

        [HttpPost]
        public async Task<ActionResult<Status>> CreateStatus(StatusCreateDto statusDto)
        {
            var status = new Status
            {
                Name = statusDto.Name,
                Color = statusDto.Color,
                Icon = statusDto.Icon,
                TriggersEmail = statusDto.TriggersEmail
            };

            _context.Status.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, status);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, StatusUpdateDto statusDto)
        {
            var status = await _context.Status.FindAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            if(statusDto.Name != null) status.Name = statusDto.Name;
            if(statusDto.Color != null) status.Color = statusDto.Color;
            if(statusDto.Icon != null) status.Icon = statusDto.Icon;
            if(statusDto.TriggersEmail.HasValue) status.TriggersEmail = statusDto.TriggersEmail.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(Guid id)
        {
            var status = await _context.Status.FindAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            var isAssociatedWithOS = await _context.OrdensDeServico.AnyAsync(os => os.StatusId == id);
            if(isAssociatedWithOS)
            {
                return Conflict("Cannot delete status. It is associated with one or more service orders.");
            }


            _context.Status.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
