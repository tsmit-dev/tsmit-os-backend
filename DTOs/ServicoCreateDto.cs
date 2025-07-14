
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class ServicoCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal DefaultPrice { get; set; }
    }
}
