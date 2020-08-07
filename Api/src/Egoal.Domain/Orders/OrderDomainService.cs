using Egoal.Domain.Repositories;
using Egoal.Domain.Services;
using Egoal.Events.Bus;
using Egoal.Events.Bus.Entities;
using Egoal.Extensions;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class OrderDomainService : DomainService, IOrderDomainService
    {
        private readonly IEventBus _eventBus;
        private readonly OrderOptions _orderOptions;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<OrderDetail, long> _orderDetailRepository;

        public OrderDomainService(
            IEventBus eventBus,
            IOptions<OrderOptions> orderOptions,
            IOrderRepository orderRepository,
            IRepository<OrderDetail, long> orderDetailRepository)
        {
            _eventBus = eventBus;
            _orderOptions = orderOptions.Value;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task CreateAsync(Order order)
        {
            var travelDate = order.Etime.To<DateTime>();

            foreach (var orderDetail in order.OrderDetails)
            {
                if (orderDetail.TicketType.MinBuyNum > 0 && orderDetail.TotalNum < orderDetail.TicketType.MinBuyNum)
                {
                    throw new UserFriendlyException($"至少需购买{orderDetail.TicketType.MinBuyNum}张");
                }

                if (orderDetail.TicketType.MaxBuyNum > 0 && orderDetail.TotalNum > orderDetail.TicketType.MaxBuyNum)
                {
                    throw new UserFriendlyException($"最多可购买{orderDetail.TicketType.MaxBuyNum}张");
                }

                if (travelDate == DateTime.Now.Date && !orderDetail.TicketType.LastBookTime.IsNullOrEmpty())
                {
                    var lastBookTime = $"{order.Etime} {orderDetail.TicketType.LastBookTime}:00".To<DateTime>();
                    if (order.CTime > lastBookTime)
                    {
                        throw new UserFriendlyException("已过最晚购买时间");
                    }
                }

                if (order.MemberId.HasValue &&
                    orderDetail.TicketType.MemberLimitDays > 0 &&
                    orderDetail.TicketType.MemberLimitCount > 0)
                {
                    var timeRange = TimeSpan.FromDays(orderDetail.TicketType.MemberLimitDays - 1);
                    var buyQuantity = await _orderRepository.GetMemberBuyQuantityAsync(
                        order.MemberId.Value,
                        orderDetail.TicketType.Id,
                        travelDate.Subtract(timeRange),
                        travelDate.Add(timeRange));
                    if (buyQuantity + orderDetail.TotalNum > orderDetail.TicketType.MemberLimitCount)
                    {
                        int memberSurplusNum = orderDetail.TicketType.MemberLimitCount - buyQuantity;
                        throw new UserFriendlyException($"会员{orderDetail.TicketType.MemberLimitDays}天内，剩余可购买{(memberSurplusNum < 1 ? 0 : memberSurplusNum)}张");
                    }
                }

                if (orderDetail.TicketType.TouristInfoType == TouristInfoType.One)
                {
                    if (orderDetail.TicketType.NeedTouristName == true && order.YdrName.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("联系人姓名不能为空");
                    }
                    if (orderDetail.TicketType.NeedTouristMobile == true && order.Mobile.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("联系人手机号码不能为空");
                    }
                    if (orderDetail.TicketType.NeedCertFlag == true && order.CertNo.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("联系人证件号码不能为空");
                    }
                }

                if (orderDetail.TicketType.TouristInfoType == TouristInfoType.Every)
                {
                    if (orderDetail.OrderTourists.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("出行人信息不能为空");
                    }

                    if (orderDetail.OrderTourists.Count != orderDetail.TotalNum)
                    {
                        throw new UserFriendlyException($"需填写{orderDetail.TotalNum}个出行人");
                    }

                    if (orderDetail.TicketType.NeedTouristName == true && orderDetail.OrderTourists.Any(t => t.Name.IsNullOrEmpty()))
                    {
                        throw new UserFriendlyException("出行人姓名不能为空");
                    }

                    if (orderDetail.TicketType.NeedTouristMobile == true && orderDetail.OrderTourists.Any(t => t.Mobile.IsNullOrEmpty()))
                    {
                        throw new UserFriendlyException("出行人手机号码不能为空");
                    }
                    if (orderDetail.TicketType.NeedTouristMobile != true && order.Mobile.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("联系人手机号码不能为空");
                    }

                    if (orderDetail.TicketType.NeedCertFlag == true)
                    {
                        if (orderDetail.OrderTourists.Any(t => t.CertNo.IsNullOrEmpty()))
                        {
                            throw new UserFriendlyException("出行人证件号码不能为空");
                        }

                        var repeatedCertNos = orderDetail.OrderTourists.GroupBy(t => t.CertNo).Where(g => g.Count() > 1);
                        if (repeatedCertNos.Count() > 0)
                        {
                            throw new UserFriendlyException($"证件号码{repeatedCertNos.First().Key}重复");
                        }

                        if (_orderOptions.CertTicketSaleDaysRange > 0 && _orderOptions.CertTicketSaleNum > 0)
                        {
                            var timeRange = TimeSpan.FromDays(_orderOptions.CertTicketSaleDaysRange - 1);
                            var startTime = travelDate.Subtract(timeRange);
                            var endTime = travelDate.Add(timeRange);
                            foreach (var tourist in orderDetail.OrderTourists)
                            {
                                var buyQuantity = await _orderRepository.GetCertBuyQuantityAsync(tourist.CertNo, startTime, endTime);
                                if (buyQuantity + 1 > _orderOptions.CertTicketSaleNum)
                                {
                                    throw new UserFriendlyException($"证件{tourist.CertNo}购票数量过多");
                                }
                            }
                        }
                    }
                }
            }

            order.Sum();
            order.Audit();
        }

        public async Task<bool> AllowCancelAsync(string listNo)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(listNo);
            if (order.IsFree())
            {
                if (order.SurplusNum != order.TotalNum)
                {
                    return false;
                }
                if (order.Etime.To<DateTime>() < DateTime.Now.Date)
                {
                    return false;
                }
            }
            else
            {
                if (order.RefundStatus == RefundStatus.退款中 || order.RefundStatus == RefundStatus.已退款)
                {
                    return false;
                }

                if (order.SurplusNum <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task CancelAsync(Order order)
        {
            order.Cancel();

            var eventData = new EntityDeletingEventData<Order>(order);
            await _eventBus.TriggerAsync(eventData);
        }

        public async Task ConsumeAsync(string listNo)
        {
            var query = _orderRepository.GetAllIncluding(o => o.OrderDetails).Where(o => o.Id == listNo);
            var order = await _orderRepository.FirstOrDefaultAsync(query);
            if (order == null)
            {
                throw new UserFriendlyException($"订单核销失败，listNo:{listNo}不存在");
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                order.Consume(orderDetail, orderDetail.TotalNum);
            }
        }

        public async Task ConsumeAsync(string listNo, long orderDetailId, int consumeNum)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(listNo);
            if (order == null)
            {
                throw new UserFriendlyException($"订单核销失败，listNo:{listNo}不存在");
            }

            var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(orderDetailId);
            if (orderDetail == null)
            {
                throw new UserFriendlyException($"订单核销失败，orderDetailId:{orderDetailId}不存在");
            }

            order.Consume(orderDetail, consumeNum);
        }
    }
}
