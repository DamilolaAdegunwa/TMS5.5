using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Messages
{
    public interface IEmailAppService
    {
        Task SendEmailVerificationCodeAsync();
        Task SendVerificationCodeAsync(string address, string code);
    }
}
