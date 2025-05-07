using System.Net;

namespace api.Exceptions
{
    public class NotFoundException : WalletHubException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) { }
    }
}