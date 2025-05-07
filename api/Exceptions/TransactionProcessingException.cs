using System.Net;

namespace api.Exceptions
{
    public class TransactionProcessingException : WalletHubException
    {
        public TransactionProcessingException(string message)
            : base(message, HttpStatusCode.BadRequest) 
        {
        }

        public TransactionProcessingException(string message, HttpStatusCode statusCode)
            : base(message, statusCode)
        {
        }
    }




}