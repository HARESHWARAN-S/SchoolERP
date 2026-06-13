using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;

namespace SchoolERP.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;
        private readonly ITeacherRepository _teacherRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ILoginRepository _loginRepo;
        private readonly ILogRepository _logRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IStudentClassRepository _studentClassRepo;
        private readonly ITeacherAttendanceRepository _teacherAttendanceRepo;

        public AdminService(
            IAdminRepository adminRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            IStudentClassRepository studentClassRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo)
        {
            _adminRepo = adminRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _studentClassRepo = studentClassRepo;
            _teacherAttendanceRepo = teacherAttendanceRepo;
        }

        // ── Admin Setup ───────────────────────────────────────────────────

        public async Task<AdminResponseDto> SetupFirstAdminAsync(CreateFirstAdminDto dto)
        {
            int count = await _adminRepo.GetAdminCountAsync();
            if (count > 0)
                throw new AdminAlreadyExistsException();

            string adminId = "A1";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var login = new Login
            {
                Username = adminId,
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };
            await _loginRepo.AddAsync(login);

            var admin = new Admin
            {
                AdminId = adminId,
                AdminName = dto.AdminName,
                DOB = dto.DOB,
                Gender = dto.Gender,
                BloodGrp = dto.BloodGrp,
                ContactNo = dto.ContactNo,
                PhotoUrl = dto.PhotoUrl
            };
            await _adminRepo.AddAsync(admin);

            await _logRepo.AddAsync($"First admin '{adminId}' created via setup");

            return MapToAdminResponse(admin);
        }

        public async Task<AdminResponseDto> AddAdminAsync(CreateAdminDto dto)
        {
            // get current active admin and inactivate them
            var currentAdmin = await _adminRepo.GetActiveAdminAsync();
            if (currentAdmin == null)
                throw new AdminNotFoundException("active admin");

            string newAdminId = await _adminRepo.GetNextAdminIdAsync();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newLogin = new Login
            {
                Username = newAdminId,
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };
            await _loginRepo.AddAsync(newLogin);

            var newAdmin = new Admin
            {
                AdminId = newAdminId,
                AdminName = dto.AdminName,
                DOB = dto.DOB,
                Gender = dto.Gender,
                BloodGrp = dto.BloodGrp,
                ContactNo = dto.ContactNo,
                PhotoUrl = dto.PhotoUrl
            };
            await _adminRepo.AddAsync(newAdmin);

            // inactivate current admin
            var currentLogin = await _loginRepo.GetByUsernameAsync(currentAdmin.AdminId);
            if (currentLogin != null)
            {
                currentLogin.Status = UserStatus.Inactive;
                await _loginRepo.UpdateAsync(currentLogin);
            }

            await _logRepo.AddAsync(
                $"Admin '{currentAdmin.AdminId}' added new admin '{newAdminId}' and was deactivated");

            return MapToAdminResponse(newAdmin);
        }

        // ── Teacher ───────────────────────────────────────────────────────

        public async Task<TeacherResponseDto> AddTeacherAsync(CreateTeacherDto dto)
        {
            string teacherId = await _teacherRepo.GetNextTeacherIdAsync();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var login = new Login
            {
                Username = teacherId,
                PasswordHash = passwordHash,
                Role = UserRole.Teacher,
                Status = UserStatus.Active
            };
            await _loginRepo.AddAsync(login);

            var teacher = new Teacher
            {
                TeacherId = teacherId,
                Name = dto.Name,
                DOB = dto.DOB,
                Gender = dto.Gender,
                BloodGrp = dto.BloodGrp,
                ContactNo = dto.ContactNo,
                PhotoUrl = dto.PhotoUrl,
                TimeTableUrl = dto.TimeTableUrl
            };
            await _teacherRepo.AddAsync(teacher);

            await _logRepo.AddAsync($"Admin added new teacher '{teacherId}'");

            return MapToTeacherResponse(teacher);
        }

        public async Task<bool> RemoveTeacherAsync(string teacherId)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var login = await _loginRepo.GetByUsernameAsync(teacherId);
            if (login == null)
                throw new UserNotFoundException(teacherId);

            login.Status = UserStatus.Inactive;
            await _loginRepo.UpdateAsync(login);

            await _logRepo.AddAsync($"Admin removed teacher '{teacherId}' (soft delete)");

            return true;
        }

        public async Task<TeacherResponseDto> GetTeacherAsync(string teacherId)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            return MapToTeacherResponse(teacher);
        }

        public async Task<List<TeacherListDto>> GetAllTeachersAsync()
        {
            var teachers = await _teacherRepo.GetAllAsync();
            return teachers.Select(t => new TeacherListDto
            {
                TeacherId = t.TeacherId,
                Name = t.Name,
                ContactNo = t.ContactNo
            }).ToList();
        }

        // ── Student ───────────────────────────────────────────────────────

        public async Task<StudentResponseDto> AddStudentAsync(CreateStudentDto dto)
        {
            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            string admnNo = await _studentRepo.GetNextStudentIdAsync();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var login = new Login
            {
                Username = admnNo,
                PasswordHash = passwordHash,
                Role = UserRole.Student,
                Status = UserStatus.Active
            };
            await _loginRepo.AddAsync(login);

            var student = new Student
            {
                AdmnNo = admnNo,
                RollNo = dto.RollNo,
                Name = dto.Name,
                Class = dto.Class,
                Sec = dto.Sec,
                Gender = dto.Gender,
                DOB = dto.DOB,
                BloodGrp = dto.BloodGrp,
                PhotoUrl = dto.PhotoUrl,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                Address = dto.Address,
                ContactNo = dto.ContactNo
            };
            await _studentRepo.AddAsync(student);

            await _logRepo.AddAsync($"Admin added new student '{admnNo}'");

            return MapToStudentResponse(student);
        }

        public async Task<bool> RemoveStudentAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var login = await _loginRepo.GetByUsernameAsync(admnNo);
            if (login == null)
                throw new UserNotFoundException(admnNo);

            login.Status = UserStatus.Inactive;
            await _loginRepo.UpdateAsync(login);

            await _logRepo.AddAsync($"Admin removed student '{admnNo}' (soft delete)");

            return true;
        }

        public async Task<StudentResponseDto> GetStudentAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            return MapToStudentResponse(student);
        }

        public async Task<List<StudentListDto>> GetAllStudentsAsync()
        {
            var students = await _studentRepo.GetAllAsync();
            return students.Select(s => new StudentListDto
            {
                AdmnNo = s.AdmnNo,
                RollNo = s.RollNo,
                Name = s.Name,
                Class = s.Class,
                Sec = s.Sec
            }).ToList();
        }

        public async Task<NotificationResponseDto> AddNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Target = dto.Target,
                Title = dto.Title,
                Message = dto.Message,
                Timestamp = DateTime.UtcNow
            };

            await _notificationRepo.AddAsync(notification);
            await _logRepo.AddAsync($"Admin added notification '{dto.Title}' for target '{dto.Target}'");

            return new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                Target = notification.Target,
                Title = notification.Title,
                Message = notification.Message,
                Timestamp = notification.Timestamp
            };
        }

        public async Task<List<NotificationResponseDto>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationRepo.GetAllAsync();

            return notifications.Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationId,
                Target = n.Target,
                Title = n.Title,
                Message = n.Message,
                Timestamp = n.Timestamp
            }).ToList();
        }

        // ── Mappers ───────────────────────────────────────────────────────

        private AdminResponseDto MapToAdminResponse(Admin a) => new AdminResponseDto
        {
            AdminId = a.AdminId,
            AdminName = a.AdminName,
            DOB = a.DOB,
            Gender = a.Gender,
            BloodGrp = a.BloodGrp,
            ContactNo = a.ContactNo,
            PhotoUrl = a.PhotoUrl,
            TotalDays = a.TotalDays,
            PresentDays = a.PresentDays,
            AttendancePercentage = a.AttendancePercentage
        };

        private TeacherResponseDto MapToTeacherResponse(Teacher t) => new TeacherResponseDto
        {
            TeacherId = t.TeacherId,
            Name = t.Name,
            DOB = t.DOB,
            Gender = t.Gender,
            BloodGrp = t.BloodGrp,
            ContactNo = t.ContactNo,
            PhotoUrl = t.PhotoUrl,
            TimeTableUrl = t.TimeTableUrl,
            TotalDays = t.TotalDays,
            PresentDays = t.PresentDays,
            AttendancePercentage = t.AttendancePercentage
        };

        private StudentResponseDto MapToStudentResponse(Student s) => new StudentResponseDto
        {
            AdmnNo = s.AdmnNo,
            RollNo = s.RollNo,
            Name = s.Name,
            Class = s.Class,
            Sec = s.Sec,
            Gender = s.Gender,
            DOB = s.DOB,
            BloodGrp = s.BloodGrp,
            PhotoUrl = s.PhotoUrl,
            FatherName = s.FatherName,
            MotherName = s.MotherName,
            Address = s.Address,
            ContactNo = s.ContactNo,
            TotalDays = s.TotalDays,
            PresentDays = s.PresentDays,
            AttendancePercentage = s.AttendancePercentage
        };

        // ── StudentClass ──────────────────────────────────────────────────

        public async Task<StudentClassResponseDto> AddStudentClassAsync(CreateStudentClassDto dto)
        {
            // Check class already exists
            var existing = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (existing != null)
                throw new StudentClassAlreadyExistsException(dto.Class, dto.Sec);

            // Check teacher exists and is active
            var teacher = await _teacherRepo.GetByIdAsync(dto.ClassTeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.ClassTeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.ClassTeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.ClassTeacherId);

            // Check teacher not already assigned as class teacher
            var alreadyAssigned = await _studentClassRepo.GetByTeacherIdAsync(dto.ClassTeacherId);
            if (alreadyAssigned != null)
                throw new TeacherAlreadyAssignedAsClassTeacherException(dto.ClassTeacherId);

            var studentClass = new StudentClass
            {
                Class = dto.Class,
                Sec = dto.Sec,
                ClassTimetable = dto.ClassTimetable,
                ClassTeacherId = dto.ClassTeacherId
            };

            await _studentClassRepo.AddAsync(studentClass);
            await _logRepo.AddAsync($"Admin added class '{dto.Class}-{dto.Sec}' with class teacher '{dto.ClassTeacherId}'");

            return new StudentClassResponseDto
            {
                Class = studentClass.Class,
                Sec = studentClass.Sec,
                ClassTimetable = studentClass.ClassTimetable,
                ClassTeacherId = studentClass.ClassTeacherId,
                ClassTeacherName = teacher.Name
            };
        }
        /*
        public async Task<bool> RemoveStudentClassAsync(string Class, string sec)
        {
            var studentClass = await _studentClassRepo.GetAsync(Class, sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(Class, sec);

            await _studentClassRepo.DeleteAsync(studentClass);
            await _logRepo.AddAsync($"Admin removed class '{Class}-{sec}'");

            return true;
        }*/

        public async Task<List<StudentClassResponseDto>> GetAllStudentClassesAsync()
        {
            var classes = await _studentClassRepo.GetAllAsync();

            return classes.Select(sc => new StudentClassResponseDto
            {
                Class = sc.Class,
                Sec = sc.Sec,
                ClassTimetable = sc.ClassTimetable,
                ClassTeacherId = sc.ClassTeacherId,
                ClassTeacherName = sc.ClassTeacher?.Name ?? "Unknown"
            }).ToList();
        }

        // ── Teacher Attendance ────────────────────────────────────────────

        public async Task<TeacherAttendanceResponseDto> MarkTeacherAttendanceAsync(MarkTeacherAttendanceDto dto)
        {
            // Validate status input
            if (dto.Status != 0 && dto.Status != 1)
                throw new InvalidAttendanceStatusException();

            // Check teacher exists
            var teacher = await _teacherRepo.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.TeacherId);

            // Check teacher is active
            var teacherStatus = await _loginRepo.GetStatusAsync(dto.TeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.TeacherId);

            // Check attendance not already marked today
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existing = await _teacherAttendanceRepo.GetAsync(dto.TeacherId, today);
            if (existing != null)
                throw new AttendanceAlreadyMarkedException(dto.TeacherId, today);

            // Map 0/1 to enum
            AttendanceStatus status = dto.Status == 1
                ? AttendanceStatus.Present
                : AttendanceStatus.Absent;

            // Save attendance record
            var attendance = new TeacherAttendance
            {
                TeacherId = dto.TeacherId,
                Date = today,
                Status = status
            };
            await _teacherAttendanceRepo.AddAsync(attendance);

            // Update teacher stats
            teacher.TotalDays += 1;
            if (status == AttendanceStatus.Present)
            {
                teacher.PresentDays += 1;
            }
            teacher.AttendancePercentage = ((decimal)teacher.PresentDays / teacher.TotalDays) * 100;
            await _teacherRepo.UpdateAsync(teacher);

            await _logRepo.AddAsync(
                $"Admin marked attendance for teacher '{dto.TeacherId}' as '{status}' on '{today}'");

            return new TeacherAttendanceResponseDto
            {
                TeacherId = teacher.TeacherId,
                TeacherName = teacher.Name,
                Date = today,
                Status = status.ToString(),
                TotalDays = teacher.TotalDays,
                PresentDays = teacher.PresentDays,
                AttendancePercentage = teacher.AttendancePercentage
            };
        }

    }
}