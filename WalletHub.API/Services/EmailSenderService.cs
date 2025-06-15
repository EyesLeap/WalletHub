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
    public string SendGridTemplateId { get; set; }
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
    public async Task SendEmailAsync(string toEmail, string subject, string username, string confirmationLink)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
            throw new WalletHubException("SendGrid API key is not configured.");
        if (string.IsNullOrEmpty(Options.SendGridTemplateId))
            throw new WalletHubException("SendGridTemplateId is not configured.");

        await Execute(subject, toEmail, username, confirmationLink);
    }
    
    private async Task Execute(string subject, string toEmail, string username, string confirmationLink)
    {
        var client = new SendGridClient(Options.SendGridKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("wallethubcrypto@gmail.com", "WalletHub"),
            Subject = subject,
            TemplateId = Options.SendGridTemplateId

        };
        msg.AddTo(new EmailAddress(toEmail));

        msg.SetClickTracking(false, false);

        msg.SetTemplateData(new
        {   
            name = username,
            confirmation_link = confirmationLink
        });

        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation($"Email to {toEmail} sent successfully.");
        else
            _logger.LogWarning($"Failed to send email to {toEmail}. Status Code: {response.StatusCode}");
    }
}



