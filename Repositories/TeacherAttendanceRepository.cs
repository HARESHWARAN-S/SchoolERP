using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Models.Enums;

namespace SchoolERP.Repositories
{
    public class TeacherAttendanceRepository : ITeacherAttendanceRepository
    {
        private readonly SchoolERPDbContext _context;

        public TeacherAttendanceRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherAttendance?> GetAsync(string teacherId, DateOnly date)
        {
            return await _context.TeacherAttendances
                .FirstOrDefaultAsync(ta => ta.TeacherId == teacherId && ta.Date == date);
        }

        public async Task<List<DateOnly>> GetLeaveDatesAsync(string teacherId)
        {
            return await _context.TeacherAttendances
                .Where(ta => ta.TeacherId == teacherId && ta.Status == AttendanceStatus.Absent)
                .Select(ta => ta.Date) 
                .ToListAsync(); 
        }

        public async Task AddAsync(TeacherAttendance attendance)
        {
            await _context.TeacherAttendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }
    }
}