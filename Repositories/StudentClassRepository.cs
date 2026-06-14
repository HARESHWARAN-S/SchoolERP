using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly SchoolERPDbContext _context;

        public StudentClassRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<StudentClass?> GetAsync(string Class, string sec)
        {
            return await _context.StudentClasses
                .Include(sc => sc.ClassTeacher)
                .FirstOrDefaultAsync(sc => sc.Class == Class && sc.Sec == sec);
        }

        public async Task<StudentClass?> GetByTeacherIdAsync(string teacherId)
        {
            return await _context.StudentClasses
                .FirstOrDefaultAsync(sc => sc.ClassTeacherId == teacherId);
        }

        public async Task<List<StudentClass>> GetAllAsync()
        {
            return await _context.StudentClasses
                .Include(sc => sc.ClassTeacher)
                .ToListAsync();
        }

        public async Task AddAsync(StudentClass studentClass)
        {
            await _context.StudentClasses.AddAsync(studentClass);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StudentClass studentClass)
        {
            _context.StudentClasses.Remove(studentClass);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentClass?> GetByClassTeacherIdAsync(string teacherId)
        {
            return await _context.StudentClasses
                .Include(sc => sc.ClassTeacher)
                .FirstOrDefaultAsync(sc => sc.ClassTeacherId == teacherId);
        }
    }
}