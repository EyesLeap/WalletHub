using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}