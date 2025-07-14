
using System.ComponentModel.DataAnnotations;

namespace myapp.DTOs
{
    public class ClienteUpdateDto
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Document { get; set; }
    }
}
