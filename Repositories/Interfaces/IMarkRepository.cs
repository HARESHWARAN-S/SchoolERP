using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IMarkRepository
    {
        Task<bool> ExistsAsync(string examName, string subject, string Class, string sec);
        Task<List<Mark>> GetByStudentAsync(string admnNo);
        Task AddRangeAsync(List<Mark> marks);
    }
}