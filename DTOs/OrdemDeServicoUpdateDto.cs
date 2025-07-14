
using System;

namespace myapp.DTOs
{
    public class OrdemDeServicoUpdateDto
    {
        public string Equipment { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string ProblemDescription { get; set; }
    }
}
