
using System;

namespace myapp.DTOs
{
    public class StatusUpdateDto
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool? TriggersEmail { get; set; }
    }
}
