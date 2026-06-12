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
    }
}