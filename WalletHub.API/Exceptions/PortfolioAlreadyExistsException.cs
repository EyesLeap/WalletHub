using System.Net;

namespace WalletHub.API.Exceptions
{
    public class PortfolioAlreadyExistsException : WalletHubException
    {
        public PortfolioAlreadyExistsException(string message = "Portfolio already exists.")
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }


} 