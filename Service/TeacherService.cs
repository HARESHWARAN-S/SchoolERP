using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Entities;
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
        private readonly IHomeworkRepository _homeworkRepo;   
        private readonly ISubjectRepository _subjectRepo;     
        private readonly IStudentRepository _studentRepo; 
        public TeacherService(
            ITeacherRepository teacherRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo,
            IHomeworkRepository homeworkRepo,       
            ISubjectRepository subjectRepo,         
            IStudentRepository studentRepo) 
        {
            _teacherRepo = teacherRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _teacherAttendanceRepo = teacherAttendanceRepo;
            _homeworkRepo = homeworkRepo;          
            _subjectRepo = subjectRepo;            
            _studentRepo = studentRepo; 
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

        public async Task<HomeworkResponseDto> AddHomeworkAsync(string teacherId, CreateHomeworkDto dto)
        {
            // Check teacher exists
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Check teacher is active
            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            // Check teacher is assigned to this subject for this class
            var subject = await _subjectRepo.GetByClassSecSubjectTeacherAsync(
                dto.Class, dto.Sec, dto.Subject, teacherId);
            if (subject == null)
                throw new UnauthorizedSubjectAccessException(teacherId, dto.Subject, dto.Class, dto.Sec);

            // Check homework not already given today for same subject
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existing = await _homeworkRepo.GetAsync(dto.Class, dto.Sec, dto.Subject, today);
            if (existing != null)
                throw new HomeworkAlreadyExistsException(dto.Class, dto.Sec, dto.Subject, today);

            var homework = new Homework
            {
                Class = dto.Class,
                Sec = dto.Sec,
                Date = today,
                Subject = dto.Subject,
                Description = dto.Description
            };

            await _homeworkRepo.AddAsync(homework);
            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' added homework for subject '{dto.Subject}' in class '{dto.Class}-{dto.Sec}'");

            return new HomeworkResponseDto
            {
                HomeworkId = homework.HomeworkId,
                Class = homework.Class,
                Sec = homework.Sec,
                Date = homework.Date,
                Subject = homework.Subject,
                Description = homework.Description
            };
        }
    }
}