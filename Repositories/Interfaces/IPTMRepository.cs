using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IPTMRepository
    {
        Task<PTM?> GetAsync(string admnNo, string teacherId, DateOnly date);
        Task<List<PTM>> GetByStudentAndClassIdAsync(string admnNo, int classId);
        Task AddAsync(PTM ptm);
    }
}