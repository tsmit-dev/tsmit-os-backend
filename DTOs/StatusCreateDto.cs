
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class StatusCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool TriggersEmail { get; set; }
    }
}
