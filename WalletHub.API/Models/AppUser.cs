using Microsoft.AspNetCore.Identity;

namespace WalletHub.API.Models
{
    public class AppUser : IdentityUser
    {
        public List<Portfolio> Portfolios {get; set;} = new List<Portfolio>();

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string? GoogleId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Provider { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}