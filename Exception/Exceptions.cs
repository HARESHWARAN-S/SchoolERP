namespace SchoolERP.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string username)
            : base($"User '{username}' not found") { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid username or password") { }
    }

    public class UserInactiveException : Exception
    {
        public UserInactiveException(string username)
            : base($"User '{username}' is inactive") { }
    }

    public class AdminAlreadyExistsException : Exception
    {
        public AdminAlreadyExistsException()
            : base("An admin already exists. Use the add-admin endpoint instead.") { }
    }

    public class AdminNotFoundException : Exception
    {
        public AdminNotFoundException(string adminId)
            : base($"Admin '{adminId}' not found") { }
    }

    public class TeacherNotFoundException : Exception
    {
        public TeacherNotFoundException(string teacherId)
            : base($"Teacher '{teacherId}' not found") { }
    }

    public class StudentNotFoundException : Exception
    {
        public StudentNotFoundException(string admnNo)
            : base($"Student '{admnNo}' not found") { }
    }

    public class SamePasswordException : Exception
    {
        public SamePasswordException()
            : base("New password cannot be the same as old password") { }
    }

    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException()
            : base("Current password is incorrect") { }
    }

        public class StudentClassNotFoundException : Exception
    {
        public StudentClassNotFoundException(string Class, string sec)
            : base($"Class '{Class}-{sec}' not found") { }
    }

    public class StudentClassAlreadyExistsException : Exception
    {
        public StudentClassAlreadyExistsException(string Class, string sec)
            : base($"Class '{Class}-{sec}' already exists") { }
    }

    public class TeacherAlreadyAssignedAsClassTeacherException : Exception
    {
        public TeacherAlreadyAssignedAsClassTeacherException(string teacherId)
            : base($"Teacher '{teacherId}' is already a class teacher of another class") { }
    }

    public class AttendanceAlreadyMarkedException : Exception
    {
        public AttendanceAlreadyMarkedException(string teacherId, DateOnly date)
            : base($"Attendance for teacher '{teacherId}' on '{date}' is already marked") { }
        public AttendanceAlreadyMarkedException(string Class, string sec, DateOnly date)
            : base($"Attendance for class '{Class}-{sec}' on '{date}' is already marked") { }
    }

    public class InvalidAttendanceStatusException : Exception
    {
        public InvalidAttendanceStatusException()
            : base("Invalid attendance status. Use 0 for Absent and 1 for Present") { }
    }
        public class SubjectAlreadyExistsException : Exception
    {
        public SubjectAlreadyExistsException(string Class, string sec, string subject)
            : base($"Subject '{subject}' already exists for class '{Class}-{sec}'") { }
    }

    public class SubjectNotFoundException : Exception
    {
        public SubjectNotFoundException(string Class, string sec, string subject)
            : base($"Subject '{subject}' not found for class '{Class}-{sec}'") { }
    }
        public class HomeworkAlreadyExistsException : Exception
    {
        public HomeworkAlreadyExistsException(string Class, string sec, string subject, DateOnly date)
            : base($"Homework for subject '{subject}' in class '{Class}-{sec}' on '{date}' already exists") { }
    }

    public class UnauthorizedSubjectAccessException : Exception
    {
        public UnauthorizedSubjectAccessException(string teacherId, string subject, string Class, string sec)
            : base($"Teacher '{teacherId}' is not assigned to subject '{subject}' for class '{Class}-{sec}'") { }
    }

        public class FeeNotFoundException : Exception
    {
        public FeeNotFoundException(int feeId)
            : base($"Fee with ID '{feeId}' not found") { }
    }

    public class FeeAlreadyPaidException : Exception
    {
        public FeeAlreadyPaidException(int feeId)
            : base($"Fee with ID '{feeId}' is already paid") { }
    }

    public class InvalidDueDateException : Exception
    {
        public InvalidDueDateException()
            : base("Due date cannot be in the past") { }
    }

    public class NoStudentsInClassException : Exception
    {
        public NoStudentsInClassException(string Class)
            : base($"No active students found in class '{Class}'") { }
    }

    public class FeeNotBelongToStudentException : Exception
    {
        public FeeNotBelongToStudentException(int feeId, string admnNo)
            : base($"Fee '{feeId}' does not belong to student '{admnNo}'") { }
    }

        public class NotAClassTeacherException : Exception
    {
        public NotAClassTeacherException(string teacherId)
            : base($"Teacher '{teacherId}' is not a class teacher") { }
    }

    public class AttendanceStrengthMismatchException : Exception
    {
        public AttendanceStrengthMismatchException(int expected, int actual)
            : base($"Attendance list length '{actual}' does not match class strength '{expected}'") { }
    }

    public class InvalidAttendanceValueException : Exception
    {
        public InvalidAttendanceValueException()
            : base("Attendance list can only contain 0 (Absent) or 1 (Present)") { }
    }

    public class RollNumberNotAssignedException : Exception
    {
        public RollNumberNotAssignedException(string Class, string sec)
            : base($"Roll numbers not assigned for class '{Class}-{sec}'. Please assign roll numbers first") { }
    }
        public class MarkListStrengthMismatchException : Exception
    {
        public MarkListStrengthMismatchException(int expected, int actual)
            : base($"Marks list length '{actual}' does not match class strength '{expected}'") { }
    }

    public class MarksOutOfRangeException : Exception
    {
        public MarksOutOfRangeException(decimal mark, decimal totalMarks)
            : base($"Mark '{mark}' is invalid. Must be -1 (absent) or between 0 and {totalMarks}") { }
    }

    public class MarksAlreadyEnteredForExamException : Exception
    {
        public MarksAlreadyEnteredForExamException(string examName, string subject, string Class, string sec)
            : base($"Marks for exam '{examName}' subject '{subject}' in class '{Class}-{sec}' already entered") { }
    }

    public class AdminAttendanceAlreadyMarkedException : Exception
    {
        public AdminAttendanceAlreadyMarkedException(string adminId, DateOnly date)
            : base($"Attendance for admin '{adminId}' on '{date}' is already marked") { }
    }
}