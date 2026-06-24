using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task ChangePasswordAsync(ChangePasswordDto dto);
        Task LogoutAsync(string username, string token); 
        Task ChangeMyPasswordAsync(string username, string newPassword);
    }
}