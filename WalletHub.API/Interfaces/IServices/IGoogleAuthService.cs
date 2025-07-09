using WalletHub.API.Dtos.Account;
using WalletHub.API.Dtos.Account.Google;
using WalletHub.API.Dtos.AuthToken;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<AuthResponseDto> GoogleLoginAsync(GoogleOAuthDto googleOAuthDto);
        Task<GoogleUserInfo> ValidateGoogleTokenAsync(string idToken);
        Task<AppUser> FindOrCreateGoogleUserAsync(GoogleUserInfo googleUserInfo);
    }
}