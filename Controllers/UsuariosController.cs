
using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.DTOs;
using myapp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

namespace myapp.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize] // Placeholder for permission-based authorization
    public class UsuariosController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Include(u => u.Role)
                .AsNoTracking()
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUsuario(Guid id)
        {
            var userResponse = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync();

            if (userResponse == null)
            {
                return NotFound();
            }

            return userResponse;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUsuario(UsuarioCreateDto usuarioDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
            {
                return Conflict("A user with this email already exists.");
            }

            var usuario = new Usuario
            {
                Name = usuarioDto.Name,
                Email = usuarioDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password),
                RoleId = usuarioDto.RoleId,
                IsActive = usuarioDto.IsActive
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var role = await _context.Cargos.FindAsync(usuario.RoleId);

            var userResponse = new UserResponseDto
            {
                Id = usuario.Id,
                Name = usuario.Name,
                Email = usuario.Email,
                RoleId = usuario.RoleId,
                RoleName = role?.Name,
                IsActive = usuario.IsActive
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = userResponse.Id }, userResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(Guid id, UsuarioUpdateDto usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (usuarioDto.Name != null) usuario.Name = usuarioDto.Name;
            if (usuarioDto.Email != null) usuario.Email = usuarioDto.Email;
            if (!string.IsNullOrEmpty(usuarioDto.Password)) usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
            if (usuarioDto.RoleId.HasValue) usuario.RoleId = usuarioDto.RoleId.Value;
            if (usuarioDto.IsActive.HasValue) usuario.IsActive = usuarioDto.IsActive.Value;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var isAssociatedWithOS = await _context.OrdensDeServico.AnyAsync(os => os.CreatedByUserId == id);
            if (isAssociatedWithOS)
            {
                return Conflict("Cannot delete user. It is associated with one or more service orders.");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
