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

        public async Task<bool> ExistsAsync(int classId, DateOnly date)
        {
            return await _context.StudentAttendances
                .AnyAsync(sa => sa.ClassId == classId && sa.Date == date);
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