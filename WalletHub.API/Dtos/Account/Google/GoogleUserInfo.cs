using WalletHub.API.Dtos.AuthToken;

namespace WalletHub.API.Dtos.Account.Google
{
    public class GoogleOAuthDto
    {
        public string IdToken { get; set; } = string.Empty;

        public string? AccessToken { get; set; }
    }
}