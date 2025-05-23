using System.Net;

namespace WalletHub.API.Exceptions
{
    public class UserNotFoundException : WalletHubException
    {
         public UserNotFoundException(string identifier)
        : base($"User with identifier '{identifier}' was not found.", HttpStatusCode.NotFound)
        {
        }
    }



}