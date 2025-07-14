
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public Guid RoleId { get; set; }
        public virtual Cargo Role { get; set; }

        public bool IsActive { get; set; }
    }
}
