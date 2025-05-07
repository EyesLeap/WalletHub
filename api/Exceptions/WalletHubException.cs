using System.Net;

namespace api.Exceptions
{
    public class WalletHubException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public WalletHubException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}