using WalletHub.API.Dtos.AuthToken;

namespace WalletHub.API.Dtos.Account
{
    public class AuthResponseDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public AuthTokenResponse? TokenResponse { get; set; }
    }
}