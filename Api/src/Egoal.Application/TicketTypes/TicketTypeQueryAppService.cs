using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Caches;
using Egoal.Common;
using Egoal.Common.Dto;
using Egoal.Domain.Repositories;
using Egoal.Extensions;
using Egoal.Orders;
using Egoal.Scenics;
using Egoal.Scenics.Dto;
using Egoal.Stadiums;
using Egoal.TicketTypes.Dto;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.TicketTypes
{
    public class TicketTypeQueryAppService : ApplicationService, ITicketTypeQueryAppService
    {
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IRepository<TicketTypeDescription> _ticketTypeDescriptionRepository;
        private readonly IRepository<TicketTypeClass> _ticketTypeClassRepository;
        private readonly IRepository<TicketTypeClassDetail> _ticketTypeClassDetailRepository;
        private readonly ITicketTypeDomainService _ticketTypeDomainService;
        private readonly IRepository<Ground> _groundRepository;
        private readonly IChangCiRepository _changCiRepository;
        private readonly IStadiumDomainService _stadiumDomainService;
        private readonly IScenicDomainService _scenicDomainService;
        private readonly INameCacheService _nameCacheService;
        private readonly IOrderRepository _orderRepository;

        public TicketTypeQueryAppService(
            ITicketTypeRepository ticketTypeRepository,
            IRepository<TicketTypeDescription> ticketTypeDescriptionRepository,
            IRepository<TicketTypeClass> ticketTypeClassRepository,
            IRepository<TicketTypeClassDetail> ticketTypeClassDetailRepository,
            ITicketTypeDomainService ticketTypeDomainService,
            IRepository<Ground> groundRepository,
            IChangCiRepository changCiRepository,
            IStadiumDomainService stadiumDomainService,
            IScenicDomainService scenicDomainService,
            INameCacheService nameCacheService,
            IOrderRepository orderRepository)
        {
            _ticketTypeClassRepository = ticketTypeClassRepository;
            _ticketTypeClassDetailRepository = ticketTypeClassDetailRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketTypeDescriptionRepository = ticketTypeDescriptionRepository;
            _ticketTypeDomainService = ticketTypeDomainService;
            _groundRepository = groundRepository;
            _changCiRepository = changCiRepository;
            _stadiumDomainService = stadiumDomainService;
            _scenicDomainService = scenicDomainService;
            _nameCacheService = nameCacheService;
            _orderRepository = orderRepository;
        }

        public async Task<List<TicketTypeForSaleListDto>> GetTicketTypesForSaleAsync(GetTicketTypesForSaleInput input)
        {
            List<TicketTypeForSelfHelpDto> ticketTypeForSelfHelpDtos = await GetTicketTypeForSaleList(input);
            List<TicketTypeForSaleListDto> ticketTypeForSaleListDtos = new List<TicketTypeForSaleListDto>();
            foreach (TicketTypeForSelfHelpDto ticketTypeForSelfHelpDto in ticketTypeForSelfHelpDtos)
            {
                TicketTypeForSaleListDto ticketTypeForSaleListDto = ticketTypeForSelfHelpDto;
                ticketTypeForSaleListDtos.Add(ticketTypeForSaleListDto);
            }

            return ticketTypeForSaleListDtos;
        }

        public async Task<List<TicketTypeForSelfHelpDto>> GetTicketTypesForSelfHelpAsync(GetTicketTypesForSaleInput input)
        {
            input.TicketsOnly = true;
            List<TicketTypeForSelfHelpDto> ticketTypeForSelfHelpDtos = await GetTicketTypeForSaleList(input);

            return ticketTypeForSelfHelpDtos;
        }

        public async Task<List<TicketTypeForSelfHelpDto>> GetTicketTypeForSaleList(GetTicketTypesForSaleInput input)
        {
            if (input.SaleDate.IsNullOrEmpty())
            {
                input.SaleDate = DateTime.Now.ToDateString();
            }

            var ticketTypes = await _ticketTypeRepository.GetTicketTypesForSaleAsync(input);

            List<TicketTypeForSelfHelpDto> ticketTypeForSelfHelpDtos = new List<TicketTypeForSelfHelpDto>();
            foreach (var ticketType in ticketTypes)
            {
                if (input.TicketsOnly && ticketType.TicketTypeTypeId != TicketTypeType.门票)
                {
                    continue;
                }
                if (input.GroundId.HasValue)
                {
                    if (!await _ticketTypeDomainService.HasGrantedToGroundAsync(ticketType.Id, input.GroundId.Value))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!await _ticketTypeDomainService.HasSpecifiedCheckGroundAsync(ticketType.Id))
                    {
                        continue;
                    }
                }

                if (input.StaffId.HasValue && !await _ticketTypeDomainService.HasGrantedToStaffAsync(ticketType.Id, input.StaffId.Value))
                {
                    continue;
                }

                if (input.SalePointId.HasValue && !await _ticketTypeDomainService.HasGrantedToSalePointAsync(ticketType.Id, input.SalePointId.Value))
                {
                    continue;
                }

                if (input.ParkId.HasValue && !await _ticketTypeDomainService.HasGrantedToParkAsync(ticketType.Id, input.ParkId.Value))
                {
                    continue;
                }

                if (input.SaleChannel == SaleChannel.Local)
                {
                    if (!await _ticketTypeDomainService.HasGroundSharingAsync(ticketType.Id))
                    {
                        continue;
                    }
                }

                var startTravelDate = ticketType.GetStartTravelDate(input.SaleChannel);
                var price = await _ticketTypeDomainService.GetPriceAsync(ticketType, startTravelDate, input.SaleChannel);
                if (price == null)
                {
                    continue;
                }

                var ticketTypeClassIds = await _ticketTypeClassDetailRepository.GetAll()
                    .Where(t => t.TicketTypeId == ticketType.Id)
                    .Select(t => t.TicketTypeClassId)
                    .ToListAsync();

                var ticketTypeDto = new TicketTypeForSelfHelpDto();
                ticketTypeDto.Id = ticketType.Id;
                ticketTypeDto.Name = ticketType.GetDisplayName();
                ticketTypeDto.StartTravelDate = startTravelDate;
                ticketTypeDto.Price = price.Value;
                ticketTypeDto.AllowRefund = ticketType.AllowRefund;
                ticketTypeDto.RefundLimited = ticketType.IsRefundLimited();
                ticketTypeDto.ShouldReadDescription = ticketType.ShouldReadDescription ?? false;
                ticketTypeDto.Classes = ticketTypeClassIds.Select(id => _nameCacheService.GetTicketTypeClassName(id)).ToList();
                ticketTypeDto.NeedCertFlag = ticketType.NeedCertFlag.HasValue ? ticketType.NeedCertFlag.Value : false;
                ticketTypeDto.MinBuyNum = ticketType.MinBuyNum;
                ticketTypeDto.MaxBuyNum = ticketType.MaxBuyNum;

                ticketTypeForSelfHelpDtos.Add(ticketTypeDto);
            }

            return ticketTypeForSelfHelpDtos;
        }

        public async Task<PagedResultDto<TicketTypeDescriptionDto>> GetTicketTypeDescriptionsAsync(GetTicketTypeDescriptionsInput input)
        {
            var query = _ticketTypeDescriptionRepository.GetAll()
                 .WhereIf(input.TicketTypeId.HasValue, t => t.TicketTypeId == input.TicketTypeId);

            var count = await _ticketTypeDescriptionRepository.CountAsync(query);

            query = query.OrderBy(t => t.Id).PageBy(input);

            var descriptions = await _ticketTypeDescriptionRepository.ToListAsync(query);

            int maxLength = 15;
            var items = new List<TicketTypeDescriptionDto>();
            foreach (var description in descriptions)
            {
                var item = new TicketTypeDescriptionDto();
                item.TicketTypeId = description.TicketTypeId;
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(description.TicketTypeId);
                item.BookDescription = description.BookDescription?.Length > maxLength ? description.BookDescription.Substring(0, maxLength) : description.BookDescription;
                item.FeeDescription = description.FeeDescription?.Length > maxLength ? description.FeeDescription.Substring(0, maxLength) : description.FeeDescription;
                item.UsageDescription = description.UsageDescription?.Length > maxLength ? description.UsageDescription.Substring(0, maxLength) : description.UsageDescription;
                item.RefundDescription = description.RefundDescription?.Length > maxLength ? description.RefundDescription.Substring(0, maxLength) : description.RefundDescription;
                item.OtherDescription = description.OtherDescription?.Length > maxLength ? description.OtherDescription.Substring(0, maxLength) : description.OtherDescription;
                items.Add(item);
            }

            return new PagedResultDto<TicketTypeDescriptionDto>(count, items);
        }

        public async Task<TicketTypeDescriptionDto> GetTicketTypeDescriptionAsync(int ticketTypeId)
        {
            var descriptionDto = new TicketTypeDescriptionDto();

            var description = await _ticketTypeDescriptionRepository.FirstOrDefaultAsync(t => t.TicketTypeId == ticketTypeId);
            if (description != null)
            {
                descriptionDto.TicketTypeId = description.TicketTypeId;
                descriptionDto.BookDescription = description.BookDescription;
                descriptionDto.FeeDescription = description.FeeDescription;
                descriptionDto.UsageDescription = description.UsageDescription;
                descriptionDto.RefundDescription = description.RefundDescription;
                descriptionDto.OtherDescription = description.OtherDescription;
            }

            return descriptionDto;
        }

        public async Task<TicketTypeForNetSaleDto> GetTicketTypeForNetSaleAsync(int ticketTypeId, SaleChannel saleChannel, Guid memberId)
        {
            var query = _ticketTypeRepository.GetAllIncluding(t => t.TicketTypeGrounds).Where(t => t.Id == ticketTypeId);
            var ticketType = await _ticketTypeRepository.FirstOrDefaultAsync(query);
            if (ticketType == null)
            {
                throw new UserFriendlyException($"票类编号{ticketTypeId}不存在");
            }

            var ticketTypeDto = new TicketTypeForNetSaleDto();
            ticketTypeDto.Id = ticketType.Id;
            ticketTypeDto.Name = ticketType.GetDisplayName();
            ticketTypeDto.TicketTypeType = ticketType.TicketTypeTypeId ?? TicketTypeType.门票;
            ticketTypeDto.StartTravelDate = ticketType.GetStartTravelDate(saleChannel);
            ticketTypeDto.AllowRefund = ticketType.AllowRefund;
            ticketTypeDto.RefundLimited = ticketType.IsRefundLimited();
            ticketTypeDto.MinBuyNum = ticketType.MinBuyNum ?? 0;
            ticketTypeDto.MaxBuyNum = ticketType.GetMaxBuyNum();
            ticketTypeDto.TouristInfoType = ticketType.TouristInfoType ?? TouristInfoType.One;
            ticketTypeDto.NeedTouristName = ticketType.NeedTouristName ?? false;
            ticketTypeDto.NeedTouristMobile = ticketType.NeedTouristMobile ?? true;
            ticketTypeDto.NeedCert = ticketType.NeedCertFlag ?? false;
            ticketTypeDto.MemberLimitDays = ticketType.MemberLimitDays;

            ticketTypeDto.GroundChangCis = await GetGroundChangCisDtosVariedAsync(ticketType, ticketTypeDto.StartTravelDate);

            var today = DateTime.Now.Date;
            var endTravelDate = ticketType.GetEndTravelDate(saleChannel);
            var prices = await _ticketTypeDomainService.GetPriceAsync(ticketTypeId, ticketTypeDto.StartTravelDate.ToDateString(), endTravelDate.ToDateString());
            for (int i=0; ; i++)
            {
                DateTime dailyDateTime = today.AddDays(i);
                if(dailyDateTime > endTravelDate)
                {
                    break;
                }
                else
                {
                    TicketTypeDailyPriceDto ticketTypeDailyPriceDto = prices.FirstOrDefault(a => a.Date == dailyDateTime);
                    if(ticketTypeDailyPriceDto != null)
                    {
                        var stock = await _ticketTypeDomainService.GetSurplusStokAsync(ticketTypeId, ticketTypeDailyPriceDto.Date);

                        bool disable = stock.HasValue && stock.Value <= 0;
                        if(dailyDateTime == today && !disable)
                        {
                            disable = today != ticketTypeDto.StartTravelDate;
                        }
                        ticketTypeDto.DailyPrices.Add(new
                        {
                            Date = ticketTypeDailyPriceDto.Date.ToDateString(),
                            Price = saleChannel == SaleChannel.Net ? ticketTypeDailyPriceDto.NetPrice : ticketTypeDailyPriceDto.TicPrice,
                            Stock = stock,
                            Disable = stock.HasValue && stock.Value <= 0,
                            MemberSurplusNum = await GetMemberSurplusNum(stock, ticketType, memberId, ticketTypeDailyPriceDto.Date)
                        });
                    }
                    else
                    {
                        ticketTypeDto.DailyPrices.Add(new
                        {
                            Date = dailyDateTime.ToDateString(),
                            Price = 0,
                            Stock = 0,
                            Disable = true,
                            MemberSurplusNum = 0
                        });
                    }
                }
            }

            return ticketTypeDto;
        }

        public async Task<int> GetMemberSurplusNum(int? stock, TicketType ticketType, Guid memberId, DateTime travelDate)
        {
            int memberSurplusNum = stock ?? 0;
            if (ticketType.MemberLimitDays > 0 &&
                    ticketType.MemberLimitCount > 0)
            {
                var timeRange = TimeSpan.FromDays(ticketType.MemberLimitDays - 1);
                var buyQuantity = await _orderRepository.GetMemberBuyQuantityAsync(
                    memberId,
                    ticketType.Id,
                    travelDate.Subtract(timeRange),
                    travelDate.Add(timeRange));
                memberSurplusNum = ticketType.MemberLimitCount - buyQuantity;
            }

            return memberSurplusNum > 0 ? memberSurplusNum : 0;
        }

        public async Task<List<GroundChangCisDto>> GetTicketTypeChangCiComboboxItemsAsync(int ticketTypeId, DateTime date)
        {
            var query = _ticketTypeRepository.GetAllIncluding(t => t.TicketTypeGrounds).Where(t => t.Id == ticketTypeId);
            var ticketType = await _ticketTypeRepository.FirstOrDefaultAsync(query);

            return await GetTicketTypeChangCiComboboxItemsAsync(ticketType, date);
        }

        private async Task<List<GroundChangCisDto>> GetTicketTypeChangCiComboboxItemsAsync(TicketType ticketType, DateTime date)
        {
            List<GroundChangCisDto> groundChangCisDtos = await GetGroundChangCisDtosAsync(ticketType, date, GroundChangCiType.名称加剩余);
            return groundChangCisDtos;
        }

        public async Task<List<GroundChangCisDto>> GetGroundChangCisDtosTimeAsync(TicketType ticketType, DateTime date)
        {
            List<GroundChangCisDto> groundChangCisDtos = await GetGroundChangCisDtosAsync(ticketType, date, GroundChangCiType.名称时间余票);
            return groundChangCisDtos;
        }

        public async Task<List<GroundChangCisDto>> GetGroundChangCisDtosVariedAsync(int ticketTypeId, DateTime date)
        {
            var query = _ticketTypeRepository.GetAllIncluding(t => t.TicketTypeGrounds).Where(t => t.Id == ticketTypeId);
            var ticketType = await _ticketTypeRepository.FirstOrDefaultAsync(query);

            return await GetGroundChangCisDtosVariedAsync(ticketType, date);
        }

        public async Task<List<GroundChangCisDto>> GetGroundChangCisDtosVariedAsync(TicketType ticketType, DateTime date)
        {
            List<GroundChangCisDto> groundChangCisDtos = await GetGroundChangCisDtosAsync(ticketType, date, GroundChangCiType.多种集合);
            return groundChangCisDtos;
        }

        public async Task<List<GroundChangCisDto>> GetGroundChangCisDtosAsync(TicketType ticketType, DateTime date, GroundChangCiType groundChangCiType)
        {
            var changCis = new List<GroundChangCisDto>();

            foreach (var ticketTypeGround in ticketType.TicketTypeGrounds)
            {
                var ground = await _groundRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(g => g.Id == ticketTypeGround.GroundId);

                if (ground?.SeatSaleFlag != true && ground?.ChangCiSaleFlag != true)
                {
                    continue;
                }

                var changCisDto = new GroundChangCisDto();
                changCisDto.GroundId = ground.Id;
                changCisDto.GroundName = ground.Name;
                changCisDto.HasGroundSeat = ground.SeatSaleFlag == true;
                changCisDto.HasGroundChangCi = ground.ChangCiSaleFlag == true;

                List<ChangCiPlanDto> validChangCis = await GetChangCiPlanDtosAsync(date, ground);
                if (groundChangCiType == GroundChangCiType.名称加剩余)
                {
                    changCisDto.ChangCis = validChangCis
                    .Select(c => new ComboboxItemDto<int> { Value = c.ChangCiId, DisplayText = $"{c.ChangCiName}(剩余{c.SurplusNum})" })
                    .ToList();
                }
                if (groundChangCiType == GroundChangCiType.名称时间余票)
                {
                    changCisDto.ChangCiDtos = validChangCis.Select(c => new ChangCiDto { Name = c.ChangCiName, Stime = c.STime, Etime = c.ETime, SurplusNum = c.SurplusNum, Id = c.ChangCiId })
                        .ToList();
                }
                if (groundChangCiType == GroundChangCiType.多种集合)
                {
                    changCisDto.ChangCis = validChangCis
                    .Select(c => new ComboboxItemDto<int> { Value = c.ChangCiId, DisplayText = $"{c.ChangCiName}(剩余{c.SurplusNum})" })
                    .ToList();
                    changCisDto.ChangCiDtos = validChangCis.Select(c => new ChangCiDto { Name = c.ChangCiName, Stime = c.STime, Etime = c.ETime, SurplusNum = c.SurplusNum, Id = c.ChangCiId })
                        .ToList();
                }

                changCis.Add(changCisDto);
            }

            return changCis;
        }

        public async Task<List<ChangCiPlanDto>> GetChangCiPlanDtosAsync(DateTime date, Ground ground)
        {
            var validChangCis = new List<ChangCiPlanDto>();
            var groundChangCis = await _changCiRepository.GetChangCiPlanAsync(date.ToDateString(), ground.Id);
            foreach (var groundChangCi in groundChangCis)
            {
                if (date.Date == DateTime.Now.Date)
                {
                    if (ground.ChangCiDelaySaleMinutes.HasValue)
                    {
                        var startTime = $"{DateTime.Now.ToDateString()} {groundChangCi.STime.Substring(0, 5):00}";
                        if (startTime.To<DateTime>().AddMinutes(ground.ChangCiDelaySaleMinutes.Value) < DateTime.Now)
                        {
                            continue;
                        }
                    }

                    var endTime = $"{DateTime.Now.ToDateString()} {groundChangCi.ETime.Substring(0, 5)}:00";
                    if (endTime.To<DateTime>() < DateTime.Now)
                    {
                        continue;
                    }
                }

                if (ground.SeatSaleFlag == true && ground.StadiumId.HasValue)
                {
                    var seatSurplusQuantity = await _stadiumDomainService.GetSeatSurplusQuantityAsync(date, groundChangCi.ChangCiId, ground.StadiumId.Value);
                    if (seatSurplusQuantity <= 0)
                    {
                        continue;
                    }
                    groundChangCi.SurplusNum = seatSurplusQuantity;
                }
                else if (ground.ChangCiSaleFlag == true)
                {
                    var changCi = new ChangCi
                    {
                        Id = groundChangCi.ChangCiId,
                        ChangCiNum = groundChangCi.ChangCiNum,
                        ReservedNum = groundChangCi.ReservedNum
                    };
                    var surplusQuantity = await _scenicDomainService.GetGroundSeatSurplusQuantityAsync(ground, date, changCi);
                    if (surplusQuantity <= 0)
                    {
                        continue;
                    }
                    groundChangCi.SurplusNum = surplusQuantity;
                }

                validChangCis.Add(groundChangCi);
            }

            return validChangCis;
        }

        public async Task<List<ComboboxItemDto<int>>> GetTicketTypeTypeComboboxItemsAsync()
        {
            return await _ticketTypeRepository.GetTicketTypeTypeComboboxItemsAsync();
        }

        public async Task<List<ComboboxItemDto<int>>> GetTicketTypeComboboxItemsAsync(TicketTypeType? ticketTypeTypeId)
        {
            var query = _ticketTypeRepository.GetAll()
                .WhereIf(ticketTypeTypeId.HasValue, t => t.TicketTypeTypeId == ticketTypeTypeId)
                .OrderBy(t => t.SortCode)
                .Select(t => new ComboboxItemDto<int>
                {
                    Value = t.Id,
                    DisplayText = t.Name
                });

            var items = await _ticketTypeRepository.ToListAsync(query);

            return items;
        }

        public async Task<List<ComboboxItemDto<int>>> GetNetSaleTicketTypeComboboxItemsAsync()
        {
            var query = _ticketTypeRepository.GetAll()
                .Where(t => t.XsTypeId >= 2)
                .OrderBy(t => t.SortCode)
                .Select(t => new ComboboxItemDto<int>
                {
                    Value = t.Id,
                    DisplayText = t.Name
                });

            var items = await _ticketTypeRepository.ToListAsync(query);

            return items;
        }

        public async Task<List<ComboboxItemDto<int>>> GetTicketTypeClassComboboxItemsAsync()
        {
            var query = _ticketTypeClassRepository.GetAll()
                .OrderBy(t => t.SortCode)
                .Select(t => new ComboboxItemDto<int>
                {
                    Value = t.Id,
                    DisplayText = t.Name
                });

            var items = await _ticketTypeClassRepository.ToListAsync(query);

            return items;
        }
    }
}
