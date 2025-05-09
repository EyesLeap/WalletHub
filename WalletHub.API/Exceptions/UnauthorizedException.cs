using System.Net;

namespace WalletHub.API.Exceptions
{
    public class UnauthorizedException : WalletHubException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }


}