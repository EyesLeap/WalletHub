using System.Net;

namespace WalletHub.API.Exceptions
{
    public class AssetNotFoundException : WalletHubException
    {
        public AssetNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }




}