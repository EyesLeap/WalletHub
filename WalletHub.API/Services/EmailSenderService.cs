using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;

namespace WalletHub.API.Services;

public class AuthMessageSenderOptions
{
    public string SendGridKey { get; set; }
}

public class EmailSenderService : IEmailSenderService
{

    private readonly ILogger<EmailSenderService> _logger;
    public AuthMessageSenderOptions Options { get; }

    public EmailSenderService(IOptions<AuthMessageSenderOptions> optionsAccessor,
                        ILogger<EmailSenderService> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
            throw new WalletHubException("SendGrid API key is not configured.");

        await Execute(Options.SendGridKey, subject, message, toEmail);
    }
    
    private async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("wallethubcrypto@gmail.com", "WalletHub"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        msg.SetClickTracking(false, false);

        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation($"Email to {toEmail} sent successfully.");
        else
            _logger.LogWarning($"Failed to send email to {toEmail}. Status Code: {response.StatusCode}");
    }
}



