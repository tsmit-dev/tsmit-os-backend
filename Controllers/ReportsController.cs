
using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using myapp.Services;

namespace myapp.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize] // Placeholder for permission-based authorization, e.g., [Authorize(Policy = "CanViewReports")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISystemSettingsService _systemSettingsService;

        public ReportsController(ApplicationDbContext context, ISystemSettingsService systemSettingsService)
        {
            _context = context;
            _systemSettingsService = systemSettingsService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ReportSummaryDto>> GetSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            // Default to the last 30 days if no dates are provided
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var dateFilteredQuery = _context.OrdensDeServico
                .Where(os => os.CreatedAt >= start && os.CreatedAt <= end);

            // 1. Total OS created in the period
            var totalCreated = await dateFilteredQuery.CountAsync();

            // 2. Total OS finalized in the period
            var finalizedStatusId = _systemSettingsService.GetReadyForDeliveryStatusId(); 
            // In a real scenario, you might have multiple "finalized" statuses
            var finalizedQuery = dateFilteredQuery.Where(os => os.StatusId == finalizedStatusId);
            var totalFinalized = await finalizedQuery.CountAsync();

            // 3. Current count of OS by status (not filtered by date, as it's a snapshot of the current state)
            var osByStatus = await _context.OrdensDeServico
                .Include(os => os.Status)
                .GroupBy(os => new { os.StatusId, os.Status.Name })
                .Select(g => new StatusCountDto { StatusName = g.Key.Name, Count = g.Count() })
                .ToListAsync();

            // 4. Average resolution time
            var finalizedOrdersInPeriod = await finalizedQuery
                .Select(os => new { os.CreatedAt, os.UpdatedAt })
                .ToListAsync();

            double averageResolutionTimeInDays = 0;
            if (finalizedOrdersInPeriod.Any())
            {
                averageResolutionTimeInDays = finalizedOrdersInPeriod
                    .Average(os => (os.UpdatedAt - os.CreatedAt).TotalDays);
            }

            var summary = new ReportSummaryDto
            {
                TotalCreated = totalCreated,
                TotalFinalized = totalFinalized,
                OsByStatus = osByStatus,
                AverageResolutionTimeInDays = Math.Round(averageResolutionTimeInDays, 2)
            };

            return Ok(summary);
        }
    }
}
