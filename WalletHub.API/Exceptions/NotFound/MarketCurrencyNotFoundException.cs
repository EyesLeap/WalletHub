using System.Net;

namespace WalletHub.API.Exceptions
{
    public class MarketCurrencyNotFoundException : WalletHubException
    {
        public MarketCurrencyNotFoundException(string symbol)
            : base($"MarketCurrency with symbol {symbol} was not found.", HttpStatusCode.NotFound)
        {
        }
    }

}