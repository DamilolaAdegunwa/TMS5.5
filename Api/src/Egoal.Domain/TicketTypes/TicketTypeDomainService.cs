using Egoal.Authorization;
using Egoal.Common;
using Egoal.Domain.Repositories;
using Egoal.Domain.Services;
using Egoal.Extensions;
using Egoal.Tickets;
using Egoal.TicketTypes.Dto;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.TicketTypes
{
    public class TicketTypeDomainService : DomainService, ITicketTypeDomainService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IRepository<TicketTypeDateTypePrice> _ticketTypeDateTypePriceRepository;
        private readonly IRepository<TmDate> _tmDateRepository;
        private readonly IRepository<TicketTypeStock> _ticketTypeStockRepository;
        private readonly ITicketSaleStockRepository _ticketSaleStockRepository;
        private readonly IRepository<TicketTypeGateGroup> _ticketTypeGateGroupRepository;
        private readonly IRepository<TicketTypeGroundSharing> _ticketTypeGroundSharingRepository;
        private readonly IRepository<TicketTypeGroundChangCi> _ticketTypeGroundChangCiRepository;

        private readonly IRightDomainService _rightDomainService;

        public TicketTypeDomainService(
            ITicketTypeRepository ticketTypeRepository,
            IRepository<TicketTypeDateTypePrice> ticketTypeDateTypePriceRepository,
            IRepository<TmDate> tmDateRepository,
            IRepository<TicketTypeStock> ticketTypeStockRepository,
            ITicketSaleStockRepository ticketSaleStockRepository,
            IRepository<TicketTypeGateGroup> ticketTypeGateGroupRepository,
            IRepository<TicketTypeGroundSharing> ticketTypeGroundSharingRepository,
            IRepository<TicketTypeGroundChangCi> ticketTypeGroundChangCiRepository,
            IRightDomainService rightDomainService)
        {
            _ticketTypeRepository = ticketTypeRepository;
            _ticketTypeDateTypePriceRepository = ticketTypeDateTypePriceRepository;
            _tmDateRepository = tmDateRepository;
            _ticketTypeStockRepository = ticketTypeStockRepository;
            _ticketSaleStockRepository = ticketSaleStockRepository;
            _ticketTypeGateGroupRepository = ticketTypeGateGroupRepository;
            _ticketTypeGroundSharingRepository = ticketTypeGroundSharingRepository;
            _ticketTypeGroundChangCiRepository = ticketTypeGroundChangCiRepository;

            _rightDomainService = rightDomainService;
        }

        public async Task<bool> HasSpecifiedCheckGroundAsync(int ticketTypeId)
        {
            return await _ticketTypeRepository.HasSpecifiedCheckGroundAsync(ticketTypeId);
        }

        public async Task<bool> HasGrantedToGroundAsync(int ticketTypeId, int groundId)
        {
            return await _ticketTypeRepository.HasGrantedToGroundAsync(ticketTypeId, groundId);
        }

        public async Task<bool> HasGrantedToStaffAsync(int ticketTypeId, int staffId)
        {
            return await _ticketTypeRepository.HasGrantedToStaffAsync(ticketTypeId, staffId);
        }

        public async Task<bool> HasGrantedToSalePointAsync(int ticketTypeId, int salePointId)
        {
            return await _ticketTypeRepository.HasGrantedToSalePointAsync(ticketTypeId, salePointId);
        }

        public async Task<bool> HasGrantedToGateGroupAsync(int ticketTypeId, int gateGroupId)
        {
            return await _ticketTypeGateGroupRepository.AnyAsync(t => t.TicketTypeId == ticketTypeId && t.GateGroupId == gateGroupId);
        }

        public async Task<bool> HasGrantedToParkAsync(int ticketTypeId, int parkId)
        {
            return await _ticketTypeRepository.HasGrantedToParkAsync(ticketTypeId, parkId);
        }

        public async Task<bool> HasGroundSharingAsync(int ticketTypeId)
        {
            return await _ticketTypeGroundSharingRepository.GetAll().AnyAsync(t => t.TicketTypeId == ticketTypeId);
        }

        public async Task<decimal?> GetPriceAsync(TicketType ticketType, DateTime date, SaleChannel saleChannel)
        {
            var price = await GetPriceAsync(ticketType.Id, date.ToDateString());
            if (price != null)
            {
                return saleChannel == SaleChannel.Net ? price.NetPrice : price.TicPrice;
            }

            return null;
        }

        public async Task<TicketTypeDateTypePrice> GetPriceAsync(int ticketTypeId, string date)
        {
            TicketTypeDateTypePrice ticketTypeDateTypePrice = null;
            if (await IsPriceByDateAsync())
            {
                var dateTypeId = await _tmDateRepository.GetAll()
                    .Where(d => d.Date == date)
                    .Select(d => d.DateTypeId)
                    .FirstOrDefaultAsync();

                ticketTypeDateTypePrice = await _ticketTypeDateTypePriceRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.TicketTypeId == ticketTypeId && p.DateTypeId == dateTypeId);
                if(ticketTypeDateTypePrice == null)
                {
                    ticketTypeDateTypePrice = await _ticketTypeDateTypePriceRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.TicketTypeId == ticketTypeId);
                }
            }
            else
            {
                ticketTypeDateTypePrice = await _ticketTypeRepository.GetAll()
                .Where(t => t.Id == ticketTypeId)
                .Select(t => new TicketTypeDateTypePrice
                {
                    TicketTypeId = t.Id,
                    TicPrice = t.TicPrice.Value,
                    NetPrice = t.NetPrice,
                    PrintPrice = t.PrintPrice
                })
                .FirstOrDefaultAsync();
            }

            return ticketTypeDateTypePrice;
        }

        public async Task<List<TicketTypeDailyPriceDto>> GetPriceAsync(int ticketTypeId, string startDate, string endDate)
        {
            if (await IsPriceByDateAsync())
            {
                return await _ticketTypeRepository.GetPriceAsync(ticketTypeId, startDate, endDate);
            }

            var price = await _ticketTypeRepository.GetAll()
                .Where(t => t.Id == ticketTypeId)
                .Select(t => new TicketTypeDateTypePrice
                {
                    TicketTypeId = t.Id,
                    TicPrice = t.TicPrice.Value,
                    NetPrice = t.NetPrice,
                    PrintPrice = t.PrintPrice
                })
                .FirstOrDefaultAsync();

            var prices = new List<TicketTypeDailyPriceDto>();
            for (var date = startDate.To<DateTime>(); date <= endDate.To<DateTime>(); date = date.AddDays(1))
            {
                prices.Add(new TicketTypeDailyPriceDto
                {
                    TicketTypeId = ticketTypeId,
                    Date = date,
                    TicPrice = price.TicPrice,
                    NetPrice = price.NetPrice,
                    PrintPrice = price.PrintPrice
                });
            }

            return prices;
        }

        private async Task<bool> IsPriceByDateAsync()
        {
            return await _rightDomainService.HasFeatureAsync(Guid.Parse(Permissions.TMS_TicketTypeDateTypePriceSet));
        }

        public async Task<int?> GetChangCiIdAsync(int ticketTypeId, int groundId)
        {
            var changCiId = await _ticketTypeGroundChangCiRepository.GetAll()
                .Where(t => t.TicketTypeId == ticketTypeId && t.GroundId == groundId)
                .Select(t => t.ChangCiId)
                .FirstOrDefaultAsync();

            return changCiId;
        }

        public async Task ReduceStockAsync(TicketSaleStock saleStock)
        {
            var surplusStok = await GetSurplusStokAsync(saleStock.TicketTypeId, saleStock.TravelDate);
            if (!surplusStok.HasValue)
            {
                return;
            }

            if (saleStock.SaleNum > surplusStok.Value)
            {
                throw new UserFriendlyException("库存不足");
            }

            if (!await _ticketSaleStockRepository.UpdateStockAsync(saleStock))
            {
                throw new UserFriendlyException("库存更新失败");
            }
        }

        public async Task<int?> GetSurplusStokAsync(int ticketTypeId, DateTime travelDate)
        {
            var totalStock = await GetTotalStockAsync(ticketTypeId, travelDate);
            if (totalStock == null)
            {
                return null;
            }

            var saleQuantity = await _ticketSaleStockRepository.GetSaleQuantityAsync(totalStock.TicketTypeId, totalStock.StartDate, totalStock.EndDate);
            return totalStock.Stock - saleQuantity;
        }

        public async Task IncreaseStockAsync(TicketSaleStock saleStock)
        {
            var totalStock = await GetTotalStockAsync(saleStock.TicketTypeId, saleStock.TravelDate);
            if (totalStock == null)
            {
                return;
            }

            saleStock.SaleNum = -Math.Abs(saleStock.SaleNum);

            if (!await _ticketSaleStockRepository.UpdateStockAsync(saleStock))
            {
                throw new UserFriendlyException("库存更新失败");
            }
        }

        private async Task<TicketTypeStock> GetTotalStockAsync(int ticketTypeId, DateTime travelDate)
        {
            var query = _ticketTypeStockRepository.GetAll()
                .Where(s => s.TicketTypeId == ticketTypeId && s.StartDate <= travelDate && s.EndDate >= travelDate)
                .OrderBy(s => s.Days);

            return await _ticketTypeStockRepository.FirstOrDefaultAsync(query);
        }
    }
}
