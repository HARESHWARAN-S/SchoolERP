using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Helpers;

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

        public async Task UpdateAsync(StudentClass studentClass)
        {
            _context.StudentClasses.Update(studentClass);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByTimetableAsync(string timetableUrl)
        {
            return await _context.StudentClasses
                .AnyAsync(sc => sc.ClassTimetable == timetableUrl);
        }

        public async Task<StudentClass?> GetCurrentAsync(string Class, string sec, string academicYear)
        {
            return await _context.StudentClasses
                .Include(sc => sc.ClassTeacher)
                .FirstOrDefaultAsync(sc =>
                    sc.Class == Class &&
                    sc.Sec == sec &&
                    sc.AcademicYear == academicYear);
        }

        public async Task<StudentClass?> GetCurrentByClassTeacherIdAsync(
            string teacherId, string academicYear)
        {
            return await _context.StudentClasses
                .Include(sc => sc.ClassTeacher)
                .FirstOrDefaultAsync(sc =>
                    sc.ClassTeacherId == teacherId &&
                    sc.AcademicYear == academicYear);
        }

        public async Task<string?> GetPreviousAcademicYearAsync()
        {
            string current = AcademicYearHelper.GetCurrentAcademicYear();
            // e.g. current = "2026-2027" → previous = "2025-2026"
            int currentStartYear = int.Parse(current.Split('-')[0]);
            string previous = $"{currentStartYear - 1}-{currentStartYear}";

            // Check if previous year actually exists in DB
            bool exists = await _context.StudentClasses
                .AnyAsync(sc => sc.AcademicYear == previous);
            return exists ? previous : null;
        }
    }
}