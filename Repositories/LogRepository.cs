using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly SchoolERPDbContext _context;

        public LogRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(string message)
        {
            var log = new Log
            {
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}