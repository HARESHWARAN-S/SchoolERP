using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ITeacherAttendanceRepository
    {
        Task<TeacherAttendance?> GetAsync(string teacherId, DateOnly date);
        Task AddAsync(TeacherAttendance attendance);
        Task<List<DateOnly>> GetLeaveDatesAsync(string teacherId);
    }
}