using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(string admnNo);
        Task<List<Student>> GetAllAsync();
        Task<string> GetNextStudentIdAsync();
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task<List<Student>> GetByClassAsync(string Class, string sec);
        Task UpdateRangeAsync(List<Student> students);
        Task<List<Student>> GetAllByClassAsync(string Class);
    }
}