
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class UsuarioCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
