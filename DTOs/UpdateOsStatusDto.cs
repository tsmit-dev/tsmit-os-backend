
using System;
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class UpdateOsStatusDto
    {
        [Required]
        public Guid StatusId { get; set; }

        public string? Notes { get; set; }
    }
}
