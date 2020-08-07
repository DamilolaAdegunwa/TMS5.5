using Egoal.Application.Services;
using Egoal.Auditing;
using Egoal.BackgroundJobs;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.DynamicCodes;
using Egoal.Events.Bus;
using Egoal.Extensions;
using Egoal.Logging;
using Egoal.Members;
using Egoal.Payment.Dto;
using Egoal.Staffs;
using Egoal.Threading.Lock;
using Egoal.Trades;
using Egoal.UI;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Egoal.Payment
{
    public class PayAppService : ApplicationService, IPayAppService
    {
        private readonly ILogger _logger;
        private readonly PayOptions _payOptions;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDistributedLockFactory _lockFactory;
        private readonly INetPayOrderRepository _netPayOrderRepository;
        private readonly IRepository<RefundMoneyApply, long> _refundMoneyApplyRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly IRepository<NetPayRefundFail, Guid> _netPayRefundFailRepository;
        private readonly IRepository<Staff> _staffRepository;
        private readonly IRepository<UserWechat> _userRepository;
        private readonly NetPayServiceFactory _netPayServiceFactory;
        private readonly IBackgroundJobService _backgroundJobAppService;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly IClientInfoProvider _clientInfoProvider;

        public PayAppService(
            ILogger<PayAppService> logger,
            IOptions<PayOptions> options,
            IEventBus eventBus,
            IUnitOfWorkManager unitOfWorkManager,
            IDistributedLockFactory lockFactory,
            INetPayOrderRepository netPayOrderRepository,
            IRepository<RefundMoneyApply, long> refundMoneyApplyRepository,
            ITradeRepository tradeRepository,
            IRepository<NetPayRefundFail, Guid> netPayRefundFailRepository,
            IRepository<Staff> staffRepository,
            IRepository<UserWechat> userRepository,
            NetPayServiceFactory netPayServiceFactory,
            IBackgroundJobService backgroundJobAppService,
            IDynamicCodeService dynamicCodeService,
            IClientInfoProvider clientInfoProvider)
        {
            _logger = logger;
            _payOptions = options.Value;
            _eventBus = eventBus;
            _unitOfWorkManager = unitOfWorkManager;
            _lockFactory = lockFactory;
            _netPayOrderRepository = netPayOrderRepository;
            _refundMoneyApplyRepository = refundMoneyApplyRepository;
            _tradeRepository = tradeRepository;
            _netPayRefundFailRepository = netPayRefundFailRepository;
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            _netPayServiceFactory = netPayServiceFactory;
            _backgroundJobAppService = backgroundJobAppService;
            _dynamicCodeService = dynamicCodeService;
            _clientInfoProvider = clientInfoProvider;
        }

        public async Task PrePayAsync(PrePayInput payInput)
        {
            var netPayOrder = new NetPayOrder();
            netPayOrder.ListNo = payInput.ListNo;
            netPayOrder.TotalFee = payInput.PayMoney;
            netPayOrder.OrderStatusId = NetPayOrderStatus.未支付;
            netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
            netPayOrder.UserId = payInput.UserId;

            var payCommand = payInput.Adapt<PayCommandBase>();
            payCommand.PayStartTime = netPayOrder.Ctime;
            payCommand.PayExpireTime = netPayOrder.GetExpireTime(_payOptions.OrderExpireMinutes);
            netPayOrder.PayArgs = payCommand.ToJson();

            await _netPayOrderRepository.InsertAsync(netPayOrder);

            var jobArgs = new ConfirmPayStatusJobArgs();
            jobArgs.ListNo = payInput.ListNo;
            jobArgs.Attach = payInput.Attach;
            var delay = TimeSpan.FromMinutes(_payOptions.OrderExpireMinutes).Subtract(TimeSpan.FromSeconds(10));
            await _backgroundJobAppService.EnqueueAsync<ConfirmPayStatusJob>(jobArgs.ToJson(), delay: delay);
        }

        public async Task<PayOutput> MicroPayAsync(MicroPayInput payInput)
        {
            using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(payInput.ListNo)))
            {
                if (!locker.IsAcquired)
                {
                    throw new UserFriendlyException($"订单{payInput.ListNo}锁定失败");
                }

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == payInput.ListNo);
                    if (netPayOrder == null)
                    {
                        throw new UserFriendlyException($"订单{payInput.ListNo}不存在");
                    }

                    if (!netPayOrder.AllowPay())
                    {
                        throw new UserFriendlyException($"订单{netPayOrder.OrderStatusName}");
                    }

                    var payTypeId = payInput.PayTypeId;
                    var payCommand = netPayOrder.PayArgs.JsonToObject<MicroPayCommand>();
                    payCommand.AuthCode = payInput.AuthCode;
                    payCommand.OnlinePayTradeType = OnlinePayTradeType.MICROPAY;
                    payCommand.SubPayTypeId = GetPayTypeFromAuthCode(payInput.AuthCode);
                    payCommand.ClientIp = _clientInfoProvider.ClientIpAddress;

                    using (var subUow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        var netPayType = netPayOrder.GetMicroNetPayType(payTypeId);
                        await _netPayOrderRepository.SetPayTypeAsync(netPayOrder.Id, payTypeId, payCommand.SubPayTypeId, OnlinePayTradeType.MICROPAY, netPayType, netPayType.ToString());

                        var args = new QueryNetPayJobArgs();
                        args.ListNo = netPayOrder.ListNo;
                        args.PayTypeId = payTypeId;
                        args.OnlinePayTradeType = OnlinePayTradeType.MICROPAY;
                        args.NetPayTypeId = netPayType;
                        args.Attach = payCommand.Attach;
                        args.IntervalSecond = 5;
                        await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(args.ToJson(), delay: TimeSpan.FromSeconds(args.IntervalSecond));

                        await subUow.CompleteAsync();
                    }

                    var payService = _netPayServiceFactory.GetPayService(payTypeId);

                    var result = await payService.MicroPayAsync(payCommand);
                    if (result.IsPaid)
                    {
                        netPayOrder.TransactionId = result.TransactionId;
                        netPayOrder.SubTransactionId = result.SubTransactionId;
                        netPayOrder.OrderStatusId = NetPayOrderStatus.支付成功;
                        netPayOrder.PayTime = result.PayTime;
                        netPayOrder.BankType = result.BankType;
                        netPayOrder.ClearPayArgs();
                        netPayOrder.ErrorCode = null;

                        var eventData = new PaySuccessEventData();
                        eventData.ListNo = netPayOrder.ListNo;
                        eventData.PayTypeId = payTypeId;
                        eventData.Attach = payCommand.Attach;
                        await _eventBus.TriggerAsync(eventData);
                    }
                    else if (result.IsPaying)
                    {
                        netPayOrder.OrderStatusId = NetPayOrderStatus.用户支付中;
                    }
                    else
                    {
                        netPayOrder.OrderStatusId = NetPayOrderStatus.支付失败;
                        netPayOrder.ErrorCode = result.ErrorMessage;
                    }
                    netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
                    netPayOrder.Mtime = DateTime.Now;

                    await uow.CompleteAsync();

                    var output = new PayOutput();
                    output.Success = result.IsPaid;
                    output.IsPaying = result.IsPaying;
                    output.Message = result.ErrorMessage;

                    return output;
                }
            }
        }

        private int GetPayTypeFromAuthCode(string authCode)
        {
            int code = authCode.Substring(0, 2).To<int>();
            if (code >= 10 && code <= 15)
            {
                return DefaultPayType.微信支付;
            }
            if (code >= 25 && code <= 30)
            {
                return DefaultPayType.支付宝付款;
            }

            throw new NotSupportedException("支付授权码无效");
        }

        public async Task<string> OffiaccountPayAsync(string listNo)
        {
            return await JsApiPayAsync(listNo, OnlinePayTradeType.OFFIACCOUNT);
        }

        public async Task<string> MiniprogramPayAsync(string listNo)
        {
            return await JsApiPayAsync(listNo, OnlinePayTradeType.MINIPROGRAM);
        }

        private async Task<string> JsApiPayAsync(string listNo, OnlinePayTradeType onlinePayTradeType)
        {
            var netPayOrder = await _netPayOrderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(o => o.ListNo == listNo);
            if (netPayOrder == null)
            {
                throw new UserFriendlyException($"订单{listNo}不存在");
            }

            if (!netPayOrder.AllowPay())
            {
                throw new UserFriendlyException($"订单{netPayOrder.OrderStatusName}");
            }

            if (!netPayOrder.JsApiPayArgs.IsNullOrEmpty())
            {
                return netPayOrder.JsApiPayArgs;
            }

            var payTypeId = _payOptions.WxSalePayTypeId;
            var payCommand = netPayOrder.PayArgs.JsonToObject<JsApiPayCommand>();
            payCommand.OnlinePayTradeType = onlinePayTradeType;
            payCommand.SubPayTypeId = DefaultPayType.微信支付;
            payCommand.OpenId = await GetOpenIdAsync(netPayOrder.UserId, onlinePayTradeType);
            payCommand.ClientIp = _clientInfoProvider.ClientIpAddress;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var netPayType = netPayOrder.GetJsApiNetPayType(payTypeId);
                await _netPayOrderRepository.SetPayTypeAsync(netPayOrder.Id, payTypeId, DefaultPayType.微信支付, onlinePayTradeType, netPayType, netPayType.ToString());

                var args = new QueryNetPayJobArgs();
                args.ListNo = netPayOrder.ListNo;
                args.PayTypeId = payTypeId;
                args.OnlinePayTradeType = onlinePayTradeType;
                args.NetPayTypeId = netPayType;
                args.Attach = payCommand.Attach;
                args.IntervalSecond = 15;
                await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(args.ToJson(), delay: TimeSpan.FromSeconds(args.IntervalSecond));

                await uow.CompleteAsync();
            }

            var payService = _netPayServiceFactory.GetPayService(payTypeId);

            var parameters = await payService.JsApiPayAsync(payCommand);
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _netPayOrderRepository.SetJsApiPayArgsAsync(netPayOrder.Id, parameters);

                await uow.CompleteAsync();
            }

            return parameters;
        }

        private async Task<string> GetOpenIdAsync(string userId, OnlinePayTradeType tradeType)
        {
            if (userId.IsNullOrEmpty())
            {
                throw new UserFriendlyException("获取用户信息失败,UserId为空");
            }
            var memberId = Guid.Parse(userId);
            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == memberId);
            if (user == null)
            {
                throw new UserFriendlyException($"获取用户信息失败,{memberId}");
            }

            return tradeType == OnlinePayTradeType.MINIPROGRAM ? user.MiniProgramOpenId : user.OffiaccountOpenId;
        }

        public async Task<string> NativePayAsync(NativePayInput input)
        {
            var netPayOrder = await _netPayOrderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(o => o.ListNo == input.ListNo);
            if (netPayOrder == null)
            {
                throw new UserFriendlyException($"订单{input.ListNo}不存在");
            }

            if (!netPayOrder.AllowPay())
            {
                throw new UserFriendlyException($"订单{netPayOrder.OrderStatusName}");
            }

            var payTypeId = input.PayTypeId;
            var payCommand = netPayOrder.PayArgs.JsonToObject<NativePayCommand>();
            payCommand.OnlinePayTradeType = OnlinePayTradeType.NATIVE;
            payCommand.SubPayTypeId = input.SubPayTypeId ?? input.PayTypeId;
            payCommand.ClientIp = _clientInfoProvider.ClientIpAddress;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var netPayType = netPayOrder.GetNativeNetPayType(payTypeId);
                await _netPayOrderRepository.SetPayTypeAsync(netPayOrder.Id, payTypeId, payCommand.SubPayTypeId, OnlinePayTradeType.NATIVE, netPayType, netPayType.ToString());

                var args = new QueryNetPayJobArgs();
                args.ListNo = netPayOrder.ListNo;
                args.PayTypeId = payTypeId;
                args.OnlinePayTradeType = OnlinePayTradeType.NATIVE;
                args.NetPayTypeId = netPayType;
                args.Attach = payCommand.Attach;
                args.IntervalSecond = 15;
                await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(args.ToJson(), delay: TimeSpan.FromSeconds(args.IntervalSecond));

                await uow.CompleteAsync();
            }

            var payService = _netPayServiceFactory.GetPayService(payTypeId);

            var qrcode = await payService.NativePayAsync(payCommand);

            return qrcode;
        }

        public async Task<string> H5PayAsync(H5PayInput input)
        {
            var netPayOrder = await _netPayOrderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(o => o.ListNo == input.ListNo);
            if (netPayOrder == null)
            {
                throw new UserFriendlyException($"订单{input.ListNo}不存在");
            }

            if (!netPayOrder.AllowPay())
            {
                throw new UserFriendlyException($"订单{netPayOrder.OrderStatusName}");
            }

            var payTypeId = input.PayTypeId;
            var payCommand = netPayOrder.PayArgs.JsonToObject<H5PayCommand>();
            payCommand.OnlinePayTradeType = OnlinePayTradeType.MWEB;
            payCommand.SubPayTypeId = input.SubPayTypeId ?? input.PayTypeId;
            payCommand.WapUrl = input.ReturnUrl;
            payCommand.WapName = input.WapName;
            payCommand.ClientIp = _clientInfoProvider.ClientIpAddress;

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var netPayType = netPayOrder.GetNativeNetPayType(payTypeId);
                await _netPayOrderRepository.SetPayTypeAsync(netPayOrder.Id, payTypeId, payCommand.SubPayTypeId, OnlinePayTradeType.MWEB, netPayType, netPayType.ToString());

                var args = new QueryNetPayJobArgs();
                args.ListNo = netPayOrder.ListNo;
                args.PayTypeId = payTypeId;
                args.OnlinePayTradeType = OnlinePayTradeType.MWEB;
                args.NetPayTypeId = netPayType;
                args.Attach = payCommand.Attach;
                args.IntervalSecond = 15;

                await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(args.ToJson(), delay: TimeSpan.FromSeconds(args.IntervalSecond));

                await uow.CompleteAsync();
            }

            var payService = _netPayServiceFactory.GetPayService(payTypeId);

            var result = await payService.H5PayAsync(payCommand);

            return result;
        }

        public async Task<PayOutput> CashPayAsync(string listNo)
        {
            using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(listNo)))
            {
                if (!locker.IsAcquired)
                {
                    throw new UserFriendlyException($"订单{listNo}锁定失败");
                }

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == listNo);
                    if (netPayOrder == null)
                    {
                        throw new UserFriendlyException($"订单{listNo}不存在");
                    }

                    if (!netPayOrder.AllowPay())
                    {
                        throw new UserFriendlyException($"订单{netPayOrder.OrderStatusName}");
                    }

                    var payTypeId = DefaultPayType.现金;
                    var payCommand = netPayOrder.PayArgs.JsonToObject<PayCommandBase>();

                    netPayOrder.PayTypeId = payTypeId;
                    netPayOrder.NetPayTypeId = null;
                    netPayOrder.NetPayTypeName = null;
                    netPayOrder.OrderStatusId = NetPayOrderStatus.支付成功;
                    netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
                    netPayOrder.PayTime = DateTime.Now;
                    netPayOrder.ClearPayArgs();
                    netPayOrder.ErrorCode = null;
                    netPayOrder.Mtime = DateTime.Now;

                    var eventData = new PaySuccessEventData();
                    eventData.ListNo = netPayOrder.ListNo;
                    eventData.PayTypeId = payTypeId;
                    eventData.Attach = payCommand.Attach;
                    await _eventBus.TriggerAsync(eventData);

                    await uow.CompleteAsync();

                    return new PayOutput { Success = true };
                }
            }
        }

        public async Task LoopQueryNetPayAsync(QueryNetPayJobArgs input)
        {
            using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(input.ListNo)))
            {
                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    if (!locker.IsAcquired)
                    {
                        await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(input.ToJson(), delay: TimeSpan.FromSeconds(input.IntervalSecond));

                        await uow.CompleteAsync();

                        return;
                    }

                    var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == input.ListNo);
                    if (netPayOrder == null)
                    {
                        throw new TmsException($"订单{input.ListNo}不存在");
                    }

                    if (!netPayOrder.IsPayResultUnknown())
                    {
                        return;
                    }

                    var queryCommand = new QueryPayCommand();
                    queryCommand.ListNo = netPayOrder.ListNo;
                    queryCommand.TransactionId = netPayOrder.TransactionId;
                    queryCommand.OnlinePayTradeType = input.OnlinePayTradeType;
                    queryCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                    queryCommand.PayTime = netPayOrder.Ctime;
                    var payService = _netPayServiceFactory.GetPayService(input.PayTypeId);
                    var queryResult = await payService.QueryPayAsync(queryCommand);
                    if (queryResult.IsPaid)
                    {
                        netPayOrder.PayTypeId = input.PayTypeId;
                        netPayOrder.NetPayTypeId = input.NetPayTypeId;
                        netPayOrder.NetPayTypeName = netPayOrder.NetPayTypeId?.ToString();
                        netPayOrder.TransactionId = queryResult.TransactionId;
                        netPayOrder.SubTransactionId = queryResult.SubTransactionId;
                        netPayOrder.OrderStatusId = NetPayOrderStatus.支付成功;
                        netPayOrder.PayTime = queryResult.PayTime;
                        netPayOrder.BankType = queryResult.BankType;
                        netPayOrder.ClearPayArgs();
                        netPayOrder.ErrorCode = null;

                        var eventData = new PaySuccessEventData();
                        eventData.ListNo = netPayOrder.ListNo;
                        eventData.PayTypeId = input.PayTypeId;
                        eventData.Attach = input.Attach;
                        await _eventBus.TriggerAsync(eventData);
                    }
                    else if (netPayOrder.GetExpireTime(_payOptions.OrderExpireMinutes) > DateTime.Now)
                    {
                        netPayOrder.OrderStatusId = NetPayOrderStatus.用户支付中;

                        await _backgroundJobAppService.EnqueueAsync<QueryNetPayJob>(input.ToJson(), delay: TimeSpan.FromSeconds(input.IntervalSecond));
                    }
                    else
                    {
                        netPayOrder.OrderStatusId = NetPayOrderStatus.支付失败;
                        netPayOrder.ErrorCode = queryResult.ErrorMessage;
                    }
                    netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
                    netPayOrder.Mtime = DateTime.Now;

                    await uow.CompleteAsync();
                }
            }
        }

        public async Task ConfirmPayStatusAsync(ConfirmPayStatusJobArgs input)
        {
            using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(input.ListNo)))
            {
                if (!locker.IsAcquired)
                {
                    throw new TmsException($"订单{input.ListNo}锁定失败");
                }

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == input.ListNo);
                    if (netPayOrder == null)
                    {
                        throw new TmsException($"订单{input.ListNo}不存在");
                    }

                    if (netPayOrder.OrderStatusId.IsIn(NetPayOrderStatus.支付成功, NetPayOrderStatus.已关闭))
                    {
                        return;
                    }

                    if (!netPayOrder.IsNetPay() || netPayOrder.OrderStatusId == NetPayOrderStatus.支付失败)
                    {
                        var eventData = new PayTimeoutEventData();
                        eventData.ListNo = netPayOrder.ListNo;
                        eventData.Attach = input.Attach;
                        await _eventBus.TriggerAsync(eventData);

                        await uow.CompleteAsync();

                        return;
                    }

                    var queryCommand = new QueryPayCommand();
                    queryCommand.ListNo = netPayOrder.ListNo;
                    queryCommand.TransactionId = netPayOrder.TransactionId;
                    queryCommand.OnlinePayTradeType = netPayOrder.OnlinePayTradeType.Value;
                    queryCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                    queryCommand.PayTime = netPayOrder.Ctime;
                    var payService = _netPayServiceFactory.GetPayService(netPayOrder.PayTypeId.Value);
                    var queryResult = await payService.QueryPayAsync(queryCommand);
                    if (queryResult.IsPaid)
                    {
                        netPayOrder.TransactionId = queryResult.TransactionId;
                        netPayOrder.SubTransactionId = queryResult.SubTransactionId;
                        netPayOrder.OrderStatusId = NetPayOrderStatus.支付成功;
                        netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
                        netPayOrder.PayTime = queryResult.PayTime;
                        netPayOrder.BankType = queryResult.BankType;
                        netPayOrder.ClearPayArgs();
                        netPayOrder.ErrorCode = null;
                        netPayOrder.Mtime = DateTime.Now;

                        var eventData = new PaySuccessEventData();
                        eventData.ListNo = input.ListNo;
                        eventData.PayTypeId = netPayOrder.PayTypeId.Value;
                        eventData.Attach = input.Attach;
                        await _eventBus.TriggerAsync(eventData);
                    }
                    else
                    {
                        netPayOrder.ErrorCode = queryResult.ErrorMessage;

                        var eventData = new PayTimeoutEventData();
                        eventData.ListNo = netPayOrder.ListNo;
                        eventData.Attach = input.Attach; ;
                        await _eventBus.TriggerAsync(eventData);
                    }

                    await uow.CompleteAsync();
                }
            }
        }

        public async Task ClosePayAsync(string listNo)
        {
            var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == listNo);
            if (netPayOrder == null)
            {
                throw new UserFriendlyException($"订单{listNo}不存在");
            }

            netPayOrder.Close();

            if (!netPayOrder.IsNetPay()) return;

            await CloseNetPayAsync(netPayOrder);
        }

        private async Task CloseNetPayAsync(NetPayOrder netPayOrder)
        {
            var payService = _netPayServiceFactory.GetPayService(netPayOrder.PayTypeId.Value);

            if (netPayOrder.IsMicroPay())
            {
                var reverseCommand = new ReversePayCommand();
                reverseCommand.ListNo = netPayOrder.ListNo;
                reverseCommand.TransactionId = netPayOrder.TransactionId;
                reverseCommand.OnlinePayTradeType = netPayOrder.OnlinePayTradeType.Value;
                reverseCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                reverseCommand.PayTime = netPayOrder.Ctime;

                var reverseResult = await payService.ReversePayAsync(reverseCommand);
                if (reverseResult.ShouldRetry)
                {
                    throw new RetryJobException(reverseResult.ErrorMessage);
                }
            }
            else
            {
                var closeCommand = new ClosePayCommand();
                closeCommand.ListNo = netPayOrder.ListNo;
                closeCommand.TransactionId = netPayOrder.TransactionId;
                closeCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                closeCommand.OnlinePayTradeType = netPayOrder.OnlinePayTradeType.Value;
                closeCommand.PayTime = netPayOrder.Ctime;

                var closeResult = await payService.ClosePayAsync(closeCommand);
                if (!closeResult.Success && closeResult.IsPaid)
                {
                    throw new RetryJobException($"订单{netPayOrder.ListNo}关闭失败，订单已支付");
                }
            }
        }

        public async Task<NotifyResult> HandlePayNotifyAsync(string data, int payTypeId)
        {
            var payService = _netPayServiceFactory.GetPayService(payTypeId);

            try
            {
                var command = payService.DeserializeNotify(data);

                using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(command.ListNo)))
                {
                    if (!locker.IsAcquired)
                    {
                        throw new TmsException($"订单{command.ListNo}锁定失败");
                    }

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == command.ListNo);

                        if (netPayOrder?.OrderStatusId == NetPayOrderStatus.支付成功)
                        {
                            return payService.BuildNotifyResult(true);
                        }

                        if (ValidatePayNotify(command, netPayOrder, out string message))
                        {
                            var payArgs = netPayOrder.PayArgs.JsonToObject<PayCommandBase>();

                            netPayOrder.PayTypeId = payTypeId;
                            netPayOrder.TransactionId = command.TransactionId;
                            netPayOrder.SubTransactionId = command.SubTransactionId;
                            netPayOrder.OrderStatusId = NetPayOrderStatus.支付成功;
                            netPayOrder.OrderStatusName = netPayOrder.OrderStatusId.ToString();
                            netPayOrder.PayTime = command.PayTime;
                            netPayOrder.BankType = command.BankType;
                            netPayOrder.ClearPayArgs();
                            netPayOrder.ErrorCode = null;
                            netPayOrder.Mtime = DateTime.Now;

                            var eventData = new PaySuccessEventData();
                            eventData.ListNo = command.ListNo;
                            eventData.PayTypeId = payTypeId;
                            eventData.Attach = payArgs.Attach;
                            await _eventBus.TriggerAsync(eventData);
                        }
                        else if (command.PaySuccess)
                        {
                            var refundMoneyApply = new RefundMoneyApply();
                            refundMoneyApply.RefundListNo = await _dynamicCodeService.GenerateListNoAsync(ListNoType.门票网上订票);
                            refundMoneyApply.PayListNo = command.ListNo;
                            refundMoneyApply.RefundMoney = command.TotalFee;
                            refundMoneyApply.Reason = message;
                            await ApplyRefundAsync(refundMoneyApply);
                        }

                        await uow.CompleteAsync();
                    }

                    return payService.BuildNotifyResult(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);

                return payService.BuildNotifyResult(false, ex.Message);
            }
        }

        private bool ValidatePayNotify(NotifyCommand input, NetPayOrder netPayOrder, out string message)
        {
            if (netPayOrder == null)
            {
                message = $"订单{input.ListNo}不存在";

                return false;
            }
            if (netPayOrder.OrderStatusId == NetPayOrderStatus.已关闭)
            {
                message = $"订单{input.ListNo}已取消";

                return false;
            }
            if (!input.PaySuccess)
            {
                message = "支付失败";

                return false;
            }
            if (netPayOrder.TotalFee != input.TotalFee)
            {
                message = $"支付金额不正确，应付{netPayOrder.TotalFee}，实付{input.TotalFee}";

                return false;
            }

            message = "成功";
            return true;
        }

        public async Task ApplyRefundAsync(RefundMoneyApply refundMoneyApply)
        {
            await _refundMoneyApplyRepository.InsertAsync(refundMoneyApply);
            await _unitOfWorkManager.Current.SaveChangesAsync();

            await _backgroundJobAppService.EnqueueAsync<RefundMoneyJob>(refundMoneyApply.Id.ToString());
        }

        public async Task RefundAsync(long id)
        {
            var refundMoneyApply = await _refundMoneyApplyRepository.FirstOrDefaultAsync(id);

            var netPayOrder = await _netPayOrderRepository.FirstOrDefaultAsync(o => o.ListNo == refundMoneyApply.PayListNo);
            if (!netPayOrder.PayTypeId.HasValue)
            {
                netPayOrder.PayTypeId = await GetPayTypeIdAsync(netPayOrder.ListNo);
            }

            try
            {
                var payService = _netPayServiceFactory.GetPayService(netPayOrder.PayTypeId.Value);

                if (refundMoneyApply.ApplySuccess)
                {
                    var queryCommand = new QueryRefundCommand();
                    queryCommand.ListNo = netPayOrder.ListNo;
                    queryCommand.RefundListNo = refundMoneyApply.RefundListNo;
                    queryCommand.TransactionId = netPayOrder.TransactionId;
                    queryCommand.OnlinePayTradeType = netPayOrder.OnlinePayTradeType.Value;
                    queryCommand.RefundId = refundMoneyApply.RefundId;
                    queryCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                    queryCommand.RefundFee = refundMoneyApply.RefundMoney;
                    queryCommand.PayTime = netPayOrder.Ctime;

                    var result = await payService.QueryRefundAsync(queryCommand);

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        refundMoneyApply = await _refundMoneyApplyRepository.FirstOrDefaultAsync(id);
                        if (result.Success)
                        {
                            refundMoneyApply.Status = RefundApplyStatus.退款成功;
                            refundMoneyApply.RefundRecvAccount = result.RefundRecvAccount;
                            refundMoneyApply.RefundSuccessTime = result.RefundTime;
                        }
                        else
                        {
                            refundMoneyApply.Status = result.ShouldRetry ? RefundApplyStatus.退款中 : RefundApplyStatus.退款失败;
                        }
                        refundMoneyApply.ResultMessage = result.ErrorMessage;
                        refundMoneyApply.HandleTime = DateTime.Now;

                        await uow.CompleteAsync();
                    }

                    if (refundMoneyApply.Status == RefundApplyStatus.退款中)
                    {
                        throw new RetryJobException(result.ErrorMessage);
                    }
                }
                else
                {
                    var refundCommand = new RefundCommand();
                    refundCommand.ListNo = netPayOrder.ListNo;
                    refundCommand.RefundListNo = refundMoneyApply.RefundListNo;
                    refundCommand.TransactionId = netPayOrder.TransactionId;
                    refundCommand.OnlinePayTradeType = netPayOrder.OnlinePayTradeType.Value;
                    refundCommand.SubPayTypeId = netPayOrder.SubPayTypeId;
                    refundCommand.TotalFee = netPayOrder.TotalFee;
                    refundCommand.RefundFee = refundMoneyApply.RefundMoney;
                    refundCommand.PayTime = netPayOrder.Ctime;

                    var result = await payService.RefundAsync(refundCommand);

                    using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        refundMoneyApply = await _refundMoneyApplyRepository.FirstOrDefaultAsync(id);
                        if (result.Success)
                        {
                            refundMoneyApply.Status = RefundApplyStatus.退款中;
                            refundMoneyApply.ApplySuccess = true;
                            refundMoneyApply.ApplySuccessTime = DateTime.Now;
                            refundMoneyApply.RefundId = result.RefundId;
                        }
                        else
                        {
                            refundMoneyApply.Status = result.ShouldRetry ? RefundApplyStatus.退款中 : RefundApplyStatus.退款失败;
                        }
                        refundMoneyApply.ResultMessage = result.ErrorMessage;
                        refundMoneyApply.HandleTime = DateTime.Now;

                        await uow.CompleteAsync();
                    }

                    if (refundMoneyApply.Status == RefundApplyStatus.退款中)
                    {
                        throw new RetryJobException(result.ErrorMessage);
                    }
                }
            }
            catch (UnSupportedPayTypeException)
            {
                await RefundFailAsync(netPayOrder, refundMoneyApply);
            }
        }

        private async Task<int> GetPayTypeIdAsync(string listNo)
        {
            var payTypeId = await _tradeRepository.GetAll()
                .Where(t => t.ListNo == listNo)
                .Select(t => t.PayTypeId)
                .FirstOrDefaultAsync();

            return payTypeId.Value;
        }

        private async Task RefundFailAsync(NetPayOrder netPayOrder, RefundMoneyApply refundMoneyApply)
        {
            var trade = await _tradeRepository.GetAll()
                .Where(t => t.ListNo == refundMoneyApply.RefundListNo)
                .Select(t => new { t.SalePointId, t.CashierId, t.CashPcid })
                .FirstOrDefaultAsync();

            var employeeNo = await _staffRepository.GetAll()
                .Where(s => s.Id == trade.CashierId)
                .Select(s => s.EmployeeNo)
                .FirstOrDefaultAsync();

            var refundFail = new NetPayRefundFail();
            refundFail.NetPayTypeId = netPayOrder.NetPayTypeId;
            refundFail.NetPayTypeName = netPayOrder.NetPayTypeName;
            refundFail.ListNo = netPayOrder.ListNo;
            refundFail.RefundListNo = refundMoneyApply.RefundListNo;
            refundFail.TotalFee = netPayOrder.TotalFee.ToString();
            refundFail.RefundFee = refundMoneyApply.RefundMoney.ToString();
            refundFail.StatusId = 2;
            refundFail.StatusName = "退款失败";
            refundFail.SalePointId = trade.SalePointId;
            refundFail.CashPcId = trade.CashPcid;
            refundFail.EmployeeNo = employeeNo;
            refundFail.Memo = "WebApi不支持该付款方式";

            await _netPayRefundFailRepository.InsertAsync(refundFail);
        }

        public async Task<NetPayOrderDto> GetNetPayOrderAsync(string listNo)
        {
            var netPayOrder = await _netPayOrderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(o => o.ListNo == listNo);
            if (netPayOrder == null)
            {
                throw new UserFriendlyException($"订单{listNo}不存在");
            }

            var orderDto = new NetPayOrderDto();
            orderDto.PayMoney = netPayOrder.TotalFee;
            if (!netPayOrder.PayArgs.IsNullOrEmpty())
            {
                var payInput = netPayOrder.PayArgs.JsonToObject<PayCommandBase>();
                var timespan = payInput.PayExpireTime - DateTime.Now;
                orderDto.ExpireSeconds = Convert.ToInt64(timespan.TotalSeconds);
            }
            orderDto.PaySuccess = netPayOrder.OrderStatusId == NetPayOrderStatus.支付成功;
            orderDto.IsPaying = netPayOrder.IsPayResultUnknown();

            return orderDto;
        }
    }
}
