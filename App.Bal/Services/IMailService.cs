using App.Entity.Dto;

namespace App.Bal.Services
{
    public interface IMailService
    {
        public Task SendInvite(EmailDto dto);
        public Task SendEmailOtp(EmailDto dto);
        public Task SendPasswordRecoveryMail(EmailDto dto);
        public Task SendContractInviteEmail(EmailDto dto);

    }
}
