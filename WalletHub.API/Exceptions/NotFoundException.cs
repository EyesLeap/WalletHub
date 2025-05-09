using System.Net;

namespace WalletHub.API.Exceptions
{
    public class NotFoundException : WalletHubException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) { }
    }
}