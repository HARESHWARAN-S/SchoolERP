using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Entities;
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
        private readonly IFeeRepository _feeRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IStripeService _stripeService;
        private readonly IStudentAttendanceRepository _studentAttendanceRepo;
        private readonly IMarkRepository _markRepo;

        public StudentService(
            IStudentRepository studentRepo,
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            INotificationRepository notificationRepo,
            IStudentClassRepository studentClassRepo,
            IHomeworkRepository homeworkRepo,
            IFeeRepository feeRepo,
            IPaymentRepository paymentRepo,
            IStripeService stripeService,
            IStudentAttendanceRepository studentAttendanceRepo,
            IMarkRepository markRepo)
        {
            _studentRepo = studentRepo;
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _notificationRepo = notificationRepo;
            _homeworkRepo = homeworkRepo;
            _studentClassRepo = studentClassRepo;
            _feeRepo = feeRepo;
            _paymentRepo = paymentRepo;
            _stripeService = stripeService;
            _studentAttendanceRepo = studentAttendanceRepo;
            _markRepo = markRepo;
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
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var homeworks = await _homeworkRepo.GetByClassAsync(student.Class, student.Sec);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed homework");

            return homeworks.Select(h => new StudentHomeworkResponseDto
            {
                Date = h.Date,
                Subject = h.Subject,
                Description = h.Description
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

        public async Task<PayFeeResponseDto> PayFeeAsync(string admnNo, int feeId)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var fee = await _feeRepo.GetByIdAsync(feeId);
            if (fee == null)
                throw new FeeNotFoundException(feeId);

            if (fee.AdmnNo != admnNo)
                throw new FeeNotBelongToStudentException(feeId, admnNo);

            if (fee.Status == FeeStatus.Paid)
                throw new FeeAlreadyPaidException(feeId);

            string stripePaymentId = await _stripeService.MockChargeAsync(fee.Amount, admnNo, feeId);

            fee.Status = FeeStatus.Paid;
            await _feeRepo.UpdateAsync(fee);

            var payment = new Payment
            {
                FeeId = feeId,
                AdmnNo = admnNo,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Amount = fee.Amount,
                StripePaymentId = stripePaymentId
            };
            var savedPayment = await _paymentRepo.AddAsync(payment);

            await _logRepo.AddAsync(
                $"Student '{admnNo}' paid fee '{feeId}' — StripeId: '{stripePaymentId}'");

            return new PayFeeResponseDto
            {
                InvoiceNo = savedPayment.InvoiceNo,
                FeeId = savedPayment.FeeId,
                AdmnNo = savedPayment.AdmnNo,
                Date = savedPayment.Date,
                StripePaymentId = savedPayment.StripePaymentId
            };
        }

        public async Task<List<PaymentHistoryResponseDto>> GetPaymentHistoryAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var payments = await _paymentRepo.GetByStudentAsync(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed payment history");

            return payments.Select(p => new PaymentHistoryResponseDto
            {
                InvoiceNo = p.InvoiceNo,
                Date = p.Date,
                Amount = p.Amount,
                StripePaymentId = p.StripePaymentId
            }).ToList();
        }

        public async Task<List<FeeDueResponseDto>> GetFeeDueAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var fees = await _feeRepo.GetDueByStudentAsync(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed fee dues");

            return fees.Select(f => new FeeDueResponseDto
            {
                FeeId = f.FeeId,
                Name = f.Name,
                Amount = f.Amount,
                DueDate = f.DueDate
            }).ToList();
        }

        public async Task<List<LeaveDetailsResponseDto>> GetLeaveDetailsAsync(string admnNo)
        {
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var absentDates = await _studentAttendanceRepo.GetAbsentDatesAsync(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed leave details");

            return absentDates.Select(a => new LeaveDetailsResponseDto
            {
                Date = a.Date
            }).ToList();
        }

        public async Task<List<StudentMarkResponseDto>> GetMarksAsync(string admnNo)
        {
            // Check student exists and is active
            var student = await _studentRepo.GetByIdAsync(admnNo);
            if (student == null)
                throw new StudentNotFoundException(admnNo);

            var status = await _loginRepo.GetStatusAsync(admnNo);
            if (status == UserStatus.Inactive)
                throw new UserInactiveException(admnNo);

            var marks = await _markRepo.GetByStudentAsync(admnNo);

            await _logRepo.AddAsync($"Student '{admnNo}' viewed marks");

            return marks.Select(m => new StudentMarkResponseDto
            {
                ExamName = m.ExamName,
                Subject = m.Subject,
                Date = m.Date,
                MarksObtained = m.MarksObtained == -1 ? "Absent" : m.MarksObtained.ToString(),
                TotalMarks = m.TotalMarks
            }).ToList();
        }
    }
}