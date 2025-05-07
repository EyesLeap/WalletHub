using System.Net;

namespace api.Exceptions
{
    public class PortfolioAlreadyExistsException : WalletHubException
    {
        public PortfolioAlreadyExistsException(string message = "Portfolio already exists.")
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }


} 