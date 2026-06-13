using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class BlacklistedToken
    {
        [Key]
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}