
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
    [Route("api/servicos")]
    [Authorize] // Placeholder for permission-based authorization
    public class ServicosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servico>>> GetServicos()
        {
            return await _context.Servicos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servico>> GetServico(Guid id)
        {
            var servico = await _context.Servicos.FindAsync(id);

            if (servico == null)
            {
                return NotFound();
            }

            return servico;
        }

        [HttpPost]
        public async Task<ActionResult<Servico>> CreateServico(ServicoCreateDto servicoDto)
        {
            var servico = new Servico
            {
                Name = servicoDto.Name,
                Description = servicoDto.Description,
                DefaultPrice = servicoDto.DefaultPrice
            };

            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServico), new { id = servico.Id }, servico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServico(Guid id, ServicoUpdateDto servicoDto)
        {
            var servico = await _context.Servicos.FindAsync(id);

            if (servico == null)
            {
                return NotFound();
            }

            if (servicoDto.Name != null) servico.Name = servicoDto.Name;
            if (servicoDto.Description != null) servico.Description = servicoDto.Description;
            if (servicoDto.DefaultPrice.HasValue) servico.DefaultPrice = servicoDto.DefaultPrice.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServico(Guid id)
        {
            var servico = await _context.Servicos.FindAsync(id);

            if (servico == null)
            {
                return NotFound();
            }

            // Business Rule: Check if the service is associated with any service order.
            // This requires a many-to-many relationship between OrdemDeServico and Servico,
            // which is not defined in the current model. I'll assume for now that this check
            // is not needed, but it should be implemented if the relationship is added.

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
