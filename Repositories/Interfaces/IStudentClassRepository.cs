using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IStudentClassRepository
    {
        Task<StudentClass?> GetAsync(string Class, string sec);
        Task<StudentClass?> GetByTeacherIdAsync(string teacherId);
        Task<List<StudentClass>> GetAllAsync();
        Task AddAsync(StudentClass studentClass);
        Task DeleteAsync(StudentClass studentClass);
        Task<StudentClass?> GetByClassTeacherIdAsync(string teacherId);
        Task<StudentClass?> GetCurrentByClassTeacherIdAsync(string teacherId, string academicYear);
        Task UpdateAsync(StudentClass studentClass);
        Task<bool> ExistsByTimetableAsync(string timetableUrl);
        Task<StudentClass?> GetCurrentAsync(string Class, string sec, string academicYear);
    }
}