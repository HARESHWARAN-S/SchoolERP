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
        private readonly IHomeworkRepository _homeworkRepo;

        public StudentService(
            IStudentRepository studentRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            IStudentClassRepository studentClassRepo,
            IHomeworkRepository homeworkRepo)
        {
            _studentRepo = studentRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _homeworkRepo = homeworkRepo;
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

        public async Task<List<StudentHomeworkResponseDto>> GetHomeworkAsync(string admnNo)
        {
            // Check student exists
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            // Check student is active
            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            // Get homework for student's class and sec
            var homeworks = await _homeworkRepo.GetByClassAsync(student.Class, student.Sec);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed homework");

            // Return only date, subject, description
            return homeworks.Select(h => new StudentHomeworkResponseDto
            {
                Date = h.Date,
                Subject = h.Subject,
                Description = h.Description
            }).ToList();
        }

        public async Task<bool> AssignRollNumbersAsync(string Class, string sec)
        {
            // Check class exists
            var studentClass = await _studentClassRepo.GetAsync(Class, sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(Class, sec);

            // Get all active students in this class (rollNo != -1)
            var students = await _studentRepo.GetByClassAsync(Class, sec);

            if (!students.Any())
            {
                throw new Exception($"No active students found in class '{Class}-{sec}'");
            }

            // Sort by name ascending and assign roll numbers from 1
            var sortedStudents = students
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int rollNo = 1;
            foreach (var student in sortedStudents)
            {
                student.RollNo = rollNo;
                rollNo++;
            }

            await _studentRepo.UpdateRangeAsync(sortedStudents);

            await _logRepo.AddAsync(
                $"Admin assigned roll numbers for class '{Class}-{sec}' to {sortedStudents.Count} students");

            return true;
        }
    }
}