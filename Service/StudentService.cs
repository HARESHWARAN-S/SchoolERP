using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using SchoolERP.Models.Enums;

namespace SchoolERP.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly ILoginRepository _loginRepo;
        private readonly ILogRepository _logRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IStudentClassRepository _studentClassRepo;

        public StudentService(
            IStudentRepository studentRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            IStudentClassRepository studentClassRepo)
        {
            _studentRepo = studentRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _studentClassRepo = studentClassRepo;
        }

        public async Task<StudentResponseDto> GetMyDetailsAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed their profile");

            return new StudentResponseDto
            {
                AdmnNo = student.AdmnNo,         
                RollNo = student.RollNo,         
                Name = student.Name,
                Class = student.Class,           
                Sec = student.Sec,               
                Gender = student.Gender,
                DOB = student.DOB,
                BloodGrp = student.BloodGrp,
                PhotoUrl = student.PhotoUrl,     
                FatherName = student.FatherName, 
                MotherName = student.MotherName, 
                Address = student.Address,       
                ContactNo = student.ContactNo,
                TotalDays = student.TotalDays,
                PresentDays = student.PresentDays,
                AttendancePercentage = student.AttendancePercentage
            };

        }

        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync()
        {
            var notifications = await _notificationRepo.GetByTargetAsync(NotificationTarget.Student);

            return notifications.Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationId,
                Target = n.Target,
                Title = n.Title,
                Message = n.Message,
                Timestamp = n.Timestamp
            }).ToList();
        }
        
        public async Task<string> GetMyTimeTableAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed their timetable");
            
            var classes = await _studentClassRepo.GetAsync(student.Class,student.Sec);
            if (classes == null)
                throw new StudentClassNotFoundException(student.Class,student.Sec);
            return classes.ClassTimetable;
        }
    }
}