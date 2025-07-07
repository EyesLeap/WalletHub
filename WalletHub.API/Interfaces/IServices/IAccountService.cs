using WalletHub.API.Dtos.Account;
using WalletHub.API.Dtos.AuthToken;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IAccountService
    {
        Task<AppUser> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterWithConfirmationAsync(RegisterDto registerDto, string confirmationLink);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        Task SendConfirmationEmailAsync(AppUser appUser, string confirmationLink);
        Task ResendConfirmationEmailAsync(string email, string confirmationLink);
        Task<bool> ConfirmEmailAsync(string userId, string token);

        Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> RevokeRefreshTokenAsync(string userId);
    }
}