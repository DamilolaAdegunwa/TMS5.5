using Egoal.Application.Services;
using Egoal.Cryptography;
using Egoal.Net.Mail;
using System.Threading.Tasks;

namespace Egoal.Messages
{
    public class EmailAppService : ApplicationService, IEmailAppService
    {
        private readonly IEmailSender _emailSender;

        public EmailAppService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendEmailVerificationCodeAsync()
        {
            await _emailSender.SendAsync("741440696@qq.com", "邮箱验证码", $"你的验证码：{RandomHelper.CreateRandomNumber()}", false);
        }

        public async Task SendVerificationCodeAsync(string address, string code)
        {
            await _emailSender.SendAsync(address, "验证码", $"您的验证码是：{code}。请不要把验证码泄露给其他人。", false);
        }
    }
}
