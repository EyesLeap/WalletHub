using System.Net;

namespace WalletHub.API.Exceptions.NotFound
{
    public class NotFoundException : WalletHubException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) { }
    }
}