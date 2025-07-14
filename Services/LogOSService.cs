
using System;
using System.Threading.Tasks;
using myapp.Data;
using myapp.Models;

namespace myapp.Services
{
    public interface ILogOSService
    {
        Task CreateLogAsync(int osId, Guid userId, string changeDescription, string details = null);
    }

    public class LogOSService : ILogOSService
    {
        private readonly ApplicationDbContext _context;

        public LogOSService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateLogAsync(int osId, Guid userId, string changeDescription, string details = null)
        {
            var log = new LogOS
            {
                OsId = osId,
                UserId = userId,
                ChangeTimestamp = DateTime.UtcNow,
                ChangeDescription = changeDescription,
                Details = details
            };

            _context.LogOS.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
