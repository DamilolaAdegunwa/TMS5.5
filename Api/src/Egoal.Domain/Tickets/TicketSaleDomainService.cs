using Egoal.Caches;
using Egoal.Common;
using Egoal.Domain.Repositories;
using Egoal.Domain.Services;
using Egoal.Events.Bus;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Scenics.Dto;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.Trades;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class TicketSaleDomainService : DomainService, ITicketSaleDomainService
    {
        private readonly IEventBus _eventBus;
        private readonly ExpirationDateCalculator _expirationDateCalculator;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IRepository<TicketSalePhoto, long> _ticketSalePhotoRepository;
        private readonly IRepository<TicketGroundCache, long> _ticketGroundCacheRepository;
        private readonly IRepository<TicketGround, long> _ticketGroundRepository;
        private readonly ITicketCheckRepository _ticketCheckRepository;
        private readonly IRepository<TicketConsume, long> _ticketConsumeRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly INameCacheService _nameCacheService;
        private readonly IRepository<ChangCi> _changCiRepository;
        private readonly IRepository<MemberCard> _memberCardRepository;

        public TicketSaleDomainService(
            IEventBus eventBus,
            ExpirationDateCalculator expirationDateCalculator,
            ITicketSaleRepository ticketSaleRepository,
            ITicketTypeRepository ticketTypeRepository,
            IRepository<TicketSalePhoto, long> ticketSalePhotoRepository,
            IRepository<TicketGroundCache, long> ticketGroundCacheRepository,
            IRepository<TicketGround, long> ticketGroundRepository,
            ITicketCheckRepository ticketCheckRepository,
            IRepository<TicketConsume, long> ticketConsumeRepository,
            ITradeRepository tradeRepository,
            INameCacheService nameCacheService,
            IRepository<ChangCi> changCiRepository,
            IRepository<MemberCard> memberCardRepository)
        {
            _eventBus = eventBus;
            _expirationDateCalculator = expirationDateCalculator;
            _ticketSaleRepository = ticketSaleRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketSalePhotoRepository = ticketSalePhotoRepository;
            _ticketGroundCacheRepository = ticketGroundCacheRepository;
            _ticketGroundRepository = ticketGroundRepository;
            _ticketCheckRepository = ticketCheckRepository;
            _ticketConsumeRepository = ticketConsumeRepository;
            _tradeRepository = tradeRepository;
            _nameCacheService = nameCacheService;
            _changCiRepository = changCiRepository;
            _memberCardRepository = memberCardRepository;
        }

        public async Task<bool> ShouldInValidAsync(TicketSale ticketSale, int surplusQuantity, int refundQuantity)
        {
            if (surplusQuantity > 0)
            {
                return false;
            }

            if (ticketSale.PersonNum > 1)
            {
                var maxGroundSurplusNum = await GetMaxGroundCacheSurplusNumAsync(ticketSale);
                if (maxGroundSurplusNum.HasValue)
                {
                    if (maxGroundSurplusNum.Value > ticketSale.GetCheckNum() * refundQuantity)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<int> GetConsumeNumAsync(TicketSale ticketSale)
        {
            var consumeNums = await _ticketConsumeRepository.GetAll()
                .Where(c => c.TicketId == ticketSale.Id)
                .Select(c => c.ConsumeNum)
                .ToListAsync();

            return consumeNums.Sum();
        }

        public async Task<int> GetRealNumAsync(TicketSale ticketSale)
        {
            return ticketSale.PersonNum.Value - await GetRefundNumAsync(ticketSale);
        }

        public async Task<int> GetRefundNumAsync(TicketSale ticketSale)
        {
            var refundNums = await _ticketSaleRepository.GetAll()
                .Where(t => t.ReturnTicketId == ticketSale.Id)
                .Select(t => t.PersonNum)
                .ToListAsync();

            var refundNum = refundNums.Sum();

            return refundNum.HasValue ? Math.Abs(refundNum.Value) : 0;
        }

        public async Task<int> GetSurplusNumAsync(TicketSale ticketSale)
        {
            if (ticketSale.TicketStatusId == TicketStatus.作废)
            {
                return 0;
            }

            int checkNum = ticketSale.GetCheckNum();

            var minGroundCacheSurplusNum = await _ticketGroundCacheRepository.GetAll()
                .Where(t => t.TicketId == ticketSale.Id)
                .Select(t => t.SurplusNum)
                .MinAsync();
            if (minGroundCacheSurplusNum.HasValue)
            {
                return minGroundCacheSurplusNum.Value / checkNum;
            }

            var minGroundSurplusNum = await _ticketGroundRepository.GetAll()
                .Where(t => t.TicketId == ticketSale.Id)
                .Select(t => t.SurplusNum)
                .MinAsync();
            if (minGroundSurplusNum.HasValue)
            {
                return minGroundSurplusNum.Value / checkNum;
            }

            return ticketSale.SurplusNum.HasValue ? ticketSale.SurplusNum.Value : 0;
        }

        public async Task<int> GetPhotoQuantityAsync(TicketSale ticketSale)
        {
            return await _ticketSalePhotoRepository.GetAll()
                .Where(t => t.TicketId == ticketSale.Id)
                .CountAsync();
        }

        public async Task<DateTime?> GetLastCheckInTimeAsync(long id, bool isChecking = false)
        {
            var checkTimes = await _ticketCheckRepository.GetAll()
                .Where(t => t.TicketId == id && t.InOutFlag == true)
                .OrderByDescending(t => t.Ctime)
                .Select(t => t.Ctime)
                .Take(2)
                .ToListAsync();

            if (isChecking)
            {
                return checkTimes.Count == 2 ? checkTimes[1] : null;
            }
            else
            {
                return checkTimes.FirstOrDefault();
            }
        }

        public async Task<bool> IsUsableAsync(TicketSale ticketSale)
        {
            if (ticketSale.TicketStatusId == TicketStatus.作废)
            {
                return false;
            }

            if (ticketSale.Etime.To<DateTime>() < DateTime.Now)
            {
                return false;
            }

            if (ticketSale.TicketType == null)
            {
                ticketSale.TicketType = await _ticketTypeRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == ticketSale.TicketTypeId.Value);
            }

            if (ticketSale.TicketType.AllowSecondIn == true &&
                (ticketSale.PhotoBindFlag == true || ticketSale.FingerStatusId == FingerStatus.已登记))
            {
                return true;
            }

            var checkType = ticketSale.CheckTypeId ?? ticketSale.TicketType.CheckTypeId;
            if (checkType.IsCheckByNum())
            {
                var maxGroundSurplusNum = await GetMaxGroundCacheSurplusNumAsync(ticketSale);
                if (!maxGroundSurplusNum.HasValue || maxGroundSurplusNum.Value <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<int?> GetMaxGroundCacheSurplusNumAsync(TicketSale ticketSale)
        {
            var maxSurplusNum = await _ticketGroundCacheRepository.GetAll()
                .Where(t => t.TicketId == ticketSale.Id)
                .Select(t => t.SurplusNum)
                .MaxAsync();

            return maxSurplusNum;
        }

        public async Task<bool> AllowRefundAsync(TicketSale ticketSale)
        {
            if (ticketSale.TicketStatusId == TicketStatus.作废)
            {
                return false;
            }

            if (ticketSale.HasExchanged)
            {
                return false;
            }

            if (ticketSale.CheckTypeId == CheckType.有效期票 && ticketSale.TicketStatusId != TicketStatus.已售)
            {
                return false;
            }

            if (ticketSale.PhotoBindFlag == true || ticketSale.FingerStatusId == FingerStatus.已登记)
            {
                return false;
            }

            if (ticketSale.TicketType == null)
            {
                ticketSale.TicketType = await _ticketTypeRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == ticketSale.TicketTypeId.Value);
            }

            var checkType = ticketSale.CheckTypeId ?? ticketSale.TicketType.CheckTypeId;
            if (checkType.IsCheckByNum())
            {
                if (ticketSale.SurplusNum <= 0)
                {
                    return false;
                }
            }

            if (!ticketSale.TicketType.AllowRefund)
            {
                return false;
            }

            if (ticketSale.TicketType.AllowExpiredRefund == false && ticketSale.Etime.To<DateTime>() < DateTime.Now)
            {
                return false;
            }

            if (ticketSale.TicketType.AllowExpiredRefund == true && ticketSale.TicketType.AllowExpiredRefundMaxDays > 0)
            {
                if (ticketSale.Etime.To<DateTime>().AddDays(ticketSale.TicketType.AllowExpiredRefundMaxDays.Value) < DateTime.Now)
                {
                    return false;
                }
            }

            if (ticketSale.TicketType.TicketTypeTypeId == TicketTypeType.会员卡 && ticketSale.TicketType.FirstActiveFlag == true)
            {
                MemberCard memberCard = await _memberCardRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.CardNo == ticketSale.CardNo);
                if (memberCard != null && memberCard.AviateFlag == true)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> AllowEnrollFaceAsync(TicketSale ticketSale)
        {
            if (ticketSale.TicketStatusId != TicketStatus.已售)
            {
                return false;
            }

            if (ticketSale.Etime.To<DateTime>() < DateTime.Now)
            {
                return false;
            }

            if (ticketSale.TicketType == null)
            {
                ticketSale.TicketType = await _ticketTypeRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == ticketSale.TicketTypeId.Value);
            }

            return ticketSale.TicketType.VerifyFaceFlag == true;
        }

        public async Task ActiveAsync(TicketSale ticketSale, IEnumerable<GroundChangCiDto> groundChangCis = null)
        {
            if (ticketSale.TicketCode.IsNullOrEmpty())
            {
                throw new TmsException("票号未绑定");
            }

            if (ticketSale.CardNo.IsNullOrEmpty())
            {
                throw new TmsException("卡号未绑定");
            }

            if (ticketSale.TicketType == null || ticketSale.TicketType.TicketTypeGrounds.IsNullOrEmpty())
            {
                ticketSale.TicketType = await _ticketTypeRepository.GetAll()
                    .AsNoTracking()
                    .Include(t => t.TicketTypeGrounds)
                    .Where(t => t.Id == ticketSale.TicketTypeId)
                    .FirstOrDefaultAsync();
            }

            foreach (var ticketTypeGround in ticketSale.TicketType.TicketTypeGrounds)
            {
                var ticketGround = ticketSale.MapToTicketGround();
                ticketGround.GroundId = ticketTypeGround.GroundId;
                ticketGround.GroundPrice = ticketTypeGround.GroundPrice;
                if (ticketTypeGround.TotalNum > 0)
                {
                    ticketGround.SurplusNum = ticketGround.TotalNum = ticketTypeGround.TotalNum * ticketSale.PersonNum;
                }
                ticketSale.TicketGrounds.Add(ticketGround);

                var ticketGroundCache = ticketGround.MapToTicketGroundCache();
                ticketSale.TicketGroundCaches.Add(ticketGroundCache);

                if (!groundChangCis.IsNullOrEmpty())
                {
                    ticketGround.ChangCiId = groundChangCis.FirstOrDefault(g => g.GroundId == ticketTypeGround.GroundId)?.ChangCiId;
                    if (ticketGround.ChangCiId.HasValue)
                    {
                        var changCi = await _changCiRepository.FirstOrDefaultAsync(ticketGround.ChangCiId.Value);
                        ticketGround.Stime = $"{ticketGround.Stime.Substring(0, 10)} {changCi.Stime}:00";
                        ticketGround.Etime = $"{ticketGround.Stime.Substring(0, 10)} {changCi.Etime}:00";
                    }

                    if (!ticketSale.TicketSaleSeats.IsNullOrEmpty())
                    {
                        ticketGround.SeatId = (int?)ticketSale.TicketSaleSeats.FirstOrDefault(s => s.ChangCiId == ticketGround.ChangCiId)?.SeatId;
                    }

                    ticketGroundCache.ChangCiId = ticketGround.ChangCiId;
                    ticketGroundCache.Stime = ticketGround.Stime;
                    ticketGroundCache.Etime = ticketGround.Etime;
                    ticketGroundCache.SeatId = ticketGround.SeatId;
                }
            }

            if (!groundChangCis.IsNullOrEmpty())
            {
                if (ticketSale.TicketGroundCaches.Count == 1)
                {
                    ticketSale.Stime = ticketSale.TicketGroundCaches.First().Stime;
                    ticketSale.Etime = ticketSale.TicketGroundCaches.First().Etime;
                }
                else
                {
                    var minStime = ticketSale.TicketGroundCaches.Min(g => g.Stime.To<DateTime>());
                    if (ticketSale.Stime.To<DateTime>() > minStime)
                    {
                        ticketSale.Stime = minStime.ToDateTimeString();
                    }

                    var maxEtime = ticketSale.TicketGroundCaches.Max(g => g.Etime.To<DateTime>());
                    if (ticketSale.Etime.To<DateTime>() < maxEtime)
                    {
                        ticketSale.Etime = maxEtime.ToDateTimeString();
                    }
                }
            }

            var ticketTypeGroundTypes = await _ticketTypeRepository.GetGrantedGroundTypesAsync(ticketSale.TicketTypeId.Value);
            foreach (var ticketTypeGroundType in ticketTypeGroundTypes)
            {
                TicketGroundType ticketGroundType = new TicketGroundType();
                ticketGroundType.GroundTypeId = ticketTypeGroundType.GroundTypeId;
                ticketGroundType.CardNo = ticketSale.CardNo;
                ticketGroundType.CertNo = ticketSale.CertNo;
                ticketGroundType.TradeId = ticketSale.TradeId;
                ticketGroundType.Stime = ticketSale.Stime.To<DateTime>();
                ticketGroundType.Etime = ticketSale.Etime.To<DateTime>();
                ticketGroundType.SurplusNum = ticketGroundType.TotalNum = ticketTypeGroundType.TotalNum * ticketSale.PersonNum;
                ticketGroundType.Ctime = ticketSale.Ctime;
                ticketGroundType.ParkId = ticketSale.ParkId;
                ticketSale.AddTicketGroundType(ticketGroundType);
            }
        }

        public async Task<TicketSale> RenewAsync(long ticketId)
        {
            var query = _ticketSaleRepository.GetAllIncluding(t => t.TicketGroundCaches, t => t.TicketGrounds).Where(t => t.Id == ticketId);
            var ticketSale = await _ticketSaleRepository.FirstOrDefaultAsync(query);
            if (ticketSale == null)
            {
                throw new UserFriendlyException($"TicketID{ticketId}不存在");
            }

            var etime = (await _expirationDateCalculator.GetEndDelayTimeAsync(DateTime.Now.Date, ticketSale.TicketTypeId.Value)).ToDateTimeString();

            ticketSale.Renew(etime);

            return ticketSale;
        }

        public async Task InValidAsync(TicketSale ticketSale)
        {
            ticketSale.InValid();

            await _ticketSaleRepository.InValidAsync(ticketSale);
        }

        public async Task<int> ConsumeAsync(TicketSale ticketSale, ConsumeTicketInput input)
        {
            int realNum = await GetRealNumAsync(ticketSale);
            int consumeNum = Math.Min(realNum, input.ConsumeNum);

            var ticketType = ticketSale.TicketType ?? await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticketSale.TicketTypeId.Value);
            var checkType = ticketSale.CheckTypeId ?? ticketType.CheckTypeId;

            int groundConsumeNum = checkType == CheckType.家庭套票 ? consumeNum * ticketSale.GetCheckNum() : consumeNum;

            var ticketGroundCaches = await _ticketGroundCacheRepository.GetAllListAsync(g => g.TicketId == ticketSale.Id);
            var consumeTicketGroundCaches = input.GroundId.HasValue ? ticketGroundCaches.Where(g => g.GroundId == input.GroundId).ToList() : ticketGroundCaches;
            foreach (var ticketGroundCache in consumeTicketGroundCaches)
            {
                var thisConsumeNum = Math.Min(ticketGroundCache.SurplusNum.Value, groundConsumeNum);

                ticketGroundCache.Consume(thisConsumeNum, input.GateId);

                var ticketGround = await _ticketGroundRepository.FirstOrDefaultAsync(g => g.TicketId == ticketSale.Id && g.GroundId == ticketGroundCache.GroundId);
                ticketGround.Consume(thisConsumeNum, input.GateId);

                var ticketCheck = ticketSale.MapToTicketCheck();
                ticketCheck.GroundId = ticketGroundCache.GroundId;
                ticketCheck.GroundName = _nameCacheService.GetGroundName(ticketCheck.GroundId);
                ticketCheck.GateGroupId = input.GateGroupId;
                ticketCheck.GateGroupName = _nameCacheService.GetGateGroupName(ticketCheck.GateGroupId);
                ticketCheck.GroundPrice = ticketGroundCache.GroundPrice;
                ticketCheck.GateId = input.GateId;
                ticketCheck.GateName = _nameCacheService.GetGateName(ticketCheck.GateId);
                ticketCheck.InOutFlag = true;
                ticketCheck.InOutFlagName = "入";
                ticketCheck.CheckTypeId = ticketGroundCache.CheckTypeId;
                ticketCheck.CheckTypeName = ticketCheck.CheckTypeId?.ToString();
                ticketCheck.SurplusNum = ticketGroundCache.SurplusNum;
                ticketCheck.CheckNum = thisConsumeNum;
                ticketCheck.RecycleFlag = ticketType.RecycleFlag;
                ticketCheck.RecycleFlagName = ticketCheck.RecycleFlag == true ? "是" : "否";
                ticketCheck.CheckerId = input.CheckerId;
                ticketCheck.CheckerName = _nameCacheService.GetStaffName(ticketCheck.CheckerId);
                await _ticketCheckRepository.InsertAndGetIdAsync(ticketCheck);
            }

            int totalConsumeNum = await GetConsumeNumAsync(ticketSale);
            int surplusNum = realNum - totalConsumeNum;
            if (surplusNum > 0)
            {
                var isCheckByNum = checkType.IsCheckByNum();
                bool shouldConsume = true;
                if (isCheckByNum)
                {
                    int maxGroundTotalConsumeNum = ticketGroundCaches.Max(g => realNum * ticketSale.GetCheckNum() - g.SurplusNum).Value;
                    shouldConsume = maxGroundTotalConsumeNum > totalConsumeNum;
                }
                if (shouldConsume)
                {
                    consumeNum = Math.Min(surplusNum, consumeNum);

                    var trade = await _tradeRepository.GetAll()
                        .Where(t => t.Id == ticketSale.TradeId)
                        .Select(s => new { s.TradeSource, s.ThirdPartyPlatformId, s.ThirdPartyPlatformOrderId })
                        .FirstOrDefaultAsync();

                    var ticketConsume = new TicketConsume();
                    ticketConsume.TradeId = ticketSale.TradeId;
                    ticketConsume.TicketId = ticketSale.Id;
                    ticketConsume.CardNo = ticketSale.CardNo;
                    ticketConsume.CertNo = ticketSale.CertNo;
                    ticketConsume.TicketTypeId = ticketSale.TicketTypeId.Value;
                    ticketConsume.TicketTypeName = ticketSale.TicketTypeName;
                    ticketConsume.Price = ticketSale.ReaPrice.Value;
                    ticketConsume.ConsumeNum = consumeNum;
                    ticketConsume.ConsumeTime = DateTime.Now;
                    ticketConsume.ConsumeType = input.ConsumeType;
                    ticketConsume.NeedNotice = trade.TradeSource.IsIn(TradeSource.第三方, TradeSource.分销平台);
                    ticketConsume.ThirdPartyPlatformId = trade.ThirdPartyPlatformId;
                    ticketConsume.ThirdPartyPlatformOrderId = trade.ThirdPartyPlatformOrderId;
                    if (!ticketConsume.NeedNotice)
                    {
                        ticketConsume.HasNoticed = true;
                        ticketConsume.LastNoticeTime = ticketConsume.ConsumeTime;
                    }
                    await _ticketConsumeRepository.InsertAndGetIdAsync(ticketConsume);

                    bool validFlag = isCheckByNum ? ticketGroundCaches.Any(g => g.SurplusNum > 0) : ticketSale.Etime.To<DateTime>() > DateTime.Now;
                    ticketSale.Consume(groundConsumeNum, isCheckByNum, validFlag);

                    var eventData = new TicketConsumingEventData();
                    eventData.ListNo = ticketSale.ListNo;
                    eventData.TotalConsumeNum = totalConsumeNum + consumeNum;
                    eventData.OrderListNo = ticketSale.OrderListNo;
                    eventData.OrderDetailId = ticketSale.OrderDetailId;
                    eventData.TicketConsume = ticketConsume;
                    await _eventBus.TriggerAsync(eventData);
                }
            }
            return groundConsumeNum;
        }

        public async Task CheckOutAsync(TicketSale ticketSale, CheckOutTicketInput input)
        {
            var ticketType = ticketSale.TicketType ?? await _ticketTypeRepository.FirstOrDefaultAsync(ticketSale.TicketTypeId.Value);

            var query = _ticketGroundCacheRepository.GetAll()
                .Where(g => g.TicketId == ticketSale.Id)
                .WhereIf(input.GroundId.HasValue, g => g.GroundId == input.GroundId);
            var checkOutTicketGroundCaches = await _ticketGroundCacheRepository.ToListAsync(query);
            foreach (var ticketGroundCache in checkOutTicketGroundCaches)
            {
                ticketGroundCache.CheckOut(input.CheckNum, input.GateId);

                var ticketGround = await _ticketGroundRepository.FirstOrDefaultAsync(g => g.TicketId == ticketSale.Id && g.GroundId == ticketGroundCache.GroundId);
                ticketGround.CheckOut(input.CheckNum, input.GateId);

                var ticketCheck = ticketSale.MapToTicketCheck();
                ticketCheck.GroundId = ticketGroundCache.GroundId;
                ticketCheck.GroundName = _nameCacheService.GetGroundName(ticketGroundCache.GroundId);
                ticketCheck.GateGroupId = input.GateGroupId;
                ticketCheck.GateGroupName = input.GateGroupId.HasValue ? _nameCacheService.GetGateGroupName(input.GateGroupId.Value) : string.Empty;
                ticketCheck.GroundPrice = ticketGroundCache.GroundPrice;
                ticketCheck.GateId = input.GateId;
                ticketCheck.GateName = input.GateId.HasValue ? _nameCacheService.GetGateName(input.GateId.Value) : string.Empty;
                ticketCheck.InOutFlag = false;
                ticketCheck.InOutFlagName = "出";
                ticketCheck.CheckTypeId = ticketGroundCache.CheckTypeId;
                ticketCheck.CheckTypeName = ticketCheck.CheckTypeId?.ToString();
                ticketCheck.SurplusNum = ticketGroundCache.SurplusNum;
                ticketCheck.CheckNum = input.CheckNum;
                ticketCheck.RecycleFlag = ticketType.RecycleFlag;
                ticketCheck.RecycleFlagName = ticketCheck.RecycleFlag == true ? "是" : "否";
                ticketCheck.CheckerId = input.CheckerId;
                await _ticketCheckRepository.InsertAsync(ticketCheck);
            }
        }
    }
}
