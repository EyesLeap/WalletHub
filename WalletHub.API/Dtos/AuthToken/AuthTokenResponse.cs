using System.ComponentModel.DataAnnotations;
using System.Transactions;
using WalletHub.API.Helpers;
using Microsoft.EntityFrameworkCore;
using WalletHub.API.Dtos.TransactionDtos;

namespace WalletHub.API.Dtos.AuthToken
{
    public class AuthTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}