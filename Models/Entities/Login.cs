using System.ComponentModel.DataAnnotations;
using SchoolERP.Models.Enums;

namespace SchoolERP.Models.Entities
{
    public class Login
    {
        [Key]
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;
        public string Email { get; set; }                   
        public string? ResetCode { get; set; }               
        public DateTime? ResetCodeExpiry { get; set; } 
        public Admin? Admin { get; set; }
        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
    }
}
