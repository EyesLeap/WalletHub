using System.Net;

namespace api.Exceptions
{
    public class TransactionNotFoundException : WalletHubException
    {
        public TransactionNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) 
        {
        }
    }




}