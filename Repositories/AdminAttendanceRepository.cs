using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class AdminAttendanceRepository : IAdminAttendanceRepository
    {
        private readonly SchoolERPDbContext _context;

        public AdminAttendanceRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<AdminAttendance?> GetAsync(string adminId, DateOnly date)
        {
            return await _context.AdminAttendances
                .FirstOrDefaultAsync(aa => aa.AdminId == adminId && aa.Date == date);
        }

        public async Task<List<AdminAttendance>> GetAbsentDatesAsync(string adminId)
        {
            return await _context.AdminAttendances
                .Where(aa => aa.AdminId == adminId && aa.Status == AttendanceStatus.Absent)
                .OrderByDescending(aa => aa.Date)
                .ToListAsync();
        }

        public async Task AddAsync(AdminAttendance attendance)
        {
            await _context.AdminAttendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }
    }
}