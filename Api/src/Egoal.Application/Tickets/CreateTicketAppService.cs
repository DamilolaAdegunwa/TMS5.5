using Egoal.Application.Services;
using Egoal.Caches;
using Egoal.DynamicCodes;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.Trades;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class CreateTicketAppService : ApplicationService
    {
        private readonly INameCacheService _nameCacheService;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly ITradeAppService _tradeAppService;
        private readonly ITicketTypeDomainService _ticketTypeDomainService;
        private readonly ExpirationDateCalculator _expirationDateCalculator;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly ITicketSaleRepository _ticketSaleRepository;

        public CreateTicketAppService(
            INameCacheService nameCacheService,
            IDynamicCodeService dynamicCodeService,
            ITradeAppService tradeAppService,
            ITicketTypeDomainService ticketTypeDomainService,
            ExpirationDateCalculator expirationDateCalculator,
            ITicketSaleDomainService ticketSaleDomainService,
            ITicketTypeRepository ticketTypeRepository,
            ITicketSaleRepository ticketSaleRepository)
        {
            _nameCacheService = nameCacheService;
            _dynamicCodeService = dynamicCodeService;
            _tradeAppService = tradeAppService;
            _ticketTypeDomainService = ticketTypeDomainService;
            _expirationDateCalculator = expirationDateCalculator;
            _ticketSaleDomainService = ticketSaleDomainService;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketSaleRepository = ticketSaleRepository;
        }

        public async Task<List<TicketSale>> SaleAsync(SaleTicketInput input)
        {
            var ticketSales = new List<TicketSale>();

            foreach (var item in input.Items)
            {
                var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == item.TicketTypeId);

                int personNum = 1;
                int ticketNum = item.Quantity;
                if (ticketType.CheckTypeId == CheckType.多人票)
                {
                    personNum = item.Quantity;
                    ticketNum = 1;
                }

                for (int i = 0; i < ticketNum; i++)
                {
                    var ticketSale = input.MapToTicketSale();
                    ticketSale.TicketStatusId = TicketStatus.已售;
                    ticketSale.TicketStatusName = ticketSale.TicketStatusId.ToString();
                    ticketSale.TicketType = ticketType;
                    ticketSale.TicketTypeTypeId = ticketType.TicketTypeTypeId;
                    ticketSale.TicketTypeId = ticketType.Id;
                    ticketSale.TicketTypeName = ticketType.Name;
                    ticketSale.TicketBindTypeId = ticketType.TicketBindTypeId;
                    ticketSale.Tkid = ticketType.Tkid;
                    ticketSale.Tkname = ticketSale.Tkid?.ToString();
                    ticketSale.Ttid = ticketType.Ttid;
                    ticketSale.PersonNum = personNum;
                    if (input.IsExchange)
                    {
                        ticketSale.SetTicMoney(personNum, item.TicPrice);
                        ticketSale.SetReaMoney(personNum, item.RealPrice);
                    }
                    else
                    {
                        var price = await _ticketTypeDomainService.GetPriceAsync(ticketType, input.TravelDate, input.SaleChannel);
                        ticketSale.SetTicMoney(personNum, price);
                        ticketSale.SetReaMoney(personNum, price);
                    }
                    ticketSale.PayMoney = ticketSale.ReaMoney;
                    var dailyPrice = await _ticketTypeDomainService.GetPriceAsync(item.TicketTypeId, input.TravelDate.ToDateString());
                    var printPrice = dailyPrice?.PrintPrice;
                    ticketSale.SetPrintMoney(personNum, printPrice ?? ticketType.PrintPrice);
                    ticketSale.TotalNum = ticketSale.SurplusNum = personNum * ticketType.CheckNum;
                    ticketSale.Stime = _expirationDateCalculator.GetStartValidTime(input.TravelDate, ticketType).ToDateTimeString();
                    ticketSale.Etime = _expirationDateCalculator.GetEndValidTime(input.TravelDate, ticketType).ToDateTimeString();
                    ticketSale.Sdate = ticketSale.Stime.Substring(0, 10);
                    ticketSale.CheckTypeId = ticketType.CheckTypeId;
                    ticketSale.OrderDetailId = item.OrderDetailId;
                    ticketSale.StatFlag = ticketType.StatFlag;

                    if (!item.Tourists.IsNullOrEmpty())
                    {
                        if (ticketNum == item.Tourists.Count)
                        {
                            var tourist = item.Tourists[i];

                            BindTicketSaleTourist(ticketSale, tourist);
                            ticketSale.CertTypeId = tourist.CertType;
                            ticketSale.CertTypeName = _nameCacheService.GetCertTypeName(ticketSale.CertTypeId);
                            ticketSale.CertNo = tourist.CertNo;
                        }
                        else
                        {
                            foreach (var tourist in item.Tourists)
                            {
                                BindTicketSaleTourist(ticketSale, tourist);
                            }
                        }
                    }

                    if (item.HasGroundSeat)
                    {
                        foreach (var groundChangCi in item.GroundChangCis)
                        {
                            var seatQuantity = ticketType.CheckTypeId == CheckType.家庭套票 ? personNum * ticketType.CheckNum.Value : personNum;
                            var seats = input.Seats.Where(s => s.GroundId == groundChangCi.GroundId && s.ChangCiId == groundChangCi.ChangCiId).Take(seatQuantity).ToList();
                            foreach (var seat in seats)
                            {
                                var ticketSaleSeat = new TicketSaleSeat();
                                ticketSaleSeat.TradeId = ticketSale.TradeId;
                                ticketSaleSeat.SeatId = seat.SeatId;
                                ticketSaleSeat.Sdate = seat.Sdate;
                                ticketSaleSeat.ChangCiId = seat.ChangCiId;
                                ticketSale.AddTicketSaleSeat(ticketSaleSeat);

                                input.Seats.Remove(seat);
                            }
                        }
                    }

                    if (!item.GroundChangCis.IsNullOrEmpty())
                    {
                        ticketSale.Memo = _nameCacheService.GetChangCiName(item.GroundChangCis.First().ChangCiId);
                    }

                    string ticketCode = string.Empty;
                    if (input.TradeSource == TradeSource.微信)
                    {
                        ticketCode = await _dynamicCodeService.GenerateWxTicketCodeAsync();
                    }
                    else
                    {
                        ticketCode = await _dynamicCodeService.GenerateParkTicketCodeAsync(input.ParkId ?? 1);
                    }
                    ticketSale.BindTicket(ticketCode, ticketCode);
                    if (ticketType.FirstActiveFlag != true)
                    {
                        await _ticketSaleDomainService.ActiveAsync(ticketSale, item.GroundChangCis);
                    }

                    if(i == 0)
                    {
                        AddOrderTicketSaleBuyer(input, ticketSale);
                    }

                    ticketSales.Add(ticketSale);
                }
            }

            foreach (var ticketSale in ticketSales)
            {
                if (input.PayFlag)
                {
                    ticketSale.Pay(input.PayTypeId, input.PayTypeName);
                }
                await _ticketSaleRepository.InsertAndGetIdAsync(ticketSale);

                if (ticketSale.TicketTypeTypeId == TicketTypeType.会员卡)
                {
                    var memberCard = ticketSale.MapToMemberCard();
                    memberCard.IsElectronicTicket = true;
                    memberCard.AviateFlag = ticketSale.TicketType != null && ticketSale.TicketType.FirstActiveFlag != true;
                    ticketSale.AddMemberCard(memberCard);
                }
            }

            input.TotalMoney = ticketSales.Sum(t => t.ReaMoney.Value);
            input.StatFlag = ticketSales.Any(t => t.StatFlag == true);
            await _tradeAppService.SaleTicketAsync(input);

            return ticketSales;
        }

        /// <summary>
        /// 订单联系人添加到游客表里，以游客统计
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ticketSale"></param>
        private void AddOrderTicketSaleBuyer(SaleTicketInput input, TicketSale ticketSale)
        {
            if(!string.IsNullOrEmpty(input.ContactCertNo))
            {
                TicketTourist ticketTourist = new TicketTourist();
                ticketTourist.Name = input.ContactName;
                ticketTourist.Mobile = input.ContactMobile;
                ticketTourist.CertType = input.ContactCertTypeId;
                ticketTourist.CertNo = input.ContactCertNo;
                BindTicketSaleTourist(ticketSale, ticketTourist);
            }
        }

        private void BindTicketSaleTourist(TicketSale ticketSale, TicketTourist tourist)
        {
            var ticketSaleBuyer = ticketSale.MapToTicketSaleBuyer();
            ticketSaleBuyer.BuyerName = tourist.Name;
            ticketSaleBuyer.Mobile = tourist.Mobile;
            ticketSaleBuyer.CertTypeId = tourist.CertType;
            if (!tourist.Birthday.IsNullOrEmpty())
            {
                ticketSaleBuyer.Birthday = tourist.Birthday;
            }
            if (!tourist.CertNo.IsNullOrEmpty() && tourist.CertType == DefaultCertType.二代身份证)
            {
                ticketSaleBuyer.SetIdCardNo(tourist.CertNo);
            }
            else
            {
                string certTypeName = tourist.CertType.HasValue ? DefaultCertType.GetName(tourist.CertType.Value) : "";
                if (string.IsNullOrEmpty(certTypeName) && tourist.CertType.HasValue)
                {
                    certTypeName = _nameCacheService.GetCertTypeName(tourist.CertType);
                }
                ticketSaleBuyer.CertTypeName = certTypeName;
                ticketSaleBuyer.CertNo = tourist.CertNo;
            }

            ticketSale.AddTicketSaleBuyer(ticketSaleBuyer);
        }
    }
}
