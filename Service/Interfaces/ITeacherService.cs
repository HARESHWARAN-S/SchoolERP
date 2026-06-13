using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherResponseDto> GetMyDetailsAsync(string teacherId);
    }
}