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
        private readonly IStudentAttendanceRepository _studentAttendanceRepo; // ← ADD
        private readonly IStudentClassRepository _studentClassRepo;  
        private readonly IMarkRepository _markRepo;
        
        public TeacherService(
            ITeacherRepository teacherRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo,
            IHomeworkRepository homeworkRepo,       
            ISubjectRepository subjectRepo,         
            IStudentRepository studentRepo,
            IStudentAttendanceRepository studentAttendanceRepo, 
            IStudentClassRepository studentClassRepo,
            IMarkRepository markRepo) 
        {
            _teacherRepo = teacherRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _teacherAttendanceRepo = teacherAttendanceRepo;
            _homeworkRepo = homeworkRepo;          
            _subjectRepo = subjectRepo;            
            _studentRepo = studentRepo; 
            _studentAttendanceRepo = studentAttendanceRepo; 
            _studentClassRepo = studentClassRepo;   
            _markRepo = markRepo; 
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

        public async Task<List<StudentAttendanceResponseDto>> MarkStudentAttendanceAsync(
            string teacherId, MarkStudentAttendanceDto dto)
        {
            // Check teacher exists and is active
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            // Check teacher is a class teacher
            var studentClass = await _studentClassRepo.GetByClassTeacherIdAsync(teacherId);
            if (studentClass == null)
                throw new NotAClassTeacherException(teacherId);

            // Validate attendance list values — only 0 or 1 allowed
            if (dto.Attendance.Any(a => a != 0 && a != 1))
                throw new InvalidAttendanceValueException();

            // Validate list length matches class strength
            if (dto.Attendance.Count != studentClass.ClassStrength)
                throw new AttendanceStrengthMismatchException(studentClass.ClassStrength, dto.Attendance.Count);

            // Check attendance not already marked today
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool alreadyMarked = await _studentAttendanceRepo.ExistsAsync(
                studentClass.Class, studentClass.Sec, today);
            if (alreadyMarked)
                throw new AttendanceAlreadyMarkedException(studentClass.Class, studentClass.Sec, today);

            // Get students ordered by roll number
            var students = await _studentRepo.GetByClassSecOrderedAsync(
                studentClass.Class, studentClass.Sec);

            // Check roll numbers are assigned
            var unassigned = students.Where(s => s.RollNo.Equals(0)).Any();
            if (unassigned)
                throw new RollNumberNotAssignedException(studentClass.Class, studentClass.Sec);

            // Build attendance records
            var attendanceRecords = new List<StudentAttendance>();
            var result = new List<StudentAttendanceResponseDto>();

            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                AttendanceStatus status = dto.Attendance[i] == 1
                    ? AttendanceStatus.Present
                    : AttendanceStatus.Absent;

                attendanceRecords.Add(new StudentAttendance
                {
                    Class = studentClass.Class,
                    Sec = studentClass.Sec,
                    AdmnNo = student.AdmnNo,
                    Date = today,
                    Status = status
                });

                // Update student attendance stats
                student.TotalDays += 1;
                if (status == AttendanceStatus.Present)
                    student.PresentDays += 1;

                student.AttendancePercentage = ((decimal)student.PresentDays / student.TotalDays) * 100;

                result.Add(new StudentAttendanceResponseDto
                {
                    AdmnNo = student.AdmnNo,
                    Name = student.Name,
                    RollNo = student.RollNo,
                    Date = today,
                    Status = status.ToString()
                });
            }

            // Save attendance records
            await _studentAttendanceRepo.AddRangeAsync(attendanceRecords);

            // Update student stats in DB
            await _studentRepo.UpdateRangeAsync(students);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' marked attendance for class '{studentClass.Class}-{studentClass.Sec}' on '{today}'");

            return result;
        }

        public async Task<List<MarkEntryResponseDto>> AddMarksAsync(
            string teacherId, MarkEntryDto dto)
        {
            // Check teacher exists and is active
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            // Auto fetch subject from subject table using class+sec+teacherId
            var subjectRecord = await _subjectRepo.GetByTeacherAsync(dto.Class, dto.Sec, teacherId);
            if (subjectRecord == null)
                throw new UnauthorizedSubjectAccessException(teacherId, "", dto.Class, dto.Sec);

            string subjectName = subjectRecord.SubjectName; // ← fetched automatically

            // Get class to check strength
            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            // Validate marks list length matches class strength
            if (dto.Marks.Count != studentClass.ClassStrength)
                throw new MarkListStrengthMismatchException(studentClass.ClassStrength, dto.Marks.Count);

            // Validate each mark — must be -1 or between 0 and TotalMarks
            foreach (var mark in dto.Marks)
            {
                if (mark != -1 && (mark < 0 || mark > dto.TotalMarks))
                    throw new MarksOutOfRangeException(mark, dto.TotalMarks);
            }

            // Check marks not already entered for this exam+subject+class
            bool alreadyEntered = await _markRepo.ExistsAsync(
                dto.ExamName, subjectName, dto.Class, dto.Sec);
            if (alreadyEntered)
                throw new MarksAlreadyEnteredForExamException(
                    dto.ExamName, subjectName, dto.Class, dto.Sec);

            // Get students ordered by roll number
            var students = await _studentRepo.GetByClassSecOrderedAsync(dto.Class, dto.Sec);

            // Check roll numbers assigned
            if (students.Any(s => s.RollNo == 0))
                throw new RollNumberNotAssignedException(dto.Class, dto.Sec);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var markRecords = new List<Mark>();
            var result = new List<MarkEntryResponseDto>();

            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                decimal markValue = dto.Marks[i];

                markRecords.Add(new Mark
                {
                    AdmnNo = student.AdmnNo,
                    ExamName = dto.ExamName,
                    Subject = subjectName, // ← auto fetched
                    Date = today,
                    Class = dto.Class,
                    Sec = dto.Sec,
                    MarksObtained = markValue,
                    TotalMarks = dto.TotalMarks
                });

                result.Add(new MarkEntryResponseDto
                {
                    AdmnNo = student.AdmnNo,
                    Name = student.Name,
                    RollNo = student.RollNo,
                    ExamName = dto.ExamName,
                    Subject = subjectName, // ← auto fetched
                    Date = today,
                    MarksObtained = markValue == -1 ? "Absent" : markValue.ToString(),
                    TotalMarks = dto.TotalMarks
                });
            }

            await _markRepo.AddRangeAsync(markRecords);
            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' entered marks for exam '{dto.ExamName}' subject '{subjectName}' in class '{dto.Class}-{dto.Sec}'");

            return result;
        }
    }
}