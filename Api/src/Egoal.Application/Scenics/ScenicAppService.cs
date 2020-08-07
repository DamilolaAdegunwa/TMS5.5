using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.AutoMapper;
using Egoal.Caches;
using Egoal.Common;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.Extensions;
using Egoal.Scenics.Dto;
using Egoal.Stadiums;
using Egoal.Stadiums.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Scenics
{
    public class ScenicAppService : ApplicationService, IScenicAppService
    {
        private readonly ScenicOptions _scenicOptions;
        private readonly IRepository<Park> _parkRepository;
        private readonly IRepository<Ground> _groundRepository;
        private readonly IRepository<GateGroup> _gateGroupRepository;
        private readonly IRepository<Gate> _gateRepository;
        private readonly IRepository<SalePoint> _salePointRepository;
        private readonly IGroundDateChangCiSaleNumRepository _groundDateChangCiSaleNumRepository;
        private readonly IRepository<GroundRemoteBookRecord, long> _groundRemoteBookRecordRepository;
        private readonly IRepository<Scenic> _scenicRepository;
        private readonly IChangCiRepository _changCiRepository;
        private readonly IRepository<TicketType> _ticketTypeRepository;
        private readonly ITicketTypeDomainService _ticketTypeDomainService;
        private readonly IStadiumDomainService _stadiumDomainService;
        private readonly ITicketTypeQueryAppService _ticketTypeQueryAppService;
        private readonly INameCacheService _nameCacheService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ScenicAppService(
            IOptions<ScenicOptions> scenicOptions,
            IRepository<Park> parkRepository,
            IRepository<Ground> groundRepository,
            IRepository<GateGroup> gateGroupRepository,
            IRepository<Gate> gateRepository,
            IRepository<SalePoint> salePointRepository,
            IGroundDateChangCiSaleNumRepository groundDateChangCiSaleNumRepository,
            IRepository<GroundRemoteBookRecord, long> groundRemoteBookRecordRepository,
            IRepository<Scenic> scenicRepository,
            IChangCiRepository changCiRepository,
            IRepository<TicketType> ticketTypeRepository,
            ITicketTypeDomainService ticketTypeDomainService,
            IStadiumDomainService stadiumDomainService,
            ITicketTypeQueryAppService ticketTypeQueryAppService,
            INameCacheService nameCacheService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _scenicOptions = scenicOptions.Value;
            _parkRepository = parkRepository;
            _groundRepository = groundRepository;
            _gateGroupRepository = gateGroupRepository;
            _gateRepository = gateRepository;
            _salePointRepository = salePointRepository;
            _groundDateChangCiSaleNumRepository = groundDateChangCiSaleNumRepository;
            _groundRemoteBookRecordRepository = groundRemoteBookRecordRepository;
            _scenicRepository = scenicRepository;
            _changCiRepository = changCiRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketTypeDomainService = ticketTypeDomainService;
            _stadiumDomainService = stadiumDomainService;
            _ticketTypeQueryAppService = ticketTypeQueryAppService;
            _nameCacheService = nameCacheService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<ScenicDto> GetScenicAsync()
        {
            var scenicDto = new ScenicDto();

            var scenic = await _scenicRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id > 0);
            if (scenic != null)
            {
                scenic.MapTo(scenicDto);

                if (!scenic.Photos.IsNullOrEmpty())
                {
                    var photos = scenic.Photos.JsonToObject<List<PhotoDto>>();
                    scenicDto.PhotoList.AddRange(photos);
                }
            }
            scenicDto.PageLabelMainText = _scenicOptions.PageLabelMainText;

            return scenicDto;
        }

        public async Task EditScenicAsync(ScenicDto input)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var scenic = await _scenicRepository.FirstOrDefaultAsync(s => s.Id > 0);
                if (scenic == null)
                {
                    scenic = new Scenic();
                }

                input.MapTo(scenic);
                scenic.OpenTime = input.OpenTime ?? string.Empty;
                scenic.CloseTime = input.CloseTime ?? string.Empty;
                scenic.Photos = input.PhotoList.ToJson();

                await _scenicRepository.InsertOrUpdateAsync(scenic);

                await uow.CompleteAsync();
            }

            _scenicOptions.ScenicName = input.ScenicName;
            _scenicOptions.ParkOpenTime = input.OpenTime;
            _scenicOptions.ParkCloseTime = input.CloseTime;
        }

        public async Task<List<BookGroundChangCiOutput>> BookGroundChangCiAsync(BookGroundChangCiInput input)
        {
            var outputs = new List<BookGroundChangCiOutput>();

            var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == input.TicketTypeId);
            var quantity = input.Quantity;
            if (ticketType.CheckTypeId == CheckType.家庭套票)
            {
                quantity = quantity * ticketType.CheckNum.Value;
            }

            if (!input.GroundChangCis.IsNullOrEmpty())
            {
                foreach (var groundChangCi in input.GroundChangCis)
                {
                    outputs.Add(await BookGroundChangCiAsync(groundChangCi.GroundId, input.Date, groundChangCi.ChangCiId, quantity, input.ListNo, input.IsRemote, input.SeatTypeId));
                }
                return outputs;
            }

            if (input.GroundId.HasValue && input.ChangCiId.HasValue)
            {
                outputs.Add(await BookGroundChangCiAsync(input.GroundId.Value, input.Date, input.ChangCiId.Value, quantity, input.ListNo, input.IsRemote, input.SeatTypeId));
                return outputs;
            }

            var groundChangCis = await _ticketTypeQueryAppService.GetTicketTypeChangCiComboboxItemsAsync(input.TicketTypeId, input.Date.To<DateTime>());
            if (groundChangCis.IsNullOrEmpty())
            {
                return outputs;
            }

            foreach (var groundChangCi in groundChangCis)
            {
                if (groundChangCi.ChangCis.IsNullOrEmpty())
                {
                    throw new UserFriendlyException($"{groundChangCi.GroundName}座位数量不足");
                }

                var changCiId = await _ticketTypeDomainService.GetChangCiIdAsync(ticketType.Id, groundChangCi.GroundId);
                if (changCiId.HasValue)
                {
                    if (!groundChangCi.ChangCis.Any(c => c.Value == changCiId.Value))
                    {
                        throw new UserFriendlyException($"{groundChangCi.GroundName}座位数量不足");
                    }

                    var output = await BookGroundChangCiAsync(groundChangCi.GroundId, input.Date, changCiId.Value, quantity, input.ListNo, input.IsRemote);
                    outputs.Add(output);
                }
                else if (!input.StartTime.IsNullOrEmpty())
                {
                    var changCiPlan = await _changCiRepository.GetChangCiPlanAsync(input.Date, groundChangCi.GroundId, input.StartTime);
                    if (changCiPlan.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException($"场次：{input.StartTime}未定义");
                    }

                    var changCi = changCiPlan.First();

                    if (!groundChangCi.ChangCis.Any(c => c.Value == changCi.ChangCiId))
                    {
                        throw new UserFriendlyException($"{groundChangCi.GroundName}座位数量不足");
                    }

                    var output = await BookGroundChangCiAsync(groundChangCi.GroundId, input.Date, changCi.ChangCiId, quantity, input.ListNo, input.IsRemote);
                    outputs.Add(output);
                }
                else
                {
                    bool bookChangCiSuccess = false;
                    foreach (var changCi in groundChangCi.ChangCis)
                    {
                        try
                        {
                            var output = await BookGroundChangCiAsync(groundChangCi.GroundId, input.Date, changCi.Value, quantity, input.ListNo, input.IsRemote);
                            outputs.Add(output);
                            bookChangCiSuccess = true;

                            break;
                        }
                        catch (UserFriendlyException)
                        {
                            if (groundChangCi.HasGroundSeat)
                            {
                                throw;
                            }

                            continue;
                        }
                    }
                    if (!bookChangCiSuccess)
                    {
                        throw new UserFriendlyException($"{groundChangCi.GroundName}座位数量不足");
                    }
                }
            }

            return outputs;
        }

        public async Task<BookGroundChangCiOutput> BookGroundChangCiAsync(int groundId, string date, int changCiId, int quantity, string listNo, bool isRemote = false, int? seatTypeId = null)
        {
            var output = new BookGroundChangCiOutput();

            var changCi = await _changCiRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(c => c.Id == changCiId);
            var ground = await _groundRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(g => g.Id == groundId);
            if (ground.SeatSaleFlag == true)
            {
                SeatingInput seatingInput = new SeatingInput();
                seatingInput.ListNo = listNo;
                seatingInput.StadiumId = ground.StadiumId.Value;
                seatingInput.SeatTypeId = seatTypeId;
                seatingInput.Date = date;
                seatingInput.ChangCiId = changCiId;
                seatingInput.Quantity = quantity;

                var seatIds = await _stadiumDomainService.SeatingAsync(seatingInput);
                output.Seats = seatIds.Select(s => new NameValue<long> { Value = s, Name = _nameCacheService.GetSeatName(s) }).ToList();

                output.HasGroundSeat = true;
            }
            else if (ground.ChangCiSaleFlag == true)
            {
                GroundDateChangCiSaleNum groundDateChangCiSaleNum = new GroundDateChangCiSaleNum();
                groundDateChangCiSaleNum.GroundId = groundId;
                groundDateChangCiSaleNum.Date = date;
                groundDateChangCiSaleNum.ChangCiId = changCiId;
                groundDateChangCiSaleNum.SaleNum = quantity;

                int totalNum = ground.SeatNum ?? 0;
                if (changCi != null && changCi.ChangCiNum.HasValue && changCi.ChangCiNum.Value > 0)
                {
                    totalNum = changCi.ChangCiNum.Value - changCi.ReservedNum;
                }

                if (!await _groundDateChangCiSaleNumRepository.SaleAsync(groundDateChangCiSaleNum, totalNum))
                {
                    throw new UserFriendlyException($"{ground.Name}剩余座位数量不足");
                }

                if (isRemote)
                {
                    var record = new GroundRemoteBookRecord();
                    record.ListNo = listNo;
                    record.Date = date;
                    record.GroundId = groundId;
                    record.ChangCiId = changCiId;
                    record.Quantity = quantity;

                    await _groundRemoteBookRecordRepository.InsertAsync(record);
                }


                output.HasGroundChangCi = true;
            }

            output.GroundId = groundId;
            output.GroundName = _nameCacheService.GetGroundName(groundId);
            output.ChangCiId = changCiId;
            if (changCi != null)
            {
                output.ChangCiName = $"{changCi.Stime}-{changCi.Etime}";
            }
            else
            {
                output.ChangCiName = _nameCacheService.GetChangCiName(changCiId);
            }

            return output;
        }

        public async Task CancelGroundChangCiAsync(CancelGroundChangCiInput input)
        {
            if (input.IsRemote)
            {
                await CancelGroundChangCiRemoteAsync(input);
                return;
            }

            if (input.HasGroundSeat)
            {
                await _stadiumDomainService.CancelSeatingAsync(input.ListNo, input.TicketId, input.Quantity);
            }
            else if (input.HasGroundChangCi)
            {
                var groundDateChangCiSaleNum = await _groundDateChangCiSaleNumRepository.FirstOrDefaultAsync(g =>
                g.GroundId == input.GroundId &&
                g.Date == input.Date &&
                g.ChangCiId == input.ChangCiId);
                groundDateChangCiSaleNum.SaleNum -= input.Quantity;
            }
        }

        private async Task CancelGroundChangCiRemoteAsync(CancelGroundChangCiInput input)
        {
            await _stadiumDomainService.CancelSeatingAsync(input.ListNo, input.TicketId, input.Quantity);

            var records = await _groundRemoteBookRecordRepository.GetAllListAsync(r => r.ListNo == input.ListNo);
            foreach (var record in records)
            {
                if (record.IsCanceled) continue;

                var groundDateChangCiSaleNum = await _groundDateChangCiSaleNumRepository.FirstOrDefaultAsync(g =>
                g.GroundId == record.GroundId &&
                g.Date == record.Date &&
                g.ChangCiId == record.ChangCiId);
                groundDateChangCiSaleNum.SaleNum -= record.Quantity;

                record.IsCanceled = true;
            }
        }

        public ScenicOptions GetScenicOptions()
        {
            return _scenicOptions;
        }

        public async Task<List<ComboboxItemDto<int>>> GetParkComboboxItemsAsync()
        {
            var query = _parkRepository.GetAll()
                .OrderBy(p => p.SortCode)
                .Select(p => new ComboboxItemDto<int>
                {
                    Value = p.Id,
                    DisplayText = p.Name
                });

            var parks = await _parkRepository.ToListAsync(query);

            var defaultParks = typeof(DefaultPark).ToComboboxItems();

            return parks.Union(defaultParks, new ComboboxItemDtoComparer<int>()).ToList();
        }

        public async Task<List<ComboboxItemDto<int>>> GetGroundComboboxItemsAsync()
        {
            var query = _groundRepository.GetAll()
                .OrderBy(g => g.SortCode)
                .Select(g => new ComboboxItemDto<int>
                {
                    Value = g.Id,
                    DisplayText = g.Name
                });

            return await _groundRepository.ToListAsync(query);
        }

        public async Task<List<ComboboxItemDto<int>>> GetGateGroupComboboxItemsAsync(int? groundId)
        {
            var query = _gateGroupRepository.GetAll()
                .WhereIf(groundId.HasValue, g => g.GroundId == groundId)
                .OrderBy(g => g.SortCode)
                .Select(g => new ComboboxItemDto<int>
                {
                    Value = g.Id,
                    DisplayText = g.Name
                });

            return await _gateGroupRepository.ToListAsync(query);
        }

        public async Task<List<ComboboxItemDto<int>>> GetSalePointComboboxItemsAsync(int? parkId)
        {
            var query = _salePointRepository.GetAll()
                .Where(s => s.SalePointType == SalePointType.售票点)
                .WhereIf(parkId.HasValue, s => s.ParkId == parkId)
                .OrderBy(g => g.SortCode)
                .Select(g => new ComboboxItemDto<int>
                {
                    Value = g.Id,
                    DisplayText = g.Name
                });

            var salePoints = await _salePointRepository.ToListAsync(query);

            var defaultSalePoints = typeof(DefaultSalePoint).ToComboboxItems();

            return salePoints.Union(defaultSalePoints, new ComboboxItemDtoComparer<int>()).ToList();
        }

        public async Task<List<ComboboxItemDto<int>>> GetGateComboBoxItemsAsync()
        {
            var query = _gateRepository.GetAll()
                .OrderBy(g => g.SortCode)
                .Select(g => new ComboboxItemDto<int>
                {
                    Value = g.Id,
                    DisplayText = g.Name
                });

            return await _gateRepository.ToListAsync(query);
        }

        public bool GetWxTouristNeedCertTypeFlag()
        {
            bool wxTouristNeedCertTypeFlag = _scenicOptions.WxTouristNeedCertTypeFlag == "1" ? true : false;
            return wxTouristNeedCertTypeFlag;
        }

        public ScenicObjectDto GetScenicObject()
        {
            ScenicObjectDto scenicObjectDto = new ScenicObjectDto();
            scenicObjectDto.ScenicName = _scenicOptions.ScenicName;
            scenicObjectDto.Copyright = _scenicOptions.Copyright;
            scenicObjectDto.ScenicObject = _scenicOptions.ScenicObject;
            scenicObjectDto.PageLabelMainText = _scenicOptions.PageLabelMainText;
            return scenicObjectDto;
        }
    }
}
