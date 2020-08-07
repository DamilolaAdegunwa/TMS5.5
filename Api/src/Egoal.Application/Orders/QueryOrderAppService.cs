using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Caches;
using Egoal.Common;
using Egoal.Domain.Repositories;
using Egoal.Excel;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Orders.Dto;
using Egoal.Payment;
using Egoal.Scenics;
using Egoal.Stadiums;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class QueryOrderAppService : ApplicationService
    {
        private readonly OrderOptions _orderOptions;
        private readonly ScenicOptions _scenicOptions;

        private readonly IOrderRepository _orderRepository;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly IRepository<OrderPlan> _orderPlanRepository;
        private readonly IRepository<OrderAgeRange, long> _orderAgeRangeRepository;
        private readonly IRepository<OrderGroundChangCi, long> _orderGroundChangCiRepository;
        private readonly IRepository<OrderTourist, long> _orderTouristRepository;
        private readonly IRepository<RefundOrderApply, long> _refundOrderApplyRepository;
        private readonly IRepository<RefundMoneyApply, long> _refundMoneyApplyRepository;
        private readonly IRepository<TicketSalePhoto, long> _ticketSalePhotoRepository;
        private readonly IRepository<ChangCi> _changCiRepository;
        private readonly ISeatStatusCacheRepository _seatStatusCacheRepository;
        private readonly IOrderDomainService _orderDomainService;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly QueryTicketAppService _queryTicketAppService;
        private readonly INameCacheService _nameCacheService;
        private readonly IPayAppService _payAppService;

        public QueryOrderAppService(
            IOptions<OrderOptions> orderOptions,
            IOptions<ScenicOptions> scenicOptions,
            IOrderRepository orderRepository,
            ITicketSaleRepository ticketSaleRepository,
            IRepository<OrderPlan> orderPlanRepository,
            IRepository<OrderAgeRange, long> orderAgeRangeRepository,
            IRepository<OrderGroundChangCi, long> orderGroundChangCiRepository,
            IRepository<OrderTourist, long> orderTouristRepository,
            IRepository<RefundOrderApply, long> refundOrderApplyRepository,
            IRepository<RefundMoneyApply, long> refundMoneyApplyRepository,
            IRepository<TicketSalePhoto, long> ticketSalePhotoRepository,
            IRepository<ChangCi> changCiRepository,
            ISeatStatusCacheRepository seatStatusCacheRepository,
            IOrderDomainService orderDomainService,
            ITicketTypeRepository ticketTypeRepository,
            QueryTicketAppService queryTicketAppService,
            INameCacheService nameCacheService,
            IPayAppService payAppService)
        {
            _orderOptions = orderOptions.Value;
            _scenicOptions = scenicOptions.Value;

            _orderRepository = orderRepository;
            _ticketSaleRepository = ticketSaleRepository;
            _orderPlanRepository = orderPlanRepository;
            _orderAgeRangeRepository = orderAgeRangeRepository;
            _orderGroundChangCiRepository = orderGroundChangCiRepository;
            _orderTouristRepository = orderTouristRepository;
            _refundOrderApplyRepository = refundOrderApplyRepository;
            _refundMoneyApplyRepository = refundMoneyApplyRepository;
            _ticketSalePhotoRepository = ticketSalePhotoRepository;
            _changCiRepository = changCiRepository;
            _seatStatusCacheRepository = seatStatusCacheRepository;
            _orderDomainService = orderDomainService;
            _ticketTypeRepository = ticketTypeRepository;

            _queryTicketAppService = queryTicketAppService;
            _nameCacheService = nameCacheService;
            _payAppService = payAppService;
        }

        public async Task<PagedResultDto<OrderSimpleListDto>> GetMemberOrdersForMobileAsync(GetMemberOrdersForMobileInput input)
        {
            var now = DateTime.Now;
            var query = _orderRepository.GetAll();
            if (input.CustomerId.HasValue)
            {
                query = query.Where(o => (o.MemberId == input.MemberId && o.CustomerId == null) || o.CustomerId == input.CustomerId.Value);
            }
            else
            {
                query = query.Where(o => o.MemberId == input.MemberId && o.CustomerId == null);
            }
            if (input.IsUsable == true)
            {
                query = query.Where(o => o.PayFlag == true && o.OrderStatusId == OrderStatus.已审核 && o.EndTime >= now && o.SurplusNum > 0);
            }
            if (input.IsNotPaid == true)
            {
                query = query.Where(o => o.PayFlag == false && o.OrderStatusId == OrderStatus.已审核);
            }

            var count = await _orderRepository.CountAsync(query);

            query = query.OrderByDescending(o => o.CTime).PageBy(input);

            var orders = await _orderRepository.ToListAsync(query);

            var orderDtos = new List<OrderSimpleListDto>();
            foreach (var order in orders)
            {
                var orderDto = order.MapToSimpleListDto();
                if (orderDto.OrderStatusName == "待使用")
                {
                    var tickets = await _queryTicketAppService.GetTicketSalesByListNoAsync(order.Id);
                    int notUseNum = tickets.Where(a => a.TicketStatusName == TicketStatus.已售.ToString()).Count();
                    if (notUseNum < 1)
                    {
                        orderDto.OrderStatusName = "已完成";
                    }
                }

                orderDtos.Add(orderDto);
            }

            return new PagedResultDto<OrderSimpleListDto>(count, orderDtos);
        }

        public async Task<OrderSimpleListDto> GetMemberOrderForMobileAsync(string listNo)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(o => o.Id == listNo);

            return order.MapToSimpleListDto();
        }

        public async Task<OrderInfoDto> GetLastOrderFullInfoAsync(GetLastOrderInput input)
        {
            var queryInput = new GetOrderInfoInput();
            queryInput.StartCTime = DateTime.Now.Date;
            queryInput.EndCTime = DateTime.Now;
            queryInput.CashierId = input.CashierId;
            queryInput.CashPcid = input.CashPcid;
            queryInput.SalePointId = input.SalePointId;
            queryInput.ParkId = input.ParkId;

            return await GetOrderFullInfoAsync(queryInput);
        }

        public async Task<OrderInfoDto> GetOrderFullInfoAsync(GetOrderInfoInput input)
        {
            var order = await _orderRepository.GetAllIncluding(o => o.OrderDetails)
                .WhereIf(!input.ListNo.IsNullOrEmpty(), o => o.Id == input.ListNo)
                .WhereIf(input.StartCTime.HasValue, o => o.CTime >= input.StartCTime)
                .WhereIf(input.EndCTime.HasValue, o => o.CTime <= input.EndCTime)
                .WhereIf(input.CashierId.HasValue, o => o.CashierId == input.CashierId)
                .WhereIf(input.CashPcid.HasValue, o => o.CashPcId == input.CashPcid)
                .WhereIf(input.SalePointId.HasValue, o => o.SalePointId == input.SalePointId)
                .WhereIf(input.ParkId.HasValue, o => o.ParkId == input.ParkId)
                .OrderByDescending(o => o.CTime)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UserFriendlyException("暂无数据");
            }

            var orderInfo = new OrderInfoDto();
            orderInfo.ListNo = order.Id;
            orderInfo.OrderTypeName = order.OrderTypeId.ToString();
            orderInfo.OrderStatusName = order.GetOrderStatusName();
            orderInfo.ConsumeStatusName = order.ConsumeStatus?.ToString();
            orderInfo.RefundStatusName = order.RefundStatus?.ToString();
            orderInfo.TravelDate = order.Etime;
            orderInfo.TotalMoney = order.TotalMoney;
            orderInfo.PayTypeName = _nameCacheService.GetPayTypeName(order.PayTypeId);
            orderInfo.CTime = order.CTime.ToDateTimeString();
            orderInfo.LicensePlateNumber = order.LicensePlateNumber;
            orderInfo.JidiaoName = order.JidiaoName;
            orderInfo.ThirdListNo = order.ThirdPartyPlatformOrderId;
            orderInfo.KeYuanTypeName = _nameCacheService.GetKeYuanTypeName(order.KeYuanTypeId);
            orderInfo.AreaName = _nameCacheService.GetAreaName(order.KeYuanAreaId);
            orderInfo.CashierName = _nameCacheService.GetStaffName(order.CashierId);
            orderInfo.CustomerName = _nameCacheService.GetCustomerName(order.CustomerId);
            orderInfo.MemberName = _nameCacheService.GetMemberName(order.MemberId);
            orderInfo.GuiderName = order.GuiderName;
            orderInfo.PromoterName = _nameCacheService.GetPromoterName(order.PromoterId);
            orderInfo.Memo = order.Memo;

            orderInfo.ShouldPay = order.ShouldPay();
            if (orderInfo.ShouldPay)
            {
                var payOrder = await _payAppService.GetNetPayOrderAsync(order.Id);
                if (payOrder.ExpireSeconds <= 0)
                {
                    orderInfo.ShouldPay = false;
                }
                else
                {
                    orderInfo.ExpireSeconds = payOrder.ExpireSeconds;
                }
            }

            var tickets = await _queryTicketAppService.GetTicketSalesByListNoAsync(order.Id);

            orderInfo.AllowCancel = await _orderDomainService.AllowCancelAsync(order.Id);
            if (orderInfo.AllowCancel && !tickets.IsNullOrEmpty())
            {
                orderInfo.AllowCancel = tickets.Any(t => t.AllowRefund);
            }
            if (!tickets.IsNullOrEmpty())
            {
                int notUseNum = tickets.Where(a => a.TicketStatusName == TicketStatus.已售.ToString()).Count();
                if (orderInfo.OrderStatusName == "待使用" && notUseNum < 1)
                {
                    orderInfo.OrderStatusName = "已完成";
                }
            }

            List<TicketSaleSeatDto> seats = null;
            if (order.OrderDetails.Any(o => o.HasGroundSeat))
            {
                if (orderInfo.ShouldPay)
                {
                    seats = await _seatStatusCacheRepository.GetOrderSeatsAsync(order.Id);
                }
                else
                {
                    seats = await _queryTicketAppService.GetTicketSeatsAsync(new GetTicketSeatsInput { ListNo = order.Id });
                }
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                var orderDetailDto = new OrderDetailDto();
                orderDetailDto.Id = orderDetail.Id;
                orderDetailDto.TicketTypeId = orderDetail.TicketTypeId;
                orderDetailDto.TicketTypeName = _nameCacheService.GetTicketTypeDisplayName(orderDetail.TicketTypeId);
                orderDetailDto.TotalNum = orderDetail.TotalNum;
                orderDetailDto.SurplusNum = orderDetail.SurplusNum;
                orderDetailDto.RefundQuantity = orderDetail.ReturnNum;
                orderDetailDto.ReaPrice = orderDetail.ReaPrice.Value;
                orderDetailDto.Tickets = tickets.Where(t => t.OrderDetailId == orderDetail.Id).ToList();
                if (!orderDetailDto.Tickets.IsNullOrEmpty())
                {
                    orderDetailDto.SurplusNum = orderDetailDto.Tickets.Sum(t => t.SurplusQuantity);
                    orderDetailDto.UsableQuantity = orderDetailDto.Tickets.Where(t => t.IsUsable).Sum(t => t.SurplusQuantity);
                    orderDetailDto.RefundableQuantity = orderDetailDto.Tickets.Where(t => t.AllowRefund).Sum(t => t.SurplusQuantity);
                    orderDetailDto.ETime = orderDetailDto.Tickets.Max(t => t.Etime.To<DateTime>()).ToDateString();
                }

                var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == orderDetail.TicketTypeId);
                orderDetailDto.AllowPartialRefund = ticketType.AllowPartialRefund;
                orderDetailDto.UsageMethod = ticketType.UsageMethod;
                orderDetailDto.ShowQrCode = ticketType.WxShowQrCode;
                if (!ticketType.AllowPartialRefund && orderDetail.SurplusNum != orderDetail.TotalNum)
                {
                    orderInfo.AllowCancel = false;
                }

                orderInfo.Details.Add(orderDetailDto);

                if (orderDetail.HasGroundChangCi || orderDetail.HasGroundSeat)
                {
                    orderDetailDto.GroundChangCis = new List<OrderGroundChangCiDto>();

                    var orderGroundChangCis = await _orderGroundChangCiRepository.GetAllListAsync(g => g.OrderDetailId == orderDetail.Id);
                    foreach (var orderGroundChangCi in orderGroundChangCis)
                    {
                        var groundChangCiDto = new OrderGroundChangCiDto();
                        groundChangCiDto.GroundName = _nameCacheService.GetGroundName(orderGroundChangCi.GroundId);
                        groundChangCiDto.ChangCiName = _nameCacheService.GetChangCiName(orderGroundChangCi.ChangCiId);
                        ChangCi changCi = await _changCiRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.Id == orderGroundChangCi.ChangCiId);
                        if (changCi != null)
                        {
                            groundChangCiDto.Stime = changCi.Stime;
                            groundChangCiDto.Etime = changCi.Etime;
                        }
                        orderDetailDto.GroundChangCis.Add(groundChangCiDto);

                        if (orderDetail.HasGroundSeat)
                        {
                            groundChangCiDto.Seats = seats
                                .Where(s => s.GroundId == orderGroundChangCi.GroundId && s.ChangCiId == orderGroundChangCi.ChangCiId)
                                .Select(s => new OrderSeatDto
                                {
                                    SeatName = _nameCacheService.GetSeatName(s.SeatId)
                                })
                                .ToList();
                        }
                    }
                }

                var tourists = await _orderTouristRepository.GetAll()
                    .AsNoTracking()
                    .Where(t => t.OrderDetailId == orderDetail.Id)
                    .ToListAsync();
                if (!tourists.IsNullOrEmpty())
                {
                    foreach (OrderTourist orderTourist in tourists)
                    {
                        TouristDto touristDto = new TouristDto();
                        touristDto.Name = orderTourist.Name;
                        touristDto.Mobile = orderTourist.Mobile;
                        touristDto.CertNo = orderTourist.CertNo;
                        string certTypeName = orderTourist.CertType.HasValue ? DefaultCertType.GetName(orderTourist.CertType.Value) : "";
                        if (string.IsNullOrEmpty(certTypeName) && orderTourist.CertType.HasValue)
                        {
                            certTypeName = _nameCacheService.GetCertTypeName(orderTourist.CertType);
                        }
                        touristDto.CertTypeName = certTypeName;
                        orderInfo.Tourists.Add(touristDto);
                    }
                }
            }

            orderInfo.ShowFace = orderInfo.Details.Any(d => d.Tickets.Any(t => t.ShowFace));

            if (order.RefundStatus.HasValue)
            {
                var refundOrderApplys = await _refundOrderApplyRepository.GetAllListAsync(r => r.ListNo == order.Id);
                if (refundOrderApplys.Count == 1)
                {
                    var refundOrderApply = refundOrderApplys.First();
                    orderInfo.RefundStatusName = $"退款进度：{await GetRefundApplyStatusName(refundOrderApply)}";
                }
                else if (refundOrderApplys.Count > 1)
                {
                    if (order.RefundStatus == RefundStatus.退款失败)
                    {
                        var totalNum = refundOrderApplys.Where(r => r.Status == RefundApplyStatus.退款失败).Sum(r => r.RefundQuantity);
                        orderInfo.RefundStatusName = $"{totalNum}张退款失败";
                    }
                    else if (order.RefundStatus == RefundStatus.退款中)
                    {
                        var totalNum = refundOrderApplys.Where(r => r.Status == RefundApplyStatus.退款中).Sum(r => r.RefundQuantity);
                        orderInfo.RefundStatusName = $"{totalNum}张退款中";
                    }
                    else
                    {
                        var totalNum = refundOrderApplys.Where(r => r.Status == RefundApplyStatus.退款成功).Sum(r => r.RefundQuantity);
                        orderInfo.RefundStatusName = $"{totalNum}张已退款";
                    }
                }
            }

            orderInfo.Contact = new
            {
                ContactName = order.YdrName,
                ContactMobile = order.Mobile,
                ContactCertNo = order.CertNo,
                ContactCertTypeName = order.CertTypeName
            };

            return orderInfo;
        }

        public async Task<List<RefundApplyDto>> GetRefundApplysWithStatusDetailAsync(string listNo)
        {
            var refundOrderApplyDtos = new List<RefundApplyDto>();

            int lastRefundDays = 3;

            var refundOrderApplys = await _refundOrderApplyRepository.GetAllListAsync(r => r.ListNo == listNo);
            foreach (var refundOrderApply in refundOrderApplys)
            {
                var lastRefundDate = refundOrderApply.Ctime.AddDays(lastRefundDays).ToString("yyyy年MM月dd日");

                var refundOrderApplyDto = new RefundApplyDto();
                refundOrderApplyDto.RefundListNo = refundOrderApply.RefundListNo;
                refundOrderApplyDto.RefundTimeDescription = $"预计最晚{lastRefundDate}到账";
                refundOrderApplyDto.RefundQuantity = refundOrderApply.RefundQuantity;
                refundOrderApplyDto.RefundMoney = refundOrderApply.RefundMoney;
                refundOrderApplyDto.RefundRecvAccount = "原路退款";
                refundOrderApplyDto.RefundStatusName = await GetRefundApplyStatusName(refundOrderApply);

                refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                {
                    Title = "提交退款申请",
                    Details = new List<string>
                    {
                        refundOrderApply.Ctime.ToDateTimeString()
                    }
                });

                if (refundOrderApply.Status == RefundApplyStatus.退款中)
                {
                    refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                    {
                        Title = "等待系统审核",
                        Details = new List<string>
                        {
                            $"最晚于{refundOrderApply.Ctime.AddMinutes(15).ToDateTimeString()}前处理完毕，请耐心等待。"
                        }
                    });
                }
                else if (refundOrderApply.Status == RefundApplyStatus.退款成功)
                {
                    refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                    {
                        Title = "系统审核通过",
                        Details = new List<string>
                        {
                            refundOrderApply.HandleTime.Value.ToDateTimeString(),
                            $"{refundOrderApply.RefundMoney}元的退款申请将提交至微信"
                        }
                    });

                    var refundMoneyApply = await _refundMoneyApplyRepository.FirstOrDefaultAsync(r => r.RefundListNo == refundOrderApply.RefundListNo);
                    if (refundMoneyApply.Status == RefundApplyStatus.退款中)
                    {
                        if (refundMoneyApply.ApplySuccess)
                        {
                            refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                            {
                                Title = "微信正在处理您的退款",
                                Details = new List<string>
                                {
                                    refundMoneyApply.ApplySuccessTime.Value.ToDateTimeString(),
                                    $"预计微信会在1天内完成处理。具体处理进度可使用交易号{refundMoneyApply.RefundId}拨打微信客服95017查询。"
                                }
                            });
                        }
                    }
                    else if (refundMoneyApply.Status == RefundApplyStatus.退款成功)
                    {
                        if (!refundMoneyApply.RefundRecvAccount.IsNullOrEmpty())
                        {
                            refundOrderApplyDto.RefundRecvAccount = refundMoneyApply.RefundRecvAccount;
                        }

                        refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                        {
                            Title = "微信受理退款",
                            Details = new List<string>
                            {
                                refundMoneyApply.ApplySuccessTime.Value.ToDateTimeString(),
                                "您的退款已被微信受理"
                            }
                        });

                        var lastRefundTime = refundMoneyApply.ApplySuccessTime.Value.AddDays(lastRefundDays);
                        if (lastRefundTime > DateTime.Now)
                        {
                            refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                            {
                                Title = "微信入账中",
                                Details = new List<string>
                            {
                                refundMoneyApply.RefundSuccessTime.Value.ToDateTimeString(),
                                $"微信正将{refundMoneyApply.RefundMoney}元入账至您的{refundMoneyApply.RefundRecvAccount}，预计最晚{lastRefundDate}完成入账。具体入账进度请使用交易号{refundMoneyApply.RefundId}拨打微信客服95017查询。"
                            }
                            });
                        }
                        else
                        {
                            refundOrderApplyDto.RefundTimeDescription = "已到账";
                            refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                            {
                                Title = "退款已入账",
                                Details = new List<string>
                                {
                                    lastRefundTime.ToDateTimeString(),
                                    $"微信已在{lastRefundDate}前将{refundMoneyApply.RefundMoney}元入账至您的{refundMoneyApply.RefundRecvAccount}。如有疑问请使用交易号{refundMoneyApply.RefundId}拨打微信客服95017咨询。"
                                }
                            });
                        }
                    }
                    else
                    {
                        refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                        {
                            Title = "微信退款失败",
                            Details = new List<string>
                            {
                                refundMoneyApply.HandleTime.Value.ToDateTimeString(),
                                refundMoneyApply.ResultMessage
                            }
                        });
                    }
                }
                else
                {
                    refundOrderApplyDto.StatusDetails.Add(new RefundApplyStatusDetail
                    {
                        Title = "退款审核失败",
                        Details = new List<string>
                        {
                            refundOrderApply.HandleTime.Value.ToDateTimeString(),
                            refundOrderApply.ResultMessage
                        }
                    });
                }

                refundOrderApplyDtos.Add(refundOrderApplyDto);
            }

            return refundOrderApplyDtos;
        }

        public async Task<string> GetRefundApplyStatusName(RefundOrderApply refundOrderApply)
        {
            if (refundOrderApply.Status == RefundApplyStatus.退款中)
            {
                return "等待系统审核";
            }

            if (refundOrderApply.Status == RefundApplyStatus.退款成功)
            {
                var refundMoneyApply = await _refundMoneyApplyRepository.FirstOrDefaultAsync(r => r.RefundListNo == refundOrderApply.RefundListNo);
                if (refundMoneyApply.Status == RefundApplyStatus.退款中)
                {
                    if (refundMoneyApply.ApplySuccess)
                    {
                        return "退款入账(微信)";
                    }
                    else
                    {
                        return "系统审核通过";
                    }
                }

                if (refundMoneyApply.Status == RefundApplyStatus.退款成功)
                {
                    if (refundMoneyApply.ApplySuccessTime.Value.AddDays(3) > DateTime.Now)
                    {
                        return "微信入账中";
                    }

                    return "退款已入账";
                }

                return "微信退款失败";
            }

            return "退款审核失败";
        }

        public async Task<OrderOptionsDto> GetOrderOptionsAsync()
        {
            var optionsDto = new OrderOptionsDto();

            var orderPlans = await _orderPlanRepository.GetAllListAsync();

            var today = DateTime.Now.Date;
            var todayPlan = orderPlans.FirstOrDefault(o => o.Week == today.ToWeekString());
            var parkCloseTime = $"{today.ToDateString()} {_scenicOptions.ParkCloseTime}:00".To<DateTime>();
            optionsDto.Dates.Add(new { Date = today.ToDateString(), Text = $"今天{today.ToString("MM-dd")}", Disable = (todayPlan != null && !todayPlan.Enabled) || DateTime.Now > parkCloseTime });
            var lastDate = today;
            while (optionsDto.Dates.Count < 3)
            {
                lastDate = lastDate.AddDays(1);
                var orderPlan = orderPlans.FirstOrDefault(o => o.Week == lastDate.ToWeekString());
                if (orderPlan != null && !orderPlan.Enabled)
                {
                    continue;
                }

                var text = lastDate.ToString("MM-dd");
                if (lastDate == today.AddDays(1))
                {
                    text = $"明天{text}";
                }
                else
                {
                    text = $"{text}{lastDate.ToShortWeekString()}";
                }
                optionsDto.Dates.Add(new { Date = lastDate.ToDateString(), Text = text, Disable = false });
            }

            var disabledPlans = orderPlans.Where(o => !o.Enabled).ToList();
            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                if (disabledPlans.Any(o => o.Week == date.ToWeekString()))
                {
                    optionsDto.DisabledWeeks.Add((int)date.DayOfWeek);
                }
            }

            optionsDto.GroupOrderMaxQuantity = _orderOptions.GroupOrderMaxQuantity;
            optionsDto.IndividualOrderMaxAdultQuantity = _orderOptions.IndividualOrderMaxAdultQuantity;
            optionsDto.IndividualOrderMaxChildrenQuantity = _orderOptions.IndividualOrderMaxChildrenQuantity;
            optionsDto.PerAdultMaxChildrenQuantity = _orderOptions.PerAdultMaxChildrenQuantity;

            return optionsDto;
        }

        public async Task<List<OrderForConsumeListDto>> GetGroupOrdersForConsumeAsync(GetGroupOrdersForConsumeInput input)
        {
            var travelDate = DateTime.Now.ToDateString();

            var query = _orderRepository.GetAll()
                .Where(o => o.Etime == travelDate && o.CustomerId != null)
                .WhereIf(!input.QueryText.IsNullOrEmpty(), o => o.LicensePlateNumber.Contains(input.QueryText) || o.CustomerName.Contains(input.QueryText));

            var resultQuery = query.Select(o => new OrderForConsumeListDto
            {
                ListNo = o.Id,
                TravelDate = o.Etime,
                CustomerName = o.CustomerName,
                TotalNum = o.TotalNum,
                KeYuanTypeId = o.KeYuanTypeId,
                KeYuanAreaId = o.KeYuanAreaId,
                LicensePlateNumber = o.LicensePlateNumber,
                Memo = o.Memo
            });

            var orders = await _orderRepository.ToListAsync(resultQuery);
            foreach (var order in orders)
            {
                order.AgeRanges.AddRange(await GetOrderAgeRanges(order.ListNo));

                if (order.KeYuanTypeId.HasValue)
                {
                    order.KeYuanTypeName = _nameCacheService.GetKeYuanTypeName(order.KeYuanTypeId.Value);
                }
                if (order.KeYuanAreaId.HasValue)
                {
                    order.AreaName = _nameCacheService.GetAreaName(order.KeYuanAreaId.Value);
                }

                order.CheckInTime = await _orderRepository.GetOrderCheckInTimeAsync(order.ListNo);
                order.HasCheckIn = order.CheckInTime.HasValue;
                order.CheckOutTime = await _orderRepository.GetOrderCheckOutTimeAsync(order.ListNo);
                order.HasCheckOut = order.CheckOutTime.HasValue;
            }

            return orders.OrderBy(o => o.CheckOutTime).ThenBy(o => o.CheckInTime).ToList();
        }

        private async Task<List<OrderAgeRangeDto>> GetOrderAgeRanges(string listNo)
        {
            var ageRangeDtos = new List<OrderAgeRangeDto>();

            var ageRanges = await _orderAgeRangeRepository.GetAllListAsync(a => a.ListNo == listNo);
            if (!ageRanges.IsNullOrEmpty())
            {
                foreach (var ageRange in ageRanges)
                {
                    ageRangeDtos.Add(new OrderAgeRangeDto
                    {
                        AgeRangeName = _nameCacheService.GetAgeRangeName(ageRange.AgeRangeId),
                        PersonNum = ageRange.PersonNum
                    });
                }
            }

            return ageRangeDtos;
        }

        public async Task<byte[]> GetOrdersToExcelAsync(GetOrdersInput input)
        {
            input.ShouldPage = false;

            var result = await GetOrdersAsync(input);

            return await ExcelHelper.ExportToExcelAsync(result.Items, "订单查询", string.Empty);
        }

        public async Task<PagedResultDto<OrderListDto>> GetOrdersAsync(GetOrdersInput input)
        {
            var result = await _orderRepository.GetOrdersAsync(input);
            foreach (var order in result.Items)
            {
                order.OrderTypeName = order.OrderTypeId.ToString();
                order.OrderStatusName = order.OrderStatusId.ToString();
                order.ConsumeStatusName = order.ConsumeStatus?.ToString();
                order.RefundStatusName = order.RefundStatus?.ToString();
                order.ExplainerName = _nameCacheService.GetStaffName(order.ExplainerId);
                order.ExplainerTimeslotName = _nameCacheService.GetExplainerTimeslotName(order.ExplainerTimeId);
                order.PromoterName = _nameCacheService.GetPromoterName(order.PromoterId);
                if (order.YdrName.IsNullOrEmpty() && !order.JidiaoName.IsNullOrEmpty())
                {
                    order.YdrName = order.JidiaoName;
                }
                if (order.Mobile.IsNullOrEmpty() && !order.JidiaoMobile.IsNullOrEmpty())
                {
                    order.Mobile = order.JidiaoMobile;
                }
                if (order.TotalMoney == 0)
                {
                    order.AllowCancel = await _orderDomainService.AllowCancelAsync(order.ListNo);
                }
                if (input.NeedCheckTime)
                {
                    order.CheckInTime = await _orderRepository.GetOrderCheckInTimeAsync(order.ListNo);
                    order.CheckOutTime = await _orderRepository.GetOrderCheckOutTimeAsync(order.ListNo);
                }
            }

            return result;
        }

        public async Task<DynamicColumnResultDto> StatOrderByCustomerAsync(StatOrderByCustomerInput input)
        {
            var data = await _orderRepository.StatOrderByCustomerAsync(input);

            return new DynamicColumnResultDto(data);
        }

        public async Task<SelfHelpTicketGroundOutDto> GetSelfHelpTicketGroundAsync(SelfHelpTicketGroundInput input)
        {
            SelfHelpTicketGroundOutDto selfHelpTicketGroundOutDto = new SelfHelpTicketGroundOutDto();

            selfHelpTicketGroundOutDto.TicketCode = input.TicketCode;
            var result = await _orderRepository.GetSelfHelpTicketGroundAsync(input);
            if (result.Items.Count > 0)
            {
                foreach (SelfHelpTicketGroundListDto selfHelpTicketGroundListDto in result.Items)
                {
                    selfHelpTicketGroundListDto.LastCheckTime = selfHelpTicketGroundListDto.LastInCheckTime?.ToDateTimeString();
                    if (selfHelpTicketGroundOutDto.TicketStatusName.IsNullOrEmpty())
                    {
                        if (selfHelpTicketGroundListDto.ETime < DateTime.Now)
                        {
                            selfHelpTicketGroundOutDto.TicketStatusName = "已过期";
                        }
                        else if (selfHelpTicketGroundListDto.SurplusNum > 0)
                        {
                            selfHelpTicketGroundOutDto.TicketStatusName = "未使用";
                        }
                    }
                }
            }

            selfHelpTicketGroundOutDto.PageResult = result;

            return selfHelpTicketGroundOutDto;
        }

        public async Task<SelfHelpGetOrderTicketDto> GetSelfHelpOrderTicketAsync(SelfHelpOrderTicketInput input)
        {
            SelfHelpGetOrderTicketDto selfHelpGetOrderTicketDto = new SelfHelpGetOrderTicketDto();
            List<TicketSale> ticketSales = new List<TicketSale>();
            List<OrderTourist> orderTourists = new List<OrderTourist>();
            if (!string.IsNullOrEmpty(input.IdCardNum))
            {
                ticketSales = await _ticketSaleRepository.GetAll().AsNoTracking().Where(a => a.TicketCode == input.IdCardNum).ToListAsync();
                orderTourists = await _orderTouristRepository.GetAll().AsNoTracking().Where(a => a.CertNo == input.IdCardNum).ToListAsync();
            }
            if (!string.IsNullOrEmpty(input.TicketCode))
            {
                ticketSales = await _ticketSaleRepository.GetAll().AsNoTracking().Where(a => a.TicketCode == input.IdCardNum).ToListAsync();
                orderTourists.Clear();
            }
            if (!string.IsNullOrEmpty(input.ListNo) && !string.IsNullOrEmpty(input.Mobile))
            {
                Order order = await _orderRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.Id == input.ListNo && a.Mobile == input.Mobile);
                if (order != null)
                {
                    ticketSales = await _ticketSaleRepository.GetAll().AsNoTracking().Where(a => a.ListNo == input.ListNo).ToListAsync();
                }
                orderTourists.Clear();
            }

            selfHelpGetOrderTicketDto = await _queryTicketAppService.GetSelfHelpOrderTicketByTicketSales(ticketSales, orderTourists);
            return selfHelpGetOrderTicketDto;
        }
    }
}
