using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class StudentAttendanceRepository : IStudentAttendanceRepository
    {
        private readonly SchoolERPDbContext _context;

        public StudentAttendanceRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string Class, string sec, DateOnly date)
        {
            return await _context.StudentAttendances
                .AnyAsync(sa => sa.Class == Class && sa.Sec == sec && sa.Date == date);
        }

        public async Task<List<StudentAttendance>> GetAbsentDatesAsync(string admnNo)
        {
            return await _context.StudentAttendances
                .Where(sa => sa.AdmnNo == admnNo && sa.Status == AttendanceStatus.Absent)
                .OrderByDescending(sa => sa.Date)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<StudentAttendance> attendances)
        {
            await _context.StudentAttendances.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();
        }
    }
}