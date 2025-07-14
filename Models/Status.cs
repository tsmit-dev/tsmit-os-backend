
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
    public class Status
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Color { get; set; }

        public string Icon { get; set; }

        public bool TriggersEmail { get; set; }
    }
}
