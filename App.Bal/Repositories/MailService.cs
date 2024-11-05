using App.Bal.Services;
using App.Entity.Config;
using App.Entity.Dto;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Mandrill;
using Microsoft.Extensions.Configuration;
using App.Entity.Dto.Custom;

namespace App.Bal.Repositories
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly MailConfig _mailConfig;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailConfig = new MailConfig();
            _configuration.GetSection(MailConfig.Path).Bind(_mailConfig);
        }

        public async Task SendContractInviteEmail(EmailDto dto)
        {
            string path = string.Empty;
            string fileContent = string.Empty;
            MandrillApi mandrillApi = new(_mailConfig.ApiKey);

            foreach (UserCustomDto customDto in dto.Emails)
            {
                path = Path.Combine(Environment.CurrentDirectory, "Template", "eventInvite.html");
                fileContent = File.ReadAllText(path);
                fileContent = fileContent.Replace("USERNAME", $"{customDto.FirstName} {customDto.LastName}");
                fileContent = fileContent.Replace("PROPERTYNAME", dto.PropertName);
                fileContent = fileContent.Replace("CONTRACTNUMBER", dto.ContractNnumber);
                fileContent = fileContent.Replace("EVENTNAME", dto.EventName);
                EmailMessage emailMessage = new()
                {
                    FromEmail = _mailConfig.Email,
                    FromName = _mailConfig.FromName,
                    To = [new EmailAddress(customDto.Email)],
                    Subject = dto.Subject,
                    Html = fileContent
                };
                List<EmailResult> emailResults = await mandrillApi.SendMessage(new SendMessageRequest(emailMessage));
            }
        }

        public async Task SendEmailOtp(EmailDto dto)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Template", "email_otp.html");
            string fileContent = File.ReadAllText(path);

            fileContent = fileContent.Replace("USERNAME", dto.UserName);
            fileContent = fileContent.Replace("OTP", dto.OTP);

            MandrillApi mandrillApi = new(_mailConfig.ApiKey);

            List<EmailAddress> toEmail =
            [
                new EmailAddress(dto.Email)
            ];

            EmailMessage emailMessage = new()
            {
                FromEmail = _mailConfig.Email,
                FromName = _mailConfig.FromName,
                To = toEmail,
                Subject = "VeriBuild Email Login",
                Html = fileContent
            };

            List<EmailResult> emailResults = await mandrillApi.SendMessage(new SendMessageRequest(emailMessage));
        }

        public async Task SendInvite(EmailDto dto)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Template", "userinvite.html");
            string fileContent = File.ReadAllText(path);

            fileContent = fileContent.Replace("USERNAME", dto.UserName);
            fileContent = fileContent.Replace("LINK", dto.Link);

            MandrillApi mandrillApi = new(_mailConfig.ApiKey);

            List<EmailAddress> toEmail = new()
            {
                new EmailAddress(dto.Email)
            };

            EmailMessage emailMessage = new()
            {
                FromEmail = _mailConfig.Email,
                FromName = _mailConfig.FromName,
                To = toEmail,
                Subject = "VeriBuild User Invite",
                Html = fileContent
            };

            List<EmailResult> emailResults = await mandrillApi.SendMessage(new SendMessageRequest(emailMessage));
        }

        public async Task SendPasswordRecoveryMail(EmailDto dto)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Template", "forgot_password.html");
            string fileContent = File.ReadAllText(path);

            fileContent = fileContent.Replace("FULLNAME", dto.UserName);
            fileContent = fileContent.Replace("VERIFYLINK", dto.Link);

            MandrillApi mandrillApi = new(_mailConfig.ApiKey);

            List<EmailAddress> toEmail =
            [
                new EmailAddress(dto.Email)
            ];

            EmailMessage emailMessage = new()
            {
                FromEmail = _mailConfig.Email,
                FromName = _mailConfig.FromName,
                To = toEmail,
                Subject = "VeriBuild Password Reset Mail",
                Html = fileContent
            };

            List<EmailResult> emailResults = await mandrillApi.SendMessage(new SendMessageRequest(emailMessage));
        }


    }
}
