using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IStudentAttendanceRepository
    {
        Task<bool> ExistsAsync(int classId, DateOnly date);
        Task<List<StudentAttendance>> GetAbsentDatesAsync(string admnNo);
        Task AddRangeAsync(List<StudentAttendance> attendances);
    }
}