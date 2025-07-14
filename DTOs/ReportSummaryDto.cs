
using System.Collections.Generic;

namespace myapp.DTOs
{
    public class ReportSummaryDto
    {
        public int TotalCreated { get; set; }
        public int TotalFinalized { get; set; }
        public List<StatusCountDto> OsByStatus { get; set; }
        public double AverageResolutionTimeInDays { get; set; }
    }

    public class StatusCountDto
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
    }
}
