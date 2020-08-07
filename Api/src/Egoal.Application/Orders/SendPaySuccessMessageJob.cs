using Egoal.BackgroundJobs;
using Egoal.Dependency;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Orders.Dto;
using Egoal.WeChat;
using Egoal.WeChat.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class SendPaySuccessMessageJob : IBackgroundJob, IScopedDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WeChatOptions _weChatOptions;
        private readonly MessageService _messageService;
        private readonly IRepository<UserWechat> _userRepository;

        public SendPaySuccessMessageJob(
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<WeChatOptions> weChatOptions,
            MessageService messageService,
            IRepository<UserWechat> userRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _weChatOptions = weChatOptions.Value;
            _messageService = messageService;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(string args, CancellationToken stoppingToken)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var message = args.JsonToObject<PaySuccessMessage>();

                var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == message.MemberId);
                if (user.OffiaccountOpenId.IsNullOrEmpty()) return;

                var detailUrl = _weChatOptions.WxSaleUrl.UrlCombine($"/Login?redirect=orderdetail/{message.ListNo}");
                var url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={_weChatOptions.WxAppID}&redirect_uri={detailUrl.UrlEncode()}&response_type=code&scope=snsapi_userinfo&state=123#wechat_redirect";

                await _messageService.SendPaySuccessMessageAsync(user.OffiaccountOpenId, message.ListNo, message.TotalMoney.ToString(), message.ProductInfo, url);

                await uow.CompleteAsync();
            }
        }
    }
}
