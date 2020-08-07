using Egoal.Authorization;
using Egoal.BackgroundJobs;
using Egoal.Common;
using Egoal.Dependency;
using Egoal.Domain.Repositories;
using Egoal.Events.Bus.Entities;
using Egoal.Events.Bus.Handlers;
using Egoal.Extensions;
using Egoal.TicketTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class TicketSaleCreatingEventHandler : IAsyncEventHandler<EntityCreatingEventData<TicketSale>>, IScopedDependency
    {
        private readonly IRepository<TicketTypeGroundSharing> _ticketTypeGroundSharingRepository;
        private readonly IRepository<TicketSaleGroundSharing, long> _ticketSaleGroundSharingRepository;
        private readonly IRepository<TicketTypeGroundPrice> _ticketTypeGroundPriceRepository;
        private readonly IRepository<TmDate> _tmDateRepository;
        private readonly IBackgroundJobService _backgroundJobAppService;
        private readonly IRightDomainService _rightDomainService;

        public TicketSaleCreatingEventHandler(
            IRepository<TicketTypeGroundSharing> ticketTypeGroundSharingRepository,
            IRepository<TicketSaleGroundSharing, long> ticketSaleGroundSharingRepository,
            IRepository<TicketTypeGroundPrice> ticketTypeGroundPriceRepository,
            IRepository<TmDate> tmDateRepository,
            IBackgroundJobService backgroundJobAppService,
            IRightDomainService rightDomainService)
        {
            _ticketTypeGroundSharingRepository = ticketTypeGroundSharingRepository;
            _ticketSaleGroundSharingRepository = ticketSaleGroundSharingRepository;
            _ticketTypeGroundPriceRepository = ticketTypeGroundPriceRepository;
            _tmDateRepository = tmDateRepository;
            _backgroundJobAppService = backgroundJobAppService;
            _rightDomainService = rightDomainService;
        }

        public async Task HandleEventAsync(EntityCreatingEventData<TicketSale> eventData)
        {
            await UpdateTicketSaleDayStatAsync(eventData.Entity);
            await HandleGroundSharingAsync(eventData.Entity);
        }

        private async Task UpdateTicketSaleDayStatAsync(TicketSale ticketSale)
        {
            var dayStat = new TicketSaleDayStat();
            dayStat.TicketNum = ticketSale.TicketNum ?? 0;
            dayStat.PersonNum = ticketSale.PersonNum ?? 0;
            dayStat.TicMoney = ticketSale.ReaMoney ?? 0;
            dayStat.TicketTypeId = ticketSale.TicketTypeId ?? 0;
            dayStat.CashierId = ticketSale.CashierId ?? 0;
            dayStat.CashPcid = ticketSale.CashPcid ?? 0;
            dayStat.Cdate = ticketSale.Cdate;
            dayStat.Ctp = ticketSale.Ctp;

            await _backgroundJobAppService.EnqueueAsync<UpdateTicketSaleDayStatJob>(dayStat.ToJson());
        }

        private async Task HandleGroundSharingAsync(TicketSale ticketSale)
        {
            List<TicketTypeGroundSharing> groundSharings = null;

            if (ticketSale.TicketStatusId == TicketStatus.已退)
            {
                var saleGroundSharings = await _ticketSaleGroundSharingRepository.GetAllListAsync(s => s.TicketId == ticketSale.ReturnTicketId);
                if (!saleGroundSharings.IsNullOrEmpty())
                {
                    groundSharings = saleGroundSharings.Select(s => new TicketTypeGroundSharing
                    {
                        GroundId = s.GroundId.Value,
                        SharingRate = s.SharingRate,
                        SharingPrice = s.SharingPrice
                    })
                    .ToList();
                }
            }
            else
            {
                if (await _rightDomainService.HasFeatureAsync(Guid.Parse(Permissions.TMS_PlayProjectChangCiPriceSet)))
                {
                    groundSharings = await _ticketTypeGroundPriceRepository.GetAll()
                        .Where(t => t.TicketTypeId == ticketSale.TicketTypeId && t.TicketTypePrice == ticketSale.ReaPrice && t.TradeSource == ticketSale.TradeSource)
                        .Select(t => new TicketTypeGroundSharing
                        {
                            GroundId = t.GroundId,
                            SharingPrice = t.TicPrice
                        })
                        .ToListAsync();
                }
                else
                {
                    var travelDate = ticketSale.Stime.To<DateTime>().ToDateString();
                    var dateTypeId = await _tmDateRepository.GetAll().Where(d => d.Date == travelDate).Select(d => d.DateTypeId).FirstOrDefaultAsync();
                    groundSharings = await _ticketTypeGroundSharingRepository.GetAllListAsync(g => g.TicketTypeId == ticketSale.TicketTypeId && g.DateTypeId == dateTypeId);
                }
            }

            if (groundSharings.IsNullOrEmpty())
            {
                return;
            }

            foreach (var groundSharing in groundSharings)
            {
                var ticketSaleGroundSharing = new TicketSaleGroundSharing();
                ticketSaleGroundSharing.GroundId = groundSharing.GroundId;
                ticketSaleGroundSharing.CTime = ticketSale.Ctime;

                if (groundSharing.SharingRate.HasValue)
                {
                    ticketSaleGroundSharing.SharingRate = groundSharing.SharingRate;
                    ticketSaleGroundSharing.SharingMoney = Math.Round(ticketSale.ReaMoney.Value * groundSharing.SharingRate.Value / 100, 2);
                }
                else
                {
                    ticketSaleGroundSharing.SharingPrice = groundSharing.SharingPrice;
                    ticketSaleGroundSharing.SharingNum = ticketSale.PersonNum;
                    ticketSaleGroundSharing.SharingMoney = groundSharing.SharingPrice * ticketSale.PersonNum;

                    var ticketGroundSale = new TicketGroundSale();
                    ticketGroundSale.TradeId = ticketSale.TradeId;
                    ticketGroundSale.ListNo = ticketSale.ListNo;
                    ticketGroundSale.GroundId = groundSharing.GroundId;
                    ticketGroundSale.PersonNum = ticketSale.PersonNum;
                    ticketGroundSale.UsedPersonNum = 0;
                    ticketGroundSale.UnusedPersonNum = ticketSale.PersonNum;
                    ticketGroundSale.RealPrice = groundSharing.SharingPrice;
                    ticketGroundSale.RealMoney = groundSharing.SharingPrice * ticketSale.PersonNum;
                    ticketGroundSale.CashierId = ticketSale.CashierId;
                    ticketGroundSale.CashPcId = ticketSale.CashPcid;
                    ticketGroundSale.SalePointId = ticketSale.SalePointId;
                    ticketGroundSale.CTime = ticketSale.Ctime;
                    ticketSale.AddTicketGroundSale(ticketGroundSale);
                }

                ticketSale.AddTicketSaleGroundSharing(ticketSaleGroundSharing);
            }

            if (groundSharings.Any(g => g.SharingRate.HasValue))
            {
                var totalSharingMoney = ticketSale.TicketSaleGroundSharings.Sum(t => t.SharingMoney);
                if (totalSharingMoney != ticketSale.ReaMoney)
                {
                    var ticketSaleGroundSharing = ticketSale.TicketSaleGroundSharings.First();
                    ticketSaleGroundSharing.SharingMoney += ticketSale.ReaMoney - totalSharingMoney;
                }
            }
        }
    }
}
