using System.Net;

namespace WalletHub.API.Exceptions
{
    public class TransactionNotFoundException : WalletHubException
    {
        public TransactionNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) 
        {
        }
    }




}