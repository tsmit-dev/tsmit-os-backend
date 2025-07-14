
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class TestEmailDto
    {
        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; }
    }
}
