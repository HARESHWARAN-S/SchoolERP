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
        Task<NotificationResponseDto> AddNotificationAsync(CreateNotificationDto dto);
        Task<List<NotificationResponseDto>> GetAllNotificationsAsync();
        Task<StudentClassResponseDto> AddStudentClassAsync(CreateStudentClassDto dto);
        Task<List<StudentClassResponseDto>> GetAllStudentClassesAsync();
        Task<TeacherAttendanceResponseDto> MarkTeacherAttendanceAsync(MarkTeacherAttendanceDto dto);
        Task<SubjectResponseDto> AddSubjectAsync(CreateSubjectDto dto);
        Task<List<SubjectResponseDto>> GetAllSubjectsAsync();
        Task<List<SubjectResponseDto>> GetSubjectsByClassAsync(string Class, string sec);
        Task<bool> AssignRollNumbersAsync(string Class, string sec);
        Task<List<FeeResponseDto>> AddFeeAsync(CreateFeeDto dto);
        Task<AdminProfileResponseDto> GetMyProfileAsync(string adminId);
        Task<AdminAttendanceResponseDto> MarkMyAttendanceAsync(string adminId, MarkAdminAttendanceDto dto);
        Task<List<AdminLeaveDetailsResponseDto>> GetMyLeaveDetailsAsync(string adminId);
        Task<StudentClassResponseDto> UpdateClassTimetableAsync(UpdateClassTimetableDto dto);
        Task<TeacherResponseDto> UpdateTeacherTimetableAsync(UpdateTeacherTimetableDto dto);
    }
}