
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class OrdemDeServicoCreateDto
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string Equipment { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        [Required]
        public string ProblemDescription { get; set; }
    }
}
