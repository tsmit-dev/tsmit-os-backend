
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class UsuarioUpdateDto
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public Guid? RoleId { get; set; }

        public bool? IsActive { get; set; }
    }
}
