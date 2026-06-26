using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IMarkRepository
    {
        Task<bool> ExistsAsync(string examName, string subject, int classId);
        Task<List<Mark>> GetByStudentAsync(string admnNo);
        Task AddRangeAsync(List<Mark> marks);
        Task UpdateAsync(Mark mark);
        Task<Mark?> GetByAdmnNoExamSubjectAsync(string admnNo, string examName, string subject);
    }
}