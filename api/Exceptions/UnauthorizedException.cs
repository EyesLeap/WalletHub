using System.Net;

namespace api.Exceptions
{
    public class UnauthorizedException : WalletHubException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }


}