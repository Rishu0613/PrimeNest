using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNest.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration configuration)
        {
                _config = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _config["Authentication:SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is missing or not configured correctly.");
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(
                _config["Authentication:SendGrid:FromEmail"],
                _config["Authentication:SendGrid:FromName"]
            );
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response body: {await response.Body.ReadAsStringAsync()}");
        }

    }
}
