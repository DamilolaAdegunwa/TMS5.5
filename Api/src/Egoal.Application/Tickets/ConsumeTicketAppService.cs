using Egoal.Application.Services;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.Extensions;
using Egoal.Runtime.Session;
using Egoal.Scenics;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class ConsumeTicketAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly INameCacheService _nameCacheService;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly ITicketTypeDomainService _ticketTypeDomainService;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly IRepository<TicketGroundCache, long> _ticketGroundCacheRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IRepository<Ground> _groundRepository;

        public ConsumeTicketAppService(
            ISession session,
            INameCacheService nameCacheService,
            ITicketSaleDomainService ticketSaleDomainService,
            ITicketTypeDomainService ticketTypeDomainService,
            ITicketSaleRepository ticketSaleRepository,
            IRepository<TicketGroundCache, long> ticketGroundCacheRepository,
            ITicketTypeRepository ticketTypeRepository,
            IRepository<Ground> groundRepository)
        {
            _session = session;
            _nameCacheService = nameCacheService;
            _ticketSaleDomainService = ticketSaleDomainService;
            _ticketTypeDomainService = ticketTypeDomainService;
            _ticketSaleRepository = ticketSaleRepository;
            _ticketGroundCacheRepository = ticketGroundCacheRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _groundRepository = groundRepository;
        }

        public async Task<CheckTicketOutput> CheckTicketAsync(CheckTicketInput input)
        {
            if (!input.TicketCode.IsNullOrEmpty())
            {
                return await CheckTicketByTicketCodeAsync(input);
            }
            else if (!input.CertNo.IsNullOrEmpty())
            {
                return await CheckTicketByCertNoAsync(input);
            }
            else
            {
                throw new UserFriendlyException("无效票");
            }
        }

        private async Task<CheckTicketOutput> CheckTicketByTicketCodeAsync(CheckTicketInput input)
        {
            var ticketGroundCache = await _ticketGroundCacheRepository.GetAll()
                .Where(g => g.TicketCode == input.TicketCode)
                .Where(g => g.GroundId == input.GroundId)
                .Where(g => g.CommitFlag == true)
                .OrderByDescending(g => g.Id)
                .FirstOrDefaultAsync();

            return await CheckTicketAsync(ticketGroundCache, input);
        }

        private async Task<CheckTicketOutput> CheckTicketByCertNoAsync(CheckTicketInput input)
        {
            var ticketGroundCaches = await _ticketGroundCacheRepository.GetAll()
                .Where(g => g.TicketCode == input.CertNo || g.CertNo == input.CertNo)
                .Where(g => g.GroundId == input.GroundId)
                .Where(g => g.CommitFlag == true)
                .OrderByDescending(g => g.Id)
                .ToListAsync();

            Exception exception = null;

            foreach (var ticketGroundCache in ticketGroundCaches)
            {
                try
                {
                    return await CheckTicketAsync(ticketGroundCache, input);
                }
                catch (UserFriendlyException ex)
                {
                    exception = ex;

                    continue;
                }
            }

            throw exception == null ? new UserFriendlyException("无效票") : exception;
        }

        private async Task<CheckTicketOutput> CheckTicketAsync(TicketGroundCache ticketGroundCache, CheckTicketInput input)
        {
            if (ticketGroundCache == null)
            {
                throw new UserFriendlyException("无效票");
            }

            if (ticketGroundCache.TicketStatusId.IsIn(TicketStatus.挂失, TicketStatus.过期, TicketStatus.作废, TicketStatus.已退))
            {
                throw new UserFriendlyException($"此票{ticketGroundCache.TicketStatusId}");
            }

            if (ticketGroundCache.Etime.To<DateTime>() < DateTime.Now)
            {
                throw new UserFriendlyException("此票已过期");
            }

            var isCheckByNum = ticketGroundCache.CheckTypeId.Value.IsCheckByNum();
            if (isCheckByNum)
            {
                if (ticketGroundCache.SurplusNum <= 0)
                {
                    throw new UserFriendlyException("次数已用完");
                }

                var ground = await _groundRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(g => g.Id == input.GroundId);
                if (ground == null)
                {
                    throw new UserFriendlyException("检票区域未定义");
                }

                if (ground.LastGroundId > 0)
                {
                    if (await _ticketGroundCacheRepository.AnyAsync(t =>
                    t.TicketId == ticketGroundCache.TicketId &&
                    t.GroundId == ground.LastGroundId &&
                    t.CommitFlag == true &&
                    t.SurplusNum <= 0))
                    {
                        throw new UserFriendlyException("次数已用完");
                    }
                }
            }

            if (!await _ticketTypeDomainService.HasGrantedToGateGroupAsync(ticketGroundCache.TicketTypeId.Value, input.GateGroupId))
            {
                throw new UserFriendlyException("通道未授权");
            }

            var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticketGroundCache.TicketTypeId.Value);
            if (ticketType == null)
            {
                throw new UserFriendlyException("票类未定义");
            }

            if (ticketGroundCache.IsTodayUsed())
            {
                if (ticketType.MaxCheckNumByDay > 0 && ticketGroundCache.CheckTimesByDay >= ticketType.MaxCheckNumByDay)
                {
                    throw new UserFriendlyException("已达每日最大检票次数");
                }

                if (ticketType.CheckInterval > 0 && ticketGroundCache.LastInCheckTime.Value.AddMinutes(ticketType.CheckInterval.Value) > DateTime.Now)
                {
                    throw new UserFriendlyException("未超检票间隔");
                }
            }

            var startCheckInTime = ticketGroundCache.Stime.To<DateTime>();
            if (ticketType.EarlyIn > 0)
            {
                startCheckInTime = startCheckInTime.AddMinutes(-ticketType.EarlyIn.Value);
            }
            if (startCheckInTime > DateTime.Now)
            {
                throw new UserFriendlyException("未到检票时间");
            }

            if (ticketType.DelayIn > 0)
            {
                var endCheckInTime = ticketGroundCache.Stime.To<DateTime>().AddMinutes(ticketType.DelayIn.Value);
                if (endCheckInTime < DateTime.Now)
                {
                    throw new UserFriendlyException("已过检票时间");
                }
            }

            var ticketSale = await _ticketSaleRepository.FirstOrDefaultAsync(ticketGroundCache.TicketId);
            ticketSale.TicketType = ticketType;

            var consumeInput = new ConsumeTicketInput();
            consumeInput.ConsumeNum = ticketSale.PersonNum.Value;
            consumeInput.ConsumeType = input.ConsumeType;
            consumeInput.GroundId = input.GroundId;
            consumeInput.GateGroupId = input.GateGroupId;
            consumeInput.GateId = input.GateId;
            consumeInput.CheckerId = _session.StaffId;

            int CheckNum =  await _ticketSaleDomainService.ConsumeAsync(ticketSale, consumeInput);

            var output = new CheckTicketOutput();
            output.CardNo = ticketSale.CardNo;
            output.TicketTypeName = ticketSale.TicketTypeName;
            output.Stime = ticketGroundCache.Stime;
            output.Etime = ticketGroundCache.Etime;
            output.TotalNum = ticketGroundCache.TotalNum;
            output.CheckNum = CheckNum;
            output.SurplusNum = ticketGroundCache.SurplusNum;
            output.GroundName = _nameCacheService.GetGroundName(consumeInput.GroundId);
            output.CheckerName = _nameCacheService.GetStaffName(consumeInput.CheckerId);
            output.ShouldPrintAfterCheck = ticketType.ShouldPrintAfterCheck;
            output.CheckTime = DateTime.Now;
            if (!isCheckByNum)
            {
                output.LastCheckInTime = await _ticketSaleDomainService.GetLastCheckInTimeAsync(ticketSale.Id, true);
            }

            return output;
        }
    }
}
