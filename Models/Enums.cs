namespace SchoolERP.Models.Enums
{
    public enum UserRole 
    { 
        Admin, 
        Teacher, 
        Student 
    }

    public enum UserStatus 
    { 
        Active, 
        Inactive 
    }

    public enum Gender 
    { 
        Male, 
        Female, 
        Other 
    }

    public enum BloodGroup
    {
        APositive, 
        ANegative,
        BPositive, 
        BNegative,
        ABPositive, 
        ABNegative,
        OPositive, 
        ONegative
    }
    public enum NotificationTarget
    {
        All,
        Student,
        Teacher
    }
    public enum AttendanceStatus
    {
        Absent = 0,
        Present = 1
    }
    public enum FeeStatus
    {
        Unpaid,
        Paid
    }
}
