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
    }
}