using System.Net;

namespace api.Exceptions
{
    public class PortfolioNotFoundException : WalletHubException
    {
        public PortfolioNotFoundException(int portfolioId)
            : base($"Portfolio with ID {portfolioId} was not found.", HttpStatusCode.NotFound)
        {
        }
    }

}