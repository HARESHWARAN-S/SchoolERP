using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolERPDbContext _context;

        public StudentRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(string admnNo)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.AdmnNo == admnNo);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<string> GetNextStudentIdAsync()
        {
            int count = await _context.Students.CountAsync();
            return "S" + (count + 1).ToString();
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Student>> GetByClassAsync(string Class, string sec)
        {
            return await _context.Students
                .Where(s => s.Class == Class && s.Sec == sec && s.RollNo != -1)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(List<Student> students)
        {
            _context.Students.UpdateRange(students);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Student>> GetAllByClassAsync(string Class)
        {
            return await _context.Students
                .Where(s => s.Class == Class && s.RollNo != -1)
                .ToListAsync();
        }

        public async Task<List<Student>> GetByClassSecOrderedAsync(string Class, string sec)
        {
            return await _context.Students
                .Where(s => s.Class == Class && s.Sec == sec && s.RollNo != -1)
                .OrderBy(s => s.RollNo)
                .ToListAsync();
        }

        public async Task<StudentClass?> GetByClassTeacherIdAsync(string teacherId)
        {
            return await _context.StudentClasses
                .FirstOrDefaultAsync(sc => sc.ClassTeacherId == teacherId);
        }
    }
}