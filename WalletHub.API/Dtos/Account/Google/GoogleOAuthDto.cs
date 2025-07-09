using System.ComponentModel.DataAnnotations;
using WalletHub.API.Dtos.AuthToken;

namespace WalletHub.API.Dtos.Account.Google
{
    public class GoogleUserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
    }
}