using Egoal.Application.Services;
using Egoal.AutoMapper;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.DynamicCodes;
using Egoal.Events.Bus;
using Egoal.Members;
using Egoal.Runtime.Session;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.Trades;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class RefundTicketAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly IEventBus _eventBus;
        private readonly INameCacheService _nameCacheService;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly ITradeAppService _tradeAppService;
        private readonly FaceAppService _faceAppService;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly ITicketSaleSeatRepository _ticketSaleSeatRepository;
        private readonly IRepository<TicketSalePhoto, long> _ticketSalePhotoRepository;
        private readonly IRepository<MemberCard> _memberCardRepository;

        public RefundTicketAppService(
            ISession session,
            IEventBus eventBus,
            INameCacheService nameCacheService,
            IDynamicCodeService dynamicCodeService,
            ITradeAppService tradeAppService,
            FaceAppService faceAppService,
            ITicketSaleDomainService ticketSaleDomainService,
            ITicketSaleRepository ticketSaleRepository,
            ITradeRepository tradeRepository,
            ITicketSaleSeatRepository ticketSaleSeatRepository,
            IRepository<TicketSalePhoto, long> ticketSalePhotoRepository,
            IRepository<MemberCard> memberCardRepository)
        {
            _session = session;
            _eventBus = eventBus;
            _nameCacheService = nameCacheService;
            _dynamicCodeService = dynamicCodeService;
            _tradeAppService = tradeAppService;
            _faceAppService = faceAppService;
            _ticketSaleDomainService = ticketSaleDomainService;
            _ticketSaleRepository = ticketSaleRepository;
            _tradeRepository = tradeRepository;
            _ticketSaleSeatRepository = ticketSaleSeatRepository;
            _ticketSalePhotoRepository = ticketSalePhotoRepository;
            _memberCardRepository = memberCardRepository;
        }

        public async Task RefundAsync(RefundInput input, RefundChannel refundChannel)
        {
            var ticketSale = await _ticketSaleRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TicketCode == input.TicketCode && t.TicketStatusId != TicketStatus.已退);
            if (ticketSale == null)
            {
                throw new UserFriendlyException("无效票");
            }

            if (refundChannel == RefundChannel.安卓手持机)
            {
                if (ticketSale.CashierId != _session.StaffId)
                {
                    throw new UserFriendlyException("只能退自己销售的票");
                }
            }

            if (!await _ticketSaleDomainService.AllowRefundAsync(ticketSale))
            {
                throw new UserFriendlyException("票不可退");
            }

            var surplusNum = await _ticketSaleDomainService.GetSurplusNumAsync(ticketSale);
            if (surplusNum <= 0)
            {
                throw new UserFriendlyException("次数已用完");
            }

            var tradeSource = await _tradeRepository.GetAll()
                .Where(t => t.Id == ticketSale.TradeId)
                .Select(t => t.TradeSource)
                .FirstOrDefaultAsync();

            var refundTicketInput = new RefundTicketInput();
            refundTicketInput.RefundListNo = await _dynamicCodeService.GenerateListNoAsync(tradeSource == TradeSource.本地 ? ListNoType.门票 : ListNoType.门票网上订票);
            refundTicketInput.PayListNo = ticketSale.ListNo;
            refundTicketInput.PayTypeId = ticketSale.PayTypeId.Value;
            refundTicketInput.RefundReason = "景区退票";
            refundTicketInput.CashierId = input.CashierId;
            refundTicketInput.CashierName = _nameCacheService.GetStaffName(refundTicketInput.CashierId);
            refundTicketInput.CashPcid = input.CashPcid;
            refundTicketInput.CashPcname = _nameCacheService.GetPcName(refundTicketInput.CashPcid);
            refundTicketInput.SalePointId = input.SalePointId;
            refundTicketInput.SalePointName = _nameCacheService.GetSalePointName(refundTicketInput.SalePointId);
            refundTicketInput.ParkId = input.ParkId;
            refundTicketInput.ParkName = _nameCacheService.GetParkName(refundTicketInput.ParkId);
            refundTicketInput.OriginalTradeId = ticketSale.TradeId;

            RefundTicketItem refundTicketItem = new RefundTicketItem();
            refundTicketItem.TicketId = ticketSale.Id;
            refundTicketItem.RefundQuantity = surplusNum;
            refundTicketItem.SurplusQuantityAfterRefund = 0;
            refundTicketInput.Items.Add(refundTicketItem);

            await RefundAsync(refundTicketInput);
        }

        public async Task RefundAsync(RefundTicketInput input)
        {
            var originalTicketSales = new List<TicketSale>();
            var ticketSales = new List<TicketSale>();

            foreach (var item in input.Items)
            {
                var originalTicketSale = await _ticketSaleRepository.FirstOrDefaultAsync(item.TicketId);
                originalTicketSales.Add(originalTicketSale);

                int checkNum = originalTicketSale.GetCheckNum();

                var ticketSale = input.MapToTicketSale();
                originalTicketSale.CopyTo(ticketSale);
                ticketSale.ValidFlag = false;
                ticketSale.ValidFlagName = "无效";
                ticketSale.TicketStatusId = TicketStatus.已退;
                ticketSale.TicketStatusName = TicketStatus.已退.ToString();
                ticketSale.TicketNum = -originalTicketSale.TicketNum;
                int personNum = -item.RefundQuantity;
                ticketSale.PersonNum = personNum;
                ticketSale.TotalNum = checkNum * personNum;
                ticketSale.SurplusNum = 0;
                ticketSale.SetTicMoney(personNum, originalTicketSale.TicPrice);
                ticketSale.SetReaMoney(personNum, originalTicketSale.ReaPrice);
                ticketSale.SetPrintMoney(personNum, originalTicketSale.PrintPrice);
                ticketSale.PayMoney = ticketSale.ReaMoney;
                ticketSale.ReturnTicketId = originalTicketSale.Id;
                ticketSale.ReturnRate = 100M;

                ticketSales.Add(ticketSale);

                if (await _ticketSaleDomainService.ShouldInValidAsync(originalTicketSale, item.SurplusQuantityAfterRefund, item.RefundQuantity))
                {
                    await _ticketSaleDomainService.InValidAsync(originalTicketSale);
                }
                else
                {
                    int refundCheckNum = item.RefundQuantity * checkNum;
                    originalTicketSale.SurplusNum -= refundCheckNum;
                    await _ticketSaleRepository.RefundAsync(originalTicketSale, refundCheckNum);
                }

                await DeleteSeatsAsync(originalTicketSale, item.RefundQuantity);

                await DeletePhotosAsync(originalTicketSale, item.RefundQuantity);

                await DeleteMemberCardAsync(originalTicketSale);

                await _ticketSaleRepository.InsertAsync(ticketSale);
            }

            input.TotalMoney = ticketSales.Sum(t => t.ReaMoney.Value);

            var eventData = input.MapTo<RefundTicketEventData>();
            eventData.Items.Select(i => i.OriginalTicketSale = originalTicketSales.FirstOrDefault(t => t.Id == i.TicketId)).ToArray();
            await _eventBus.TriggerAsync(eventData);

            await _tradeAppService.RefundTicketAsync(input);
        }

        private async Task DeleteSeatsAsync(TicketSale ticketSale, int quantity)
        {
            if (ticketSale.CheckTypeId == CheckType.家庭套票)
            {
                quantity = quantity * ticketSale.GetCheckNum();
            }

            var seats = await _ticketSaleSeatRepository.GetAll()
                .Where(s => s.TicketId == ticketSale.Id)
                .OrderBy(s => s.SeatId)
                .Take(quantity)
                .ToListAsync();
            foreach (var seat in seats)
            {
                await _ticketSaleSeatRepository.DeleteAsync(seat);
            }
        }

        private async Task DeletePhotosAsync(TicketSale ticketSale, int quantity)
        {
            if (ticketSale.CheckTypeId == CheckType.家庭套票)
            {
                quantity = quantity * ticketSale.GetCheckNum();
            }

            var photos = await _ticketSalePhotoRepository.GetAll()
                .Where(t => t.TicketId == ticketSale.Id)
                .OrderBy(t => t.Id)
                .Take(quantity)
                .Select(t => new { t.Id })
                .ToListAsync();

            foreach (var photo in photos)
            {
                await _faceAppService.DeleteFaceAsync(photo.Id);
            }
        }

        private async Task DeleteMemberCardAsync(TicketSale ticketSale)
        {
            if (ticketSale.TicketTypeTypeId != TicketTypeType.会员卡) return;

            var memberCard = await _memberCardRepository.FirstOrDefaultAsync(m => m.TicketId == ticketSale.Id);

            await _memberCardRepository.DeleteAsync(memberCard);
        }
    }
}
