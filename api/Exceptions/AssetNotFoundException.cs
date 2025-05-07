using System.Net;

namespace api.Exceptions
{
    public class AssetNotFoundException : WalletHubException
    {
        public AssetNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }




}