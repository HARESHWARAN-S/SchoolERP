using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IFeeRepository
    {
        Task<Fee?> GetByIdAsync(int feeId);
        Task<List<Fee>> GetDueByStudentAsync(string admnNo);
        Task AddRangeAsync(List<Fee> fees);
        Task UpdateAsync(Fee fee);
    }
}