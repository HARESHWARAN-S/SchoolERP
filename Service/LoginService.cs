using Microsoft.IdentityModel.Tokens;
using SchoolERP.Exceptions;
using SchoolERP.Models.DTOs;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;
using SchoolERP.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SchoolERP.Models.Entities;
using System.Text;

namespace SchoolERP.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepo;
        private readonly ILogRepository _logRepo;
        private readonly IConfiguration _config;
        private readonly ITokenBlacklistRepository _blacklistRepo;
        private readonly IEmailService _emailService;

        public LoginService(
            ILoginRepository loginRepo,
            ILogRepository logRepo,
            ITokenBlacklistRepository blacklistRepo,
            IConfiguration config,
            IEmailService emailService)
        {
            _loginRepo = loginRepo;
            _logRepo = logRepo;
            _blacklistRepo = blacklistRepo;
            _config = config;
            _emailService = emailService;
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

        public async Task LogoutAsync(string username, string token)
        {
            var login = await _loginRepo.GetByUsernameAsync(username);
            if (login == null)
                throw new UserNotFoundException(username);

            await _blacklistRepo.AddAsync(new BlacklistedToken
            {
                Token = token,
                Username = username,
                BlacklistedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8) 
            });

            await _logRepo.AddAsync($"User '{username}' logged out");
        }

        public async Task ChangeMyPasswordAsync(string username, string newPassword)
        {
            var login = await _loginRepo.GetByUsernameAsync(username);
            if (login == null)
                throw new UserNotFoundException(username);

            // Check new password is not same as current
            if (BCrypt.Net.BCrypt.Verify(newPassword, login.PasswordHash))
                throw new SamePasswordException();

            login.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _loginRepo.UpdateAsync(login);

            await _logRepo.AddAsync($"User '{username}' changed their password");
        }
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var login = await _loginRepo.GetByUsernameAsync(dto.Username);
            if (login == null)
                throw new UserNotFoundException(dto.Username);

            if (login.Status == UserStatus.Inactive)
                throw new UserInactiveException(dto.Username);

            if (string.IsNullOrEmpty(login.Email))
                throw new EmailNotRegisteredException(dto.Username);

            // Generate 8-character alphanumeric code
            string code = GenerateResetCode();

            // Store code + 5 min expiry in Login table
            await _loginRepo.UpdateResetCodeAsync(
                dto.Username,
                code,
                DateTime.UtcNow.AddMinutes(5));

            // Send email
            await _emailService.SendResetCodeAsync(login.Email, code, dto.Username);

            await _logRepo.AddAsync(
                $"Password reset code sent to email for user '{dto.Username}'");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var login = await _loginRepo.GetByUsernameAsync(dto.Username);
            if (login == null)
                throw new UserNotFoundException(dto.Username);

            // Check code exists
            if (string.IsNullOrEmpty(login.ResetCode))
                throw new InvalidResetCodeException();

            // Check expiry first — if expired, clear code and throw
            if (login.ResetCodeExpiry == null || DateTime.UtcNow > login.ResetCodeExpiry)
            {
                // Clear expired code
                await _loginRepo.UpdateResetCodeAsync(dto.Username, null, null);
                throw new ResetCodeExpiredException();
            }

            // Check code matches
            if (login.ResetCode != dto.Code)
                throw new InvalidResetCodeException();

            // Check new password not same as current
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, login.PasswordHash))
                throw new SamePasswordException();

            // Update password and clear reset code
            login.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _loginRepo.UpdateAsync(login);
            await _loginRepo.UpdateResetCodeAsync(dto.Username, null, null);

            await _logRepo.AddAsync($"User '{dto.Username}' reset their password");
        }

        private string GenerateResetCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}