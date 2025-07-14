
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
    [Route("api/clientes")]
    [Authorize] // Placeholder for permission-based authorization
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CreateCliente(ClienteCreateDto clienteDto)
        {
            var cliente = new Cliente
            {
                Name = clienteDto.Name,
                Email = clienteDto.Email,
                Phone = clienteDto.Phone,
                Document = clienteDto.Document
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(Guid id, ClienteUpdateDto clienteDto)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            if(clienteDto.Name != null) cliente.Name = clienteDto.Name;
            if(clienteDto.Email != null) cliente.Email = clienteDto.Email;
            if(clienteDto.Phone != null) cliente.Phone = clienteDto.Phone;
            if(clienteDto.Document != null) cliente.Document = clienteDto.Document;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            var isAssociatedWithOS = await _context.OrdensDeServico.AnyAsync(os => os.ClientId == id);
            if (isAssociatedWithOS)
            {
                return Conflict("Cannot delete client. It is associated with one or more service orders.");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
