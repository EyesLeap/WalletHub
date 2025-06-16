using WalletHub.API.Dtos.Account;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IAccountService
    {
        Task<AppUser> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> RegisterWithConfirmationAsync(RegisterDto registerDto, string confirmationLink);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> SendConfirmationEmailAsync(AppUser appUser, string confirmationLink);
        Task<string> ConfirmEmailAsync(string userId, string token);
        Task<string> ResendConfirmationEmailAsync(string email, string confirmationLink);
    }
}