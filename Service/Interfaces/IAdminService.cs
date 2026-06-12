using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminResponseDto> SetupFirstAdminAsync(CreateFirstAdminDto dto);
        Task<AdminResponseDto> AddAdminAsync(CreateAdminDto dto);
        Task<TeacherResponseDto> AddTeacherAsync(CreateTeacherDto dto);
        Task<bool> RemoveTeacherAsync(string teacherId);
        Task<TeacherResponseDto> GetTeacherAsync(string teacherId);
        Task<List<TeacherListDto>> GetAllTeachersAsync();
        Task<StudentResponseDto> AddStudentAsync(CreateStudentDto dto);
        Task<bool> RemoveStudentAsync(string admnNo);
        Task<StudentResponseDto> GetStudentAsync(string admnNo);
        Task<List<StudentListDto>> GetAllStudentsAsync();
    }
}