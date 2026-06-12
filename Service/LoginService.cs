using Microsoft.IdentityModel.Tokens;
using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolERP.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepo;
        private readonly ILogRepository _logRepo;
        private readonly IConfiguration _config;

        public LoginService(
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            IConfiguration config)
        {
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _config = config;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var login = await _loginRepo.GetByUsernameAsync(dto.Username);

            if (login == null)
                throw new InvalidCredentialsException();

            if (login.Status == UserStatus.Inactive)
                throw new UserInactiveException(dto.Username);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, login.PasswordHash))
                throw new InvalidCredentialsException();

            await _logRepo.AddAsync($"User '{dto.Username}' logged in successfully");

            return new LoginResponseDto
            {
                Username = login.Username,
                Role = login.Role,
                Token = GenerateToken(login.Username, login.Role.ToString())
            };
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var login = await _loginRepo.GetByUsernameAsync(dto.Username);

            if (login == null)
                throw new UserNotFoundException(dto.Username);

            if (login.Status == UserStatus.Inactive)
                throw new UserInactiveException(dto.Username);

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, login.PasswordHash))
                throw new IncorrectPasswordException();

            if (dto.OldPassword == dto.NewPassword)
                throw new SamePasswordException();

            login.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _loginRepo.UpdateAsync(login);

            await _logRepo.AddAsync($"User '{dto.Username}' changed their password");
        }

        private string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}