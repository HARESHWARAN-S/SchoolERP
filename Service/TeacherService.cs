using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using SchoolERP.Models.Enums;
using SchoolERP.Helpers;

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
        private readonly IPTMRepository _ptmRepo;
        
        public TeacherService(
            ITeacherRepository teacherRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            ITeacherAttendanceRepository teacherAttendanceRepo,
            IHomeworkRepository homeworkRepo,       
            ISubjectRepository subjectRepo,         
            IStudentRepository studentRepo,
            IPTMRepository ptmRepo,
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
            _ptmRepo = ptmRepo;
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
        /*
        public async Task<HomeworkResponseDto> AddHomeworkAsync(string teacherId, CreateHomeworkDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            var subject = await _subjectRepo.GetByClassSecSubjectTeacherAsync(
                dto.Class, dto.Sec, dto.Subject, teacherId);
            if (subject == null)
                throw new UnauthorizedSubjectAccessException(teacherId, dto.Subject, dto.Class, dto.Sec);

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
        }*/

        public async Task<HomeworkResponseDto> AddHomeworkAsync(string teacherId, CreateHomeworkDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Get ClassId from class+sec using helper
            int classId = await AcademicYearHelper.GetClassIdAsync(
                dto.Class, dto.Sec, _studentClassRepo);

            // Check teacher assigned to this subject for this class
            var subject = await _subjectRepo.GetByClassSecSubjectTeacherAsync(
                classId, dto.Subject, teacherId);
            if (subject == null)
                throw new UnauthorizedSubjectAccessException(teacherId, dto.Subject, dto.Class, dto.Sec);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existing = await _homeworkRepo.GetAsync(classId, dto.Subject, today);
            if (existing != null)
                throw new HomeworkAlreadyExistsException(dto.Class, dto.Sec, dto.Subject, today);

            var homework = new Homework
            {
                ClassId = classId,                         // ← use ClassId
                Date = today,
                Subject = dto.Subject,
                Description = dto.Description
            };
            await _homeworkRepo.AddAsync(homework);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' added homework for subject '{dto.Subject}'");

            return new HomeworkResponseDto
            {
                HomeworkId = homework.HomeworkId,
                Class = dto.Class,
                Sec = dto.Sec,
                Date = homework.Date,
                Subject = homework.Subject,
                Description = homework.Description
            };
        }
        /*
        public async Task<List<StudentAttendanceResponseDto>> MarkStudentAttendanceAsync(
            string teacherId, MarkStudentAttendanceDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            var studentClass = await _studentClassRepo.GetByClassTeacherIdAsync(teacherId);
            if (studentClass == null)
                throw new NotAClassTeacherException(teacherId);

            if (dto.Attendance.Any(a => a != 0 && a != 1))
                throw new InvalidAttendanceValueException();

            if (dto.Attendance.Count != studentClass.ClassStrength)
                throw new AttendanceStrengthMismatchException(studentClass.ClassStrength, dto.Attendance.Count);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool alreadyMarked = await _studentAttendanceRepo.ExistsAsync(
                studentClass.Class, studentClass.Sec, today);
            if (alreadyMarked)
                throw new AttendanceAlreadyMarkedException(studentClass.Class, studentClass.Sec, today);

            var students = await _studentRepo.GetByClassSecOrderedAsync(
                studentClass.Class, studentClass.Sec);

            var unassigned = students.Where(s => s.RollNo.Equals(0)).Any();
            if (unassigned)
                throw new RollNumberNotAssignedException(studentClass.Class, studentClass.Sec);

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

            await _studentAttendanceRepo.AddRangeAsync(attendanceRecords);

            await _studentRepo.UpdateRangeAsync(students);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' marked attendance for class '{studentClass.Class}-{studentClass.Sec}' on '{today}'");

            return result;
        }*/
        public async Task<List<StudentAttendanceResponseDto>> MarkStudentAttendanceAsync(
            string teacherId, MarkStudentAttendanceDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Get current year class for this class teacher
            string academicYear = AcademicYearHelper.GetCurrentAcademicYear();
            var studentClass = await _studentClassRepo.GetCurrentByClassTeacherIdAsync(
                teacherId, academicYear);
            if (studentClass == null)
                throw new NotAClassTeacherException(teacherId);

            if (dto.Attendance.Any(a => a != 0 && a != 1))
                throw new InvalidAttendanceValueException();

            if (dto.Attendance.Count != studentClass.ClassStrength)
                throw new AttendanceStrengthMismatchException(
                    studentClass.ClassStrength, dto.Attendance.Count);

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool alreadyMarked = await _studentAttendanceRepo.ExistsAsync(studentClass.ClassId, today);
            if (alreadyMarked)
                throw new AttendanceAlreadyMarkedException(studentClass.Class, studentClass.Sec, today);

            var students = await _studentRepo.GetByClassSecOrderedAsync(
                studentClass.Class, studentClass.Sec);

            if (students.Any(s => s.RollNo == 0))
                throw new RollNumberNotAssignedException(studentClass.Class, studentClass.Sec);

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
                    ClassId = studentClass.ClassId,        // ← use ClassId
                    AdmnNo = student.AdmnNo,
                    Date = today,
                    Status = status
                });

                student.TotalDays += 1;
                if (status == AttendanceStatus.Present)
                    student.PresentDays += 1;
                student.AttendancePercentage =
                    ((decimal)student.PresentDays / student.TotalDays) * 100;

                result.Add(new StudentAttendanceResponseDto
                {
                    AdmnNo = student.AdmnNo,
                    Name = student.Name,
                    RollNo = student.RollNo,
                    Date = today,
                    Status = status.ToString()
                });
            }

            await _studentAttendanceRepo.AddRangeAsync(attendanceRecords);
            await _studentRepo.UpdateRangeAsync(students);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' marked attendance for class '{studentClass.Class}-{studentClass.Sec}'");

            return result;
        }

        /*
        public async Task<List<MarkEntryResponseDto>> AddMarksAsync(
            string teacherId, MarkEntryDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            var subjectRecord = await _subjectRepo.GetByTeacherAsync(dto.Class, dto.Sec, teacherId);
            if (subjectRecord == null)
                throw new UnauthorizedSubjectAccessException(teacherId, "", dto.Class, dto.Sec);

            string subjectName = subjectRecord.SubjectName; 

            var studentClass = await _studentClassRepo.GetAsync(dto.Class, dto.Sec);
            if (studentClass == null)
                throw new StudentClassNotFoundException(dto.Class, dto.Sec);

            if (dto.Marks.Count != studentClass.ClassStrength)
                throw new MarkListStrengthMismatchException(studentClass.ClassStrength, dto.Marks.Count);

            foreach (var mark in dto.Marks)
            {
                if (mark != -1 && (mark < 0 || mark > dto.TotalMarks))
                    throw new MarksOutOfRangeException(mark, dto.TotalMarks);
            }

            bool alreadyEntered = await _markRepo.ExistsAsync(
                dto.ExamName, subjectName, dto.Class, dto.Sec);
            if (alreadyEntered)
                throw new MarksAlreadyEnteredForExamException(
                    dto.ExamName, subjectName, dto.Class, dto.Sec);

            var students = await _studentRepo.GetByClassSecOrderedAsync(dto.Class, dto.Sec);

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
                    Subject = subjectName,
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
                    Subject = subjectName, 
                    Date = today,
                    MarksObtained = markValue == -1 ? "Absent" : markValue.ToString(),
                    TotalMarks = dto.TotalMarks
                });
            }

            await _markRepo.AddRangeAsync(markRecords);
            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' entered marks for exam '{dto.ExamName}' subject '{subjectName}' in class '{dto.Class}-{dto.Sec}'");

            return result;
        }*/
        public async Task<List<MarkEntryResponseDto>> AddMarksAsync(string teacherId, MarkEntryDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Get ClassId from class+sec using helper
            int classId = await AcademicYearHelper.GetClassIdAsync(
                dto.Class, dto.Sec, _studentClassRepo);

            // Auto fetch subject using ClassId + teacherId
            var subjectRecord = await _subjectRepo.GetByTeacherAsync(classId, teacherId);
            if (subjectRecord == null)
                throw new UnauthorizedSubjectAccessException(teacherId, "", dto.Class, dto.Sec);

            string subjectName = subjectRecord.SubjectName;

            var studentClass = await _studentClassRepo.GetCurrentAsync(
                dto.Class, dto.Sec, AcademicYearHelper.GetCurrentAcademicYear());

            if (dto.Marks.Count != studentClass!.ClassStrength)
                throw new MarkListStrengthMismatchException(studentClass.ClassStrength, dto.Marks.Count);

            foreach (var mark in dto.Marks)
            {
                if (mark != -1 && (mark < 0 || mark > dto.TotalMarks))
                    throw new MarksOutOfRangeException(mark, dto.TotalMarks);
            }

            bool alreadyEntered = await _markRepo.ExistsAsync(dto.ExamName, subjectName, classId);
            if (alreadyEntered)
                throw new MarksAlreadyEnteredForExamException(
                    dto.ExamName, subjectName, dto.Class, dto.Sec);

            var students = await _studentRepo.GetByClassSecOrderedAsync(dto.Class, dto.Sec);

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
                    Subject = subjectName,
                    Date = today,
                    ClassId = classId,                     // ← use ClassId
                    MarksObtained = markValue,
                    TotalMarks = dto.TotalMarks
                });

                result.Add(new MarkEntryResponseDto
                {
                    AdmnNo = student.AdmnNo,
                    Name = student.Name,
                    RollNo = student.RollNo,
                    ExamName = dto.ExamName,
                    Subject = subjectName,
                    Date = today,
                    MarksObtained = markValue == -1 ? "Absent" : markValue.ToString(),
                    TotalMarks = dto.TotalMarks
                });
            }

            await _markRepo.AddRangeAsync(markRecords);
            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' entered marks for exam '{dto.ExamName}' subject '{subjectName}'");

            return result;
        }

        /*
        public async Task<UpdateMarkResponseDto> UpdateMarksAsync(string teacherId, UpdateMarkDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            var teacherStatus = await _loginRepo.GetStatusAsync(teacherId);
            if (teacherStatus == UserStatus.Inactive)
                throw new UserInactiveException(teacherId);

            var subjectRecord = await _subjectRepo.GetByTeacherAsync(dto.Class, dto.Sec, teacherId);
            if (subjectRecord == null)
                throw new UnauthorizedSubjectAccessException(teacherId, "", dto.Class, dto.Sec);

            string subjectName = subjectRecord.SubjectName;

            var student = await _studentRepo.GetByClassSecRollNoAsync(dto.Class, dto.Sec, dto.RollNo);
            if (student == null)
                throw new StudentNotFoundException($"RollNo {dto.RollNo} in class {dto.Class}-{dto.Sec}");

            var mark = await _markRepo.GetByAdmnNoExamSubjectAsync(
                student.AdmnNo, dto.ExamName, subjectName);
            if (mark == null)
                throw new MarkRecordNotFoundException(
                    dto.RollNo, dto.ExamName, subjectName, dto.Class, dto.Sec);

            if (dto.NewMarks != -1 && (dto.NewMarks < 0 || dto.NewMarks > mark.TotalMarks))
                throw new MarksOutOfRangeException(dto.NewMarks, mark.TotalMarks);

            mark.MarksObtained = dto.NewMarks;
            await _markRepo.UpdateAsync(mark);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' updated marks for roll no '{dto.RollNo}' exam '{dto.ExamName}' subject '{subjectName}' in class '{dto.Class}-{dto.Sec}'");

            return new UpdateMarkResponseDto
            {
                AdmnNo = student.AdmnNo,
                Name = student.Name,
                RollNo = student.RollNo,
                ExamName = mark.ExamName,
                Subject = mark.Subject,
                Date = mark.Date,
                MarksObtained = mark.MarksObtained == -1 ? "Absent" : mark.MarksObtained.ToString(),
                TotalMarks = mark.TotalMarks
            };
        }*/
        public async Task<UpdateMarkResponseDto> UpdateMarksAsync(string teacherId, UpdateMarkDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Get ClassId from class+sec using helper
            int classId = await AcademicYearHelper.GetClassIdAsync(
                dto.Class, dto.Sec, _studentClassRepo);

            // Auto fetch subject
            var subjectRecord = await _subjectRepo.GetByTeacherAsync(classId, teacherId);
            if (subjectRecord == null)
                throw new UnauthorizedSubjectAccessException(teacherId, "", dto.Class, dto.Sec);

            string subjectName = subjectRecord.SubjectName;

            var student = await _studentRepo.GetByClassSecRollNoAsync(dto.Class, dto.Sec, dto.RollNo);
            if (student == null)
                throw new StudentNotFoundException($"RollNo {dto.RollNo} in class {dto.Class}-{dto.Sec}");

            var mark = await _markRepo.GetByAdmnNoExamSubjectAsync(
                student.AdmnNo, dto.ExamName, subjectName);
            if (mark == null)
                throw new MarkRecordNotFoundException(
                    dto.RollNo, dto.ExamName, subjectName, dto.Class, dto.Sec);

            if (dto.NewMarks != -1 && (dto.NewMarks < 0 || dto.NewMarks > mark.TotalMarks))
                throw new MarksOutOfRangeException(dto.NewMarks, mark.TotalMarks);

            mark.MarksObtained = dto.NewMarks;
            await _markRepo.UpdateAsync(mark);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' updated marks for roll no '{dto.RollNo}' exam '{dto.ExamName}'");

            return new UpdateMarkResponseDto
            {
                AdmnNo = student.AdmnNo,
                Name = student.Name,
                RollNo = student.RollNo,
                ExamName = mark.ExamName,
                Subject = mark.Subject,
                Date = mark.Date,
                MarksObtained = mark.MarksObtained == -1 ? "Absent" : mark.MarksObtained.ToString(),
                TotalMarks = mark.TotalMarks
            };
        }

        public async Task<PTMResponseDto> AddPTMAsync(string teacherId, AddPTMDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new TeacherNotFoundException(teacherId);

            // Get ClassId from class+sec using helper
            int classId = await AcademicYearHelper.GetClassIdAsync(
                dto.Class, dto.Sec, _studentClassRepo);

            // Check teacher is class teacher OR subject teacher for this class
            var isClassTeacher = await _studentClassRepo.GetCurrentByClassTeacherIdAsync(
                teacherId, AcademicYearHelper.GetCurrentAcademicYear());
            var isSubjectTeacher = await _subjectRepo.GetByTeacherAsync(classId, teacherId);

            bool hasAccess = (isClassTeacher != null && isClassTeacher.ClassId == classId)
                        || (isSubjectTeacher != null);

            if (!hasAccess)
                throw new NotAuthorizedForClassException(teacherId, dto.Class, dto.Sec);

            // Get student by rollNo
            var student = await _studentRepo.GetByClassSecRollNoAsync(dto.Class, dto.Sec, dto.RollNo);
            if (student == null)
                throw new StudentNotFoundException($"RollNo {dto.RollNo} in {dto.Class}-{dto.Sec}");

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Check duplicate PTM entry
            var existing = await _ptmRepo.GetAsync(student.AdmnNo, teacherId, today);
            if (existing != null)
                throw new PTMAlreadyExistsException(student.AdmnNo, teacherId, today);

            // Get subject name for this teacher in this class
            string subjectName = isSubjectTeacher?.SubjectName ?? "Class Teacher";

            var ptm = new PTM
            {
                Date = today,
                AdmnNo = student.AdmnNo,
                TeacherId = teacherId,
                ClassId = classId,
                Remarks = dto.Remarks
            };
            await _ptmRepo.AddAsync(ptm);

            await _logRepo.AddAsync(
                $"Teacher '{teacherId}' added PTM entry for student '{student.AdmnNo}'");

            return new PTMResponseDto
            {
                Date = ptm.Date,
                AdmnNo = student.AdmnNo,
                StudentName = student.Name,
                TeacherId = teacherId,
                TeacherName = teacher.Name,
                Subject = subjectName,
                Remarks = ptm.Remarks
            };
        }
    }
}