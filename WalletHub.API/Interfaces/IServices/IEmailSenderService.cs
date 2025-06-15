using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WalletHub.API.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string toEmail, string subject, string username, string confirmationLink);
}