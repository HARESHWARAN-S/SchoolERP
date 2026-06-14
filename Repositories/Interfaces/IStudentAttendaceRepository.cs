using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IStudentAttendanceRepository
    {
        Task<bool> ExistsAsync(string Class, string sec, DateOnly date);
        Task<List<StudentAttendance>> GetAbsentDatesAsync(string admnNo);
        Task AddRangeAsync(List<StudentAttendance> attendances);
    }
}