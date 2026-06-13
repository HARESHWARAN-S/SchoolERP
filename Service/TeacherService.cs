using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using SchoolERP.Models.Enums;

namespace SchoolERP.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly ILoginRepository _loginRepo;
        private readonly ILogRepository _logRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly ITeacherAttendanceRepository _teacherAttendanceRepo;
        public TeacherService(
            ITeacherRepository teacherRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo)
        {
            _teacherRepo = teacherRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _teacherAttendanceRepo = teacherAttendanceRepo;
        }

        public async Task<TeacherResponseDto> GetMyDetailsAsync(string teacherId)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            await _logRepo.AddAsync($"Teacher '{teacherId}' viewed their profile");

            return new TeacherResponseDto
            {
                TeacherId = teacher.TeacherId,
                Name = teacher.Name,
                DOB = teacher.DOB,
                Gender = teacher.Gender,
                BloodGrp = teacher.BloodGrp,
                ContactNo = teacher.ContactNo,
                PhotoUrl = teacher.PhotoUrl,
                TimeTableUrl = teacher.TimeTableUrl,
                TotalDays = teacher.TotalDays,
                PresentDays = teacher.PresentDays,
                AttendancePercentage = teacher.AttendancePercentage
            };
        }

        public async Task<string> GetMyTimeTableAsync(string teacherId)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);
            var user = await _loginRepo.GetByUsernameAsync(teacherId);
            if (user.Status == UserStatus.Active)
                throw new UserInactiveException(teacherId);

            await _logRepo.AddAsync($"Teacher '{teacherId}' viewed their timetable");
            return teacher.TimeTableUrl;
        }

        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync()
        {
            var notifications = await _notificationRepo.GetByTargetAsync(NotificationTarget.Teacher);

            return notifications.Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationId,
                Target = n.Target,
                Title = n.Title,
                Message = n.Message,
                Timestamp = n.Timestamp
            }).ToList();
        }

        public async Task<List<TeacherLeaveDetailsDto>> GetMyLeaveDetailsAsync(string teacherId)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var user = await _loginRepo.GetByUsernameAsync(teacherId);
            if (user.Status == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            List<DateOnly> leaveDates = await _teacherAttendanceRepo.GetLeaveDatesAsync(teacherId);

            return leaveDates.Select(date => new TeacherLeaveDetailsDto
            {
                Date = date
            }).ToList();
            
        }
    }
}