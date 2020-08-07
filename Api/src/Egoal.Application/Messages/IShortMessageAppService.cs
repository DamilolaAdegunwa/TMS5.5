using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Messages
{
    public interface IShortMessageAppService
    {
        Task SendVerificationCodeAsync(string mobile, string code);
        Task SendRefundMessageAsync(string mobile, string travelDate, string reason);
    }
}
