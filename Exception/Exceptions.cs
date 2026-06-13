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
    }

    public class InvalidAttendanceStatusException : Exception
    {
        public InvalidAttendanceStatusException()
            : base("Invalid attendance status. Use 0 for Absent and 1 for Present") { }
    }
}