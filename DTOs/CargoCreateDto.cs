
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class CargoCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string Permissions { get; set; }
    }
}
