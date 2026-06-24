using SchoolERP.Models.Enums;

namespace SchoolERP.Models.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Username { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public string Token { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        public string Username { get; set; } = string.Empty;

        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }

    
    public class ChangeMyPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }

}