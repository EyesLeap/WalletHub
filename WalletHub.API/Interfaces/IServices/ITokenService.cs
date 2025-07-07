using System.Security.Claims;
using WalletHub.API.Dtos.AuthToken;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<AuthTokenResponse> CreateTokenResponse(AppUser user);
    }
}