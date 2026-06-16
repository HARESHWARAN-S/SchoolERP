using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IAdminAttendanceRepository
    {
        Task<AdminAttendance?> GetAsync(string adminId, DateOnly date);
        Task<List<AdminAttendance>> GetAbsentDatesAsync(string adminId);
        Task AddAsync(AdminAttendance attendance);
    }
}