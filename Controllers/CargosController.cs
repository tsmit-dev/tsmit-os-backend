
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
    [Route("api/cargos")]
    [Authorize] // Placeholder for permission-based authorization
    public class CargosController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cargo>>> GetCargos()
        {
            return await _context.Cargos.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cargo>> GetCargo(Guid id)
        {
            var cargo = await _context.Cargos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (cargo == null)
            {
                return NotFound();
            }

            return cargo;
        }

        [HttpPost]
        public async Task<ActionResult<Cargo>> CreateCargo(CargoCreateDto cargoDto)
        {
            var cargo = new Cargo
            {
                Name = cargoDto.Name,
                Permissions = cargoDto.Permissions
            };

            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCargo), new { id = cargo.Id }, cargo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCargo(Guid id, CargoUpdateDto cargoDto)
        {
            var cargo = await _context.Cargos.FindAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }

            if (cargoDto.Name != null) cargo.Name = cargoDto.Name;
            if (cargoDto.Permissions != null) cargo.Permissions = cargoDto.Permissions;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Renamed parameter 'id' to 'cargoId' to resolve ambiguity
        [HttpDelete("{cargoId}")]
        public async Task<IActionResult> DeleteCargo(Guid cargoId)
        {
            var cargoToDelete = await _context.Cargos.FindAsync(cargoId);

            if (cargoToDelete == null)
            {
                return NotFound();
            }
            
            var isAssociatedWithUser = await _context.Usuarios.AnyAsync(u => u.RoleId == cargoId);
            
            if(isAssociatedWithUser)
            {
                return Conflict("Cannot delete role. It is associated with one or more users.");
            }

            _context.Cargos.Remove(cargoToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
