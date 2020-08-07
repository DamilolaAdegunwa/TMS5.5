using Egoal.Application.Services.Dto;
using Egoal.Members.Dto;
using Egoal.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Members
{
    public interface IMemberAppService
    {
        Task<LoginOutput> LoginFromWechatOffiaccountAsync(WechatOffiaccountLoginInput input);

        /// <summary>
        /// 小程序登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<LoginOutput> LoginFromWechatMiniProgramAsync(WechatMiniProgramLoginInput input);

        Task<LoginOutput> BindStaffAsync(BindStaffInput input);
        Task<LoginOutput> BuildLoginOutputAsync(Member member, string openId, Guid? customerId = null);
        Task RegistWechatMemberAsync(RegistWechatMemberInput input);
        Task RegistMemberCardAsync(RegistMemberCardInput input, Guid memberId);
        Task<MemberCardDto> GetElectronicMemberCardAsync(Guid memberId);
        Task RenewMemberCardAsync(int id);
        Task<List<ComboboxItemDto<Guid>>> GetMemberComboboxItemsAsync();
        Task<string> BindSendVerificationCodeAsync(string mobile);
        Task<string> RegistSendVerificationCodeAsync(string mobile);
        Task<LoginOutput> BindFromWeChatAsync(BindFromWeChatInput input);
        Task<LoginOutput> RegistFromWeChatAsync(RegistMemberInput input);
        Task<PagedResultDto<MemberTicketSaleDto>> GetVipTicketsForMobileAsync(GetMemberTicketsInput input);
    }
}
