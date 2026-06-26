using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using SchoolERP.Helpers;

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
        private readonly ISubjectRepository _subjectRepo;
        private readonly IFeeRepository _feeRepo;     
        private readonly IAdminAttendanceRepository _adminAttendanceRepo;  

        public AdminService(
            IAdminRepository adminRepo,
            ITeacherRepository teacherRepo,
            IStudentRepository studentRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            IStudentClassRepository studentClassRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo,
            ISubjectRepository subjectRepo,
            IFeeRepository feeRepo,
            IAdminAttendanceRepository adminAttendanceRepo)
        {
            _adminRepo = adminRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _studentClassRepo = studentClassRepo;
            _teacherAttendanceRepo = teacherAttendanceRepo;
            _subjectRepo = subjectRepo;
            _feeRepo = feeRepo;
            _adminAttendanceRepo = adminAttendanceRepo;
        }

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
                Status = UserStatus.Active,
                Email = dto.Email 
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
                Status = UserStatus.Active,
                Email = dto.Email
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

        public async Task<TeacherResponseDto> AddTeacherAsync(CreateTeacherDto dto)
        {
            bool contactExistsInTeachers = await _teacherRepo.ExistsActiveByContactNoAsync(dto.ContactNo);
            if (contactExistsInTeachers)
                throw new DuplicateContactNoException(dto.ContactNo);

            var adminContact = await _adminRepo.GetActiveAdminContactAsync();
            if (adminContact != null && adminContact == dto.ContactNo)
                throw new DuplicateContactNoException(dto.ContactNo);

            string teacherId = await _teacherRepo.GetNextTeacherIdAsync();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var login = new Login
            {
                Username = teacherId,
                PasswordHash = passwordHash,
                Role = UserRole.Teacher,
                Status = UserStatus.Active,
                Email = dto.Email
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

            var status = await _loginRepo.GetStatusAsync(teacherId);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

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

            var status = await _loginRepo.GetStatusAsync(teacherId);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            return MapToTeacherResponse(teacher);
        }

        public async Task<List<TeacherListDto>> GetAllTeachersAsync()
        {
            var teachers = await _teacherRepo.GetActiveTeachersAsync();
            return teachers.Select(t => new TeacherListDto
            {
                TeacherId = t.TeacherId,
                Name = t.Name,
                ContactNo = t.ContactNo
            }).ToList();
        }

        public async Task<StudentResponseDto> AddStudentAsync(CreateStudentDto dto)
        {
            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            var existingStudents = await _studentRepo.GetStudentsByContactNoAsync(dto.ContactNo);
            if (existingStudents.Any())
            {
                bool isSibling = existingStudents.All(s =>
                    s.FatherName.ToLower() == dto.FatherName.ToLower() &&
                    s.MotherName.ToLower() == dto.MotherName.ToLower());

                if (!isSibling)
                    throw new StudentContactMismatchException(dto.ContactNo);
            }

            string admnNo = await _studentRepo.GetNextStudentIdAsync();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var login = new Login
            {
                Username = admnNo,
                PasswordHash = passwordHash,
                Role = UserRole.Student,
                Status = UserStatus.Active,
                Email = dto.Email
            };
            await _loginRepo.AddAsync(login);

            var student = new Student
            {
                AdmnNo = admnNo,
                RollNo = 0,
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
            studentClass.ClassStrength+=1;
            await _studentClassRepo.UpdateAsync(studentClass);

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

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            login.Status = UserStatus.Inactive;
            student.RollNo = -1;

            var studentClass =await _studentClassRepo.GetAsync(student.Class,student.Sec);
            studentClass.ClassStrength-=1;
            await _studentClassRepo.UpdateAsync(studentClass);

            await _loginRepo.UpdateAsync(login);

            await _logRepo.AddAsync($"Admin removed student '{admnNo}' (soft delete)");

            return true;
        }

        
        /*public async Task<bool> UpdateStudentAsync(string admnNo, string Class, string sec)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var login = await _loginRepo.GetByUsernameAsync(admnNo);
            if (login == null)
                throw new UserNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            // No changes needed
            if (student.Class == Class && student.Sec == sec)
                return true;

            // Old class
            var oldClass = await _studentClassRepo.GetAsync(student.Class, student.Sec);
            if (oldClass != null)
            {
                oldClass.ClassStrength--;
                await _studentClassRepo.UpdateAsync(oldClass);
            }

            // New class
            var newClass = await _studentClassRepo.GetAsync(Class, sec);
            if (newClass == null)
                throw new ClassNotFoundException(Class, sec);

            // Update student
            student.Class = Class;
            student.Sec = sec;

            newClass.ClassStrength++;
            await _studentClassRepo.UpdateAsync(newClass);

            await _studentRepo.UpdateAsync(student);

            await _logRepo.AddAsync(
                $"Admin updated student '{admnNo}' from {oldClass?.Class}-{oldClass?.Sec} to {Class}-{sec}");

            return true;
        }*/

        public async Task<StudentResponseDto> GetStudentAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            return MapToStudentResponse(student);
        }

        public async Task<List<StudentListDto>> GetAllStudentsAsync()
        {
            var students = await _studentRepo.GetActiveStudentsAsync();
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
        /*
        public async Task<StudentClassResponseDto> AddStudentClassAsync(CreateStudentClassDto dto)
        {
            var existing = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (existing != null)
                throw new StudentClassAlreadyExistsException(dto.Class, dto.Sec);

            var teacher = await _teacherRepo.GetByIdAsync(dto.ClassTeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.ClassTeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.ClassTeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.ClassTeacherId);

            var status = await _loginRepo.GetStatusAsync(dto.ClassTeacherId);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(dto.ClassTeacherId);

            var alreadyAssigned = await _studentClassRepo.GetByTeacherIdAsync(dto.ClassTeacherId);
            if (alreadyAssigned != null)
                throw new TeacherAlreadyAssignedAsClassTeacherException(dto.ClassTeacherId);

            if (string.IsNullOrWhiteSpace(dto.SubjectName))
                throw new ArgumentException("Subject name cannot be empty");

            var studentClass = new StudentClass
            {
                Class = dto.Class,
                Sec = dto.Sec,
                ClassTimetable = dto.ClassTimetable,
                ClassTeacherId = dto.ClassTeacherId,
                ClassStrength = dto.ClassStrength
            };
            await _studentClassRepo.AddAsync(studentClass);

            var subject = new Subject
            {
                Class = dto.Class,
                Sec = dto.Sec,
                SubjectName = dto.SubjectName,
                TeacherId = dto.ClassTeacherId
            };
            await _subjectRepo.AddAsync(subject);

            await _logRepo.AddAsync(
                $"Admin added class '{dto.Class}-{dto.Sec}' with class teacher '{dto.ClassTeacherId}' handling subject '{dto.SubjectName}'");

            return new StudentClassResponseDto
            {
                Class = studentClass.Class,
                Sec = studentClass.Sec,
                ClassTimetable = studentClass.ClassTimetable,
                ClassTeacherId = studentClass.ClassTeacherId,
                ClassTeacherName = teacher.Name,
                //SubjectName = dto.SubjectName
            };
        }*/

        public async Task<StudentClassResponseDto> AddStudentClassAsync(CreateStudentClassDto dto)
        {
            string academicYear = AcademicYearHelper.GetCurrentAcademicYear();

            // Check class already exists for this academic year
            var existing = await _studentClassRepo.GetCurrentAsync(dto.Class, dto.Sec, academicYear);
            if (existing != null)
                throw new StudentClassAlreadyExistsException(dto.Class, dto.Sec);

            var teacher = await _teacherRepo.GetByIdAsync(dto.ClassTeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.ClassTeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.ClassTeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.ClassTeacherId);

            // *** MODIFIED ***
            // Check teacher not already class teacher for ANY class in current academic year
            var alreadyAssigned = await _studentClassRepo
                .GetCurrentByClassTeacherIdAsync(dto.ClassTeacherId, academicYear);
            if (alreadyAssigned != null)
                throw new TeacherAlreadyAssignedAsClassTeacherException(dto.ClassTeacherId);

            if (string.IsNullOrWhiteSpace(dto.SubjectName))
                throw new ArgumentException("Subject name cannot be empty");

            var studentClass = new StudentClass
            {
                Class = dto.Class,
                Sec = dto.Sec,
                AcademicYear = academicYear,
                ClassTimetable = dto.ClassTimetable,
                ClassTeacherId = dto.ClassTeacherId,
                ClassStrength = dto.ClassStrength
            };
            await _studentClassRepo.AddAsync(studentClass);

            var subject = new Subject
            {
                ClassId = studentClass.ClassId,
                SubjectName = dto.SubjectName,
                TeacherId = dto.ClassTeacherId
            };
            await _subjectRepo.AddAsync(subject);

            await _logRepo.AddAsync(
                $"Admin added class '{dto.Class}-{dto.Sec}' for year '{academicYear}'");

            return new StudentClassResponseDto
            {
                Class = studentClass.Class,
                Sec = studentClass.Sec,
                ClassTimetable = studentClass.ClassTimetable,
                ClassTeacherId = studentClass.ClassTeacherId,
                ClassTeacherName = teacher.Name,
                //SubjectName = dto.SubjectName,
                ClassStrength = studentClass.ClassStrength
            };
        }

        public async Task<List<StudentClassResponseDto>> GetAllStudentClassesAsync()
        {
            var classes = await _studentClassRepo.GetAllAsync();

            return classes.Select(sc => new StudentClassResponseDto
            {
                Class = sc.Class,
                Sec = sc.Sec,
                ClassTimetable = sc.ClassTimetable,
                ClassTeacherId = sc.ClassTeacherId,
                ClassTeacherName = sc.ClassTeacher?.Name ?? "Unknown",
                ClassStrength = sc.ClassStrength
            }).ToList();
        }

        public async Task<TeacherAttendanceResponseDto> MarkTeacherAttendanceAsync(MarkTeacherAttendanceDto dto)
        {
            if (dto.Status != 0 && dto.Status != 1)
                throw new InvalidAttendanceStatusException();

            var teacher = await _teacherRepo.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.TeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.TeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.TeacherId);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existing = await _teacherAttendanceRepo.GetAsync(dto.TeacherId, today);
            if (existing != null)
                throw new AttendanceAlreadyMarkedException(dto.TeacherId, today);

            AttendanceStatus status = dto.Status == 1
                ? AttendanceStatus.Present
                : AttendanceStatus.Absent;

            var attendance = new TeacherAttendance
            {
                TeacherId = dto.TeacherId,
                Date = today,
                Status = status
            };
            await _teacherAttendanceRepo.AddAsync(attendance);

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
        /*
        public async Task<SubjectResponseDto> AddSubjectAsync(CreateSubjectDto dto)
        {
            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            var teacher = await _teacherRepo.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.TeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.TeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.TeacherId);

            var existing = await _subjectRepo.GetAsync(dto.Class, dto.Sec, dto.SubjectName);
            if (existing != null)
                throw new SubjectAlreadyExistsException(dto.Class, dto.Sec, dto.SubjectName);

            var subject = new Subject
            {
                Class = dto.Class,
                Sec = dto.Sec,
                SubjectName = dto.SubjectName,
                TeacherId = dto.TeacherId
            };

            await _subjectRepo.AddAsync(subject);
            await _logRepo.AddAsync(
                $"Admin added subject '{dto.SubjectName}' for class '{dto.Class}-{dto.Sec}' with teacher '{dto.TeacherId}'");

            return new SubjectResponseDto
            {
                Class = subject.Class,
                Sec = subject.Sec,
                SubjectName = subject.SubjectName,
                TeacherId = subject.TeacherId,
                TeacherName = teacher.Name
            };
        }*/

        public async Task<SubjectResponseDto> AddSubjectAsync(CreateSubjectDto dto)
        {
            // Get ClassId from class+sec using helper
            int classId = await AcademicYearHelper.GetClassIdAsync(
                dto.Class, dto.Sec, _studentClassRepo);

            var teacher = await _teacherRepo.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.TeacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(dto.TeacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(dto.TeacherId);

            var existing = await _subjectRepo.GetAsync(classId, dto.SubjectName);
            if (existing != null)
                throw new SubjectAlreadyExistsException(dto.Class, dto.Sec, dto.SubjectName);

            var subject = new Subject
            {
                ClassId = classId,                         // ← use ClassId
                SubjectName = dto.SubjectName,
                TeacherId = dto.TeacherId
            };
            await _subjectRepo.AddAsync(subject);

            await _logRepo.AddAsync(
                $"Admin added subject '{dto.SubjectName}' for class '{dto.Class}-{dto.Sec}'");

            return new SubjectResponseDto
            {
                Class = dto.Class,
                Sec = dto.Sec,
                SubjectName = dto.SubjectName,
                TeacherId = dto.TeacherId,
                TeacherName = teacher.Name
            };
        }
        /*
        public async Task<List<SubjectResponseDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepo.GetAllAsync();

            return subjects.Select(s => new SubjectResponseDto
            {
                Class = s.Class,
                Sec = s.Sec,
                SubjectName = s.SubjectName,
                TeacherId = s.TeacherId,
                TeacherName = s.Teacher?.Name ?? "Unknown"
            }).ToList();
        }*/
        public async Task<List<SubjectResponseDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return subjects.Select(s => new SubjectResponseDto
            {
                Class = s.StudentClass?.Class ?? "",
                Sec = s.StudentClass?.Sec ?? "",
                SubjectName = s.SubjectName,
                TeacherId = s.TeacherId,
                TeacherName = s.Teacher?.Name ?? "Unknown"
            }).ToList();
        }
        /*
        public async Task<List<SubjectResponseDto>> GetSubjectsByClassAsync(string Class, string sec)
        {
            var studentClass = await _studentClassRepo.GetAsync(Class, sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(Class, sec);

            var subjects = await _subjectRepo.GetByClassAsync(Class, sec);

            return subjects.Select(s => new SubjectResponseDto
            {
                Class = s.Class,
                Sec = s.Sec,
                SubjectName = s.SubjectName,
                TeacherId = s.TeacherId,
                TeacherName = s.Teacher?.Name ?? "Unknown"
            }).ToList();
        }*/

        public async Task<List<SubjectResponseDto>> GetSubjectsByClassAsync(string Class, string sec)
        {
            int classId = await AcademicYearHelper.GetClassIdAsync(Class, sec, _studentClassRepo);
            var subjects = await _subjectRepo.GetByClassIdAsync(classId);

            return subjects.Select(s => new SubjectResponseDto
            {
                Class = Class,
                Sec = sec,
                SubjectName = s.SubjectName,
                TeacherId = s.TeacherId,
                TeacherName = s.Teacher?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<bool> AssignRollNumbersAsync(string Class, string sec)
        {
            var studentClass = await _studentClassRepo.GetAsync(Class, sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(Class, sec);

            var students = await _studentRepo.GetByClassAsync(Class, sec);

            if (!students.Any())
            {
                throw new Exception($"No active students found in class '{Class}-{sec}'");
            }

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

        public async Task<List<FeeResponseDto>> AddFeeAsync(CreateFeeDto dto)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DueDate < today)
                throw new InvalidDueDateException();

            var students = await _studentRepo.GetAllByClassAsync(dto.Class);
            if (!students.Any())
                throw new NoStudentsInClassException(dto.Class);

            var fees = students.Select(s => new Fee
            {
                Name = dto.Name,
                Amount = dto.Amount,
                AdmnNo = s.AdmnNo,
                DueDate = dto.DueDate,
                Status = FeeStatus.Unpaid
            }).ToList();

            await _feeRepo.AddRangeAsync(fees);

            await _logRepo.AddAsync(
                $"Admin added fee '{dto.Name}' of amount '{dto.Amount}' for class '{dto.Class}' — {fees.Count} records created");

            return fees.Select(f => new FeeResponseDto
            {
                FeeId = f.FeeId,
                Name = f.Name,
                Amount = f.Amount,
                AdmnNo = f.AdmnNo,
                DueDate = f.DueDate,
                Status = f.Status.ToString()
            }).ToList();
        }

        public async Task<AdminProfileResponseDto> GetMyProfileAsync(string adminId)
        {
            var admin = await _adminRepo.GetByIdAsync(adminId);
            if (admin == null)
                throw new AdminNotFoundException(adminId);

            await _logRepo.AddAsync($"Admin '{adminId}' viewed their profile");

            return new AdminProfileResponseDto
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                DOB = admin.DOB,
                Gender = admin.Gender,
                BloodGrp = admin.BloodGrp,
                ContactNo = admin.ContactNo,
                PhotoUrl = admin.PhotoUrl,
                TotalDays = admin.TotalDays,                      
                PresentDays = admin.PresentDays,                   
                AttendancePercentage = admin.AttendancePercentage
            };
        }

        public async Task<AdminAttendanceResponseDto> MarkMyAttendanceAsync(
            string adminId, MarkAdminAttendanceDto dto)
        {
            if (dto.Status != 0 && dto.Status != 1)
                throw new InvalidAttendanceStatusException();

            var admin = await _adminRepo.GetByIdAsync(adminId);
            if (admin == null)
                throw new AdminNotFoundException(adminId);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existing = await _adminAttendanceRepo.GetAsync(adminId, today);
            if (existing != null)
                throw new AdminAttendanceAlreadyMarkedException(adminId, today);

            AttendanceStatus status = dto.Status == 1
                ? AttendanceStatus.Present
                : AttendanceStatus.Absent;

            var attendance = new AdminAttendance
            {
                AdminId = adminId,
                Date = today,
                Status = status
            };
            await _adminAttendanceRepo.AddAsync(attendance);

            admin.TotalDays += 1;
            if (status == AttendanceStatus.Present)
            {
                admin.PresentDays += 1;
            }
            admin.AttendancePercentage = ((decimal)admin.PresentDays / admin.TotalDays) * 100;
            await _adminRepo.UpdateAsync(admin);

            await _logRepo.AddAsync(
                $"Admin '{adminId}' marked their own attendance as '{status}' on '{today}'");

            return new AdminAttendanceResponseDto
            {
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                Date = today,
                Status = status.ToString(),
                TotalDays = admin.TotalDays,
                PresentDays = admin.PresentDays,
                AttendancePercentage = admin.AttendancePercentage
            };
        }

        public async Task<List<AdminLeaveDetailsResponseDto>> GetMyLeaveDetailsAsync(string adminId)
        {
            var admin = await _adminRepo.GetByIdAsync(adminId);
            if (admin == null)
                throw new AdminNotFoundException(adminId);

            var absentDates = await _adminAttendanceRepo.GetAbsentDatesAsync(adminId);

            await _logRepo.AddAsync($"Admin '{adminId}' viewed their leave details");

            return absentDates.Select(a => new AdminLeaveDetailsResponseDto
            {
                Date = a.Date
            }).ToList();
        }

        public async Task<StudentClassResponseDto> UpdateClassTimetableAsync(UpdateClassTimetableDto dto)
        {
            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            bool urlExists = await _studentClassRepo.ExistsByTimetableAsync(dto.ClassTimetable);
            if (urlExists && studentClass.ClassTimetable != dto.ClassTimetable)
                throw new ClassTimetableAlreadyExistsException(dto.ClassTimetable);

            if(studentClass.ClassTimetable == dto.ClassTimetable)
            {
                throw new SameTimeTableException(dto.ClassTimetable);
            }

            studentClass.ClassTimetable = dto.ClassTimetable;
            await _studentClassRepo.UpdateAsync(studentClass);

            await _logRepo.AddAsync(
                $"Admin updated timetable for class '{dto.Class}-{dto.Sec}'");

            var teacher = await _teacherRepo.GetByIdAsync(studentClass.ClassTeacherId);

            return new StudentClassResponseDto
            {
                Class = studentClass.Class,
                Sec = studentClass.Sec,
                ClassTimetable = studentClass.ClassTimetable,
                ClassTeacherId = studentClass.ClassTeacherId,
                ClassTeacherName = teacher?.Name ?? "Unknown",
                //SubjectName = "",
                ClassStrength = studentClass.ClassStrength
            };
        }

        public async Task<TeacherResponseDto> UpdateTeacherTimetableAsync(UpdateTeacherTimetableDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(dto.TeacherId);
            
            var status = await _loginRepo.GetStatusAsync(dto.TeacherId);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(dto.TeacherId);

            bool urlExists = await _teacherRepo.ExistsByTimetableAsync(dto.TimeTableUrl);
            if (urlExists && teacher.TimeTableUrl != dto.TimeTableUrl)
                throw new TeacherTimetableAlreadyExistsException(dto.TimeTableUrl);

            if(teacher.TimeTableUrl == dto.TimeTableUrl)
            {
                throw new SameTimeTableException(dto.TimeTableUrl);
            }
            teacher.TimeTableUrl = dto.TimeTableUrl;
            await _teacherRepo.UpdateAsync(teacher);

            await _logRepo.AddAsync(
                $"Admin updated timetable for teacher '{dto.TeacherId}'");

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

        public async Task<PromoteClassResponseDto> PromoteClassAsync(PromoteClassDto dto)
        {
            // ── Step 1: Get previous academic year ───────────────────────
            string? previousYear = await _studentClassRepo.GetPreviousAcademicYearAsync();
            if (previousYear == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            string currentYear = AcademicYearHelper.GetCurrentAcademicYear();

            // ── Step 2: Get the previous year's class ─────────────────────
            var previousClass = await _studentClassRepo.GetCurrentAsync(
                dto.Class, dto.Sec, previousYear);
            if (previousClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            // ── Step 3: Validate promotion list length ────────────────────
            if (dto.Promotions.Count != previousClass.ClassStrength)
                throw new PromotionListMismatchException(
                    previousClass.ClassStrength, dto.Promotions.Count);

            // ── Step 4: Validate all promotion values ─────────────────────
            foreach (var p in dto.Promotions)
            {
                if (p != "-1" && p != "0" &&
                    !System.Text.RegularExpressions.Regex.IsMatch(p, @"^[A-Za-z]$"))
                    throw new InvalidPromotionValueException(p);
            }

            // ── Step 5: Determine next class ─────────────────────────────
            bool isClass12 = dto.Class == "12";
            string nextClass = isClass12
                ? dto.Class
                : (int.Parse(dto.Class) + 1).ToString();

            // ── Step 6: Check all target classes exist for current year ───
            // Collect all unique target class+sec combinations
            var targetClasses = dto.Promotions
                .Where(p => p != "-1" && p != "0")
                .Select(p => new { Class = nextClass, Sec = p.ToUpper() })
                .Distinct()
                .ToList();

            // Also add same class+sec for failed students (value = "0")
            bool hasFailed = dto.Promotions.Any(p => p == "0");
            if (hasFailed)
                targetClasses.Add(new { Class = dto.Class, Sec = dto.Sec });

            // Check each target class exists in current academic year
            var missingClasses = new List<string>();
            foreach (var tc in targetClasses)
            {
                var exists = await _studentClassRepo.GetCurrentAsync(tc.Class, tc.Sec, currentYear);
                if (exists == null)
                    missingClasses.Add($"{tc.Class}-{tc.Sec}");
            }
            if (missingClasses.Any())
                throw new ClassNotFoundForNewYearException(missingClasses, currentYear);

            // ── Step 7: Get students ordered by roll number ───────────────
            var students = await _studentRepo.GetByClassSecOrderedAsync(dto.Class, dto.Sec);
            if (students.Any(s => s.RollNo == 0))
                throw new RollNumberNotAssignedException(dto.Class, dto.Sec);

            // ── Step 8: Process each student ─────────────────────────────
            int promoted = 0, failed = 0, left = 0;
            var details = new List<PromotionDetailDto>();

            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                string promotion = dto.Promotions[i];

                var detail = new PromotionDetailDto
                {
                    AdmnNo = student.AdmnNo,
                    Name = student.Name,
                    OldRollNo = student.RollNo,
                    OldClass = student.Class,
                    OldSec = student.Sec
                };

                if (promotion == "-1" || (isClass12 && promotion != "0"))
                {
                    // Student leaves school → inactive
                    var login = await _loginRepo.GetByUsernameAsync(student.AdmnNo);
                    if (login != null)
                    {
                        login.Status = UserStatus.Inactive;
                        await _loginRepo.UpdateAsync(login);
                    }
                    student.RollNo = -1;
                    detail.NewClass = student.Class;
                    detail.NewSec = student.Sec;
                    detail.Status = "Left";
                    left++;
                }
                else if (promotion == "0")
                {
                    // Student fails → stays in same class+sec, roll reset to 0
                    student.RollNo = 0;
                    detail.NewClass = student.Class;
                    detail.NewSec = student.Sec;
                    detail.Status = "Failed";
                    failed++;
                }
                else
                {
                    // Student promoted → move to next class + new section
                    string newSec = promotion.ToUpper();
                    student.Class = nextClass;
                    student.Sec = newSec;
                    student.RollNo = 0; // reset, will be reassigned
                    detail.NewClass = nextClass;
                    detail.NewSec = newSec;
                    detail.Status = "Promoted";
                    promoted++;
                }

                details.Add(detail);
                await _studentRepo.UpdateAsync(student);
            }

            // ── Step 9: Update ClassStrength for all affected classes ─────
            // Recalculate strength for each class in current year
            // that received students or lost students
            var affectedClasses = details
                .Select(d => new { d.NewClass, d.NewSec })
                .Distinct()
                .ToList();

            foreach (var ac in affectedClasses)
            {
                var classRecord = await _studentClassRepo.GetCurrentAsync(
                    ac.NewClass, ac.NewSec, currentYear);
                if (classRecord != null)
                {
                    // Count active students in this class
                    var activeCount = await _studentRepo.CountActiveInClassAsync(
                        ac.NewClass, ac.NewSec);
                    classRecord.ClassStrength = activeCount;
                    await _studentClassRepo.UpdateAsync(classRecord);
                }
            }

            await _logRepo.AddAsync(
                $"Admin promoted class '{dto.Class}-{dto.Sec}' from '{previousYear}' " +
                $"→ Promoted: {promoted}, Failed: {failed}, Left: {left}");

            return new PromoteClassResponseDto
            {
                PreviousClass = dto.Class,
                PreviousSec = dto.Sec,
                AcademicYear = currentYear,
                Promoted = promoted,
                Failed = failed,
                Left = left,
                Details = details
            };
        }
    }
}