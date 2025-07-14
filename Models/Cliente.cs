
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
    public class Cliente
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Document { get; set; }
    }
}
