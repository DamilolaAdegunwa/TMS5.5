using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Authorization;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.Excel;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Orders;
using Egoal.Orders.Dto;
using Egoal.Payment;
using Egoal.Runtime.Session;
using Egoal.Scenics;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.Trades;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class QueryTicketAppService : ApplicationService
    {
        private readonly ScenicOptions _scenicOptions;

        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly ITicketSaleBuyerRepository _ticketSaleBuyerRepository;
        private readonly ITicketSaleSeatRepository _ticketSaleSeatRepository;
        private readonly IRepository<TicketSalePhoto, long> _ticketSalePhotoRepository;
        private readonly ITicketCheckRepository _ticketCheckRepository;
        private readonly ITicketCheckDayStatRepository _ticketCheckDayStatRepository;
        private readonly IRepository<TicketReprintLog, long> _ticketReprintLogRepository;
        private readonly IRepository<Trade, Guid> _tradeRepository;
        private readonly ITicketExchangeHistoryRepository _ticketExchangeHistoryRepository;
        private readonly ITicketConsumeRepository _ticketConsumeRepository;
        private readonly IRepository<TicketGroundCache, long> _ticketGroundCacheRepository;
        private readonly IRepository<Ground> _groundRepository;
        private readonly IRepository<TicketType> _ticketTypeRepository;
        private readonly INameCacheService _nameCacheService;
        private readonly IScenicAppService _scenicAppService;
        private readonly IPayTypeAppService _payTypeAppService;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly IRightDomainService _rightDomainService;
        private readonly ISession _session;

        public QueryTicketAppService(
            IOptions<ScenicOptions> scenicOptions,
            ITicketSaleRepository ticketSaleRepository,
            ITicketSaleBuyerRepository ticketSaleBuyerRepository,
            ITicketSaleSeatRepository ticketSaleSeatRepository,
            IRepository<TicketSalePhoto, long> ticketSalePhotoRepository,
            ITicketCheckRepository ticketCheckRepository,
            ITicketCheckDayStatRepository ticketCheckDayStatRepository,
            IRepository<TicketReprintLog, long> ticketReprintLogRepository,
            IRepository<Trade, Guid> tradeRepository,
            ITicketExchangeHistoryRepository ticketExchangeHistoryRepository,
            ITicketConsumeRepository ticketConsumeRepository,
            IRepository<TicketGroundCache, long> ticketGroundCacheRepository,
            IRepository<Ground> groundRepository,
            IRepository<TicketType> ticketTypeRepository,
            INameCacheService nameCacheService,
            IScenicAppService scenicAppService,
            IPayTypeAppService payTypeAppService,
            ITicketSaleDomainService ticketSaleDomainService,
            IRightDomainService rightDomainService,
            ISession session)
        {
            _scenicOptions = scenicOptions.Value;

            _ticketSaleRepository = ticketSaleRepository;
            _ticketSaleBuyerRepository = ticketSaleBuyerRepository;
            _ticketSaleSeatRepository = ticketSaleSeatRepository;
            _ticketSalePhotoRepository = ticketSalePhotoRepository;
            _ticketCheckRepository = ticketCheckRepository;
            _ticketCheckDayStatRepository = ticketCheckDayStatRepository;
            _ticketReprintLogRepository = ticketReprintLogRepository;
            _tradeRepository = tradeRepository;
            _ticketExchangeHistoryRepository = ticketExchangeHistoryRepository;
            _ticketConsumeRepository = ticketConsumeRepository;
            _ticketGroundCacheRepository = ticketGroundCacheRepository;
            _groundRepository = groundRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _nameCacheService = nameCacheService;
            _scenicAppService = scenicAppService;
            _payTypeAppService = payTypeAppService;
            _ticketSaleDomainService = ticketSaleDomainService;
            _rightDomainService = rightDomainService;
            _session = session;
        }

        public List<ComboboxItemDto<int>> GetTicketStatusComboboxItems()
        {
            var items = typeof(TicketStatus).ToComboboxItems();

            items.RemoveAll(item => item.Value == (int)TicketStatus.过期);

            return items;
        }

        public async Task<byte[]> QueryTicketSalesToExcelAsync(QueryTicketSaleInput input)
        {
            input.ShouldPage = false;

            var result = await QueryTicketSalesAsync(input);

            return await ExcelHelper.ExportToExcelAsync(result.Items, "售票查询", string.Empty);
        }

        public async Task<PagedResultDto<TicketSaleListDto>> QueryTicketSalesAsync(QueryTicketSaleInput input)
        {
            var result = await _ticketSaleRepository.QueryTicketSalesAsync(input);

            foreach (var ticket in result.Items)
            {
                ticket.ValidFlagName = ticket.ValidFlag == true ? "有效" : "无效";
                ticket.PhotoBindFlagName = ticket.PhotoBindFlag == true ? "已登记" : "未登记";
                ticket.TradeSourceName = ticket.TradeSource.ToString();
                if (ticket.FingerStatusID == FingerStatus.已登记)
                {
                    ticket.FingerprintNum = await _ticketSaleRepository.GetFingerprintQuantityAsync(ticket.Id);
                    ticket.UnBindFingerprintNum = ticket.PersonNum - ticket.FingerprintNum;
                }
                if (ticket.PhotoBindFlag == true)
                {
                    ticket.PhotoBindTime = (await _ticketSaleRepository.GetFacePhotoBindTimeAsync(ticket.Id)).ToDateTimeString();
                }
                if (ticket.CashierId.HasValue && ticket.CashierName.IsNullOrEmpty())
                {
                    ticket.CashierName = _nameCacheService.GetStaffName(ticket.CashierId.Value);
                }
                if (ticket.CertNo.IsNullOrEmpty() && !input.CertNo.IsNullOrEmpty())
                {
                    ticket.CertNo = input.CertNo;
                }
                ticket.PromoterName = _nameCacheService.GetPromoterName(ticket.PromoterId);
            }

            if (result.TotalCount > 0)
            {
                var totalRow = new TicketSaleListDto();
                totalRow.RowNum = "合计";
                totalRow.PersonNum = result.Items.Sum(t => t.PersonNum);
                totalRow.RealMoney = result.Items.Sum(t => t.RealMoney);
                totalRow.TicMoney = result.Items.Sum(t => t.TicMoney);
                result.Items.Add(totalRow);
            }

            return result;
        }

        public async Task<List<TicketSaleSimpleDto>> GetTicketSalesByListNoAsync(string listNo)
        {
            var ticketDtos = new List<TicketSaleSimpleDto>();

            var tickets = await _ticketSaleRepository.GetAll()
                .AsNoTracking()
                .Where(t => t.OrderListNo == listNo && t.TicketStatusId != TicketStatus.已退)
                .ToListAsync();
            foreach (var ticket in tickets)
            {
                var ticketDto = new TicketSaleSimpleDto();
                ticketDto.TicketCode = ticket.TicketCode;
                ticketDto.TicketStatusName = ticket.TicketStatusName;
                ticketDto.TicketTypeId = ticket.TicketTypeId.Value;
                ticketDto.TicketTypeName = ticket.TicketTypeName;
                ticketDto.Quantity = ticket.PersonNum.Value;
                ticketDto.SurplusQuantity = await _ticketSaleDomainService.GetSurplusNumAsync(ticket);
                ticketDto.TotalNum = ticket.TotalNum.Value;
                ticketDto.Stime = ticket.Stime;
                ticketDto.Etime = ticket.Etime;
                ticketDto.OrderDetailId = ticket.OrderDetailId;
                ticketDto.IsUsable = await _ticketSaleDomainService.IsUsableAsync(ticket);
                ticketDto.AllowRefund = await _ticketSaleDomainService.AllowRefundAsync(ticket);
                ticketDto.ShowFace = await _ticketSaleDomainService.AllowEnrollFaceAsync(ticket) || ticket.PhotoBindFlag == true;

                ticketDtos.Add(ticketDto);
            }

            return ticketDtos;
        }

        public async Task<List<TicketSalePhotoDto>> GetTicketSalePhotosByListNoAsync(string listNo)
        {
            var ticketPhotos = new List<TicketSalePhotoDto>();

            var ticketSales = await _ticketSaleRepository.GetAll()
               .AsNoTracking()
               .Where(t => t.OrderListNo == listNo && t.TicketStatusId != TicketStatus.已退)
               .ToListAsync();

            if (ticketSales.IsNullOrEmpty()) return ticketPhotos;

            foreach (var ticketSale in ticketSales)
            {
                var ticketPhoto = await BuildTicketSalePhoto(ticketSale);
                ticketPhotos.Add(ticketPhoto);
            }

            return ticketPhotos;
        }

        public async Task<List<TicketSalePhotoDto>> GetLocalTicketsForEnrollFaceAsync()
        {
            var ticketPhotos = new List<TicketSalePhotoDto>();

            var query = from ticketSale in _ticketSaleRepository.GetAll()
                        .Where(t => t.MemberId == _session.MemberId)
                        join trade in _tradeRepository.GetAll()
                        .Where(t => t.TradeSource != TradeSource.微信)
                        on ticketSale.TradeId equals trade.Id
                        orderby ticketSale.Ctime descending
                        select ticketSale;
            var ticketSales = await query.ToListAsync();
            foreach (TicketSale ticketSale in ticketSales)
            {
                if (ticketSale.Etime.To<DateTime>() < DateTime.Now) continue;

                var ticketPhoto = await BuildTicketSalePhoto(ticketSale);
                ticketPhotos.Add(ticketPhoto);
            }

            return ticketPhotos;
        }

        public async Task<TicketSalePhotoDto> GetLocalTicketForEnrollFaceAsync(GetLocalTicketForEnrollFaceInput input)
        {
            TicketSale ticketSale = await _ticketSaleRepository.FirstOrDefaultAsync(t => t.TicketCode == input.TicketCode);
            if (ticketSale == null)
            {
                throw new UserFriendlyException("找不到二维码对应门票");
            }
            if (!await _ticketSaleDomainService.AllowEnrollFaceAsync(ticketSale))
            {
                throw new UserFriendlyException("此门票不支持登记人脸");
            }
            if (!await _tradeRepository.AnyAsync(t => t.Id == ticketSale.TradeId))
            {
                throw new UserFriendlyException("找不到此门票对应交易信息");
            }
            if (ticketSale.MemberId == _session.MemberId)
            {
                throw new UserFriendlyException("此门票您已添加");
            }
            if (ticketSale.MemberId.HasValue)
            {
                throw new UserFriendlyException("此门票已绑定微信会员");
            }

            return await BuildTicketSalePhoto(ticketSale);
        }

        private async Task<TicketSalePhotoDto> BuildTicketSalePhoto(TicketSale ticketSale)
        {
            var ticketPhoto = new TicketSalePhotoDto();
            ticketPhoto.TicketId = ticketSale.Id;
            ticketPhoto.TicketCode = ticketSale.TicketCode;
            ticketPhoto.TicketTypeName = _nameCacheService.GetTicketTypeDisplayName(ticketSale.TicketTypeId);
            ticketPhoto.TicketSaleStatusName = ticketSale.TicketStatusId.ToString();
            ticketPhoto.SurplusQuantity = await _ticketSaleDomainService.GetSurplusNumAsync(ticketSale);
            ticketPhoto.AllowEnrollFace = await _ticketSaleDomainService.AllowEnrollFaceAsync(ticketSale);

            var photos = await _ticketSalePhotoRepository.GetAll().AsNoTracking().Where(p => p.TicketId == ticketSale.Id).ToListAsync();
            foreach (var photo in photos)
            {
                ticketPhoto.Photos.Add(new { photo.Id, Url = photo.PhotoUrl });
            }

            var maxPhotoQuantity = ticketSale.CheckTypeId == CheckType.家庭套票 ? ticketPhoto.SurplusQuantity * ticketSale.GetCheckNum() : ticketPhoto.SurplusQuantity;
            ticketPhoto.MaxPhotoQuantity = Math.Max(maxPhotoQuantity, ticketPhoto.Photos.Count);

            return ticketPhoto;
        }

        public async Task<TicketSaleFullDto> GetTicketFullInfoAsync(GetTicketFullInfoInput input)
        {
            var ticketSale = await _ticketSaleRepository.GetAll()
                .AsNoTracking()
                .Include(t => t.TicketGrounds)
                .WhereIf(input.Id.HasValue, t => t.Id == input.Id)
                .WhereIf(!input.TicketCode.IsNullOrEmpty(), t => t.TicketCode == input.TicketCode)
                .WhereIf(!input.CertNo.IsNullOrEmpty(), t => t.CertNo == input.CertNo)
                .Where(t => t.TicketStatusId != TicketStatus.已退)
                .FirstOrDefaultAsync();

            if (ticketSale == null)
            {
                throw new UserFriendlyException("暂无数据");
            }

            var ticketSaleDto = new TicketSaleFullDto();
            ticketSaleDto.TicketCode = ticketSale.TicketCode;
            ticketSaleDto.ListNo = ticketSale.ListNo;
            ticketSaleDto.TicketStatusName = ticketSale.TicketStatusId.ToString();
            ticketSaleDto.TicketTypeName = _nameCacheService.GetTicketTypeName(ticketSale.TicketTypeId);
            ticketSaleDto.ReaPrice = ticketSale.ReaPrice.Value;
            ticketSaleDto.RealMoney = ticketSale.ReaMoney.Value;
            ticketSaleDto.PayTypeName = _nameCacheService.GetPayTypeName(ticketSale.PayTypeId);
            ticketSaleDto.Quantity = ticketSale.PersonNum.Value;
            ticketSaleDto.SurplusQuantity = await _ticketSaleDomainService.GetSurplusNumAsync(ticketSale);
            ticketSaleDto.TotalNum = ticketSale.TotalNum.Value;
            ticketSaleDto.Stime = ticketSale.Stime;
            ticketSaleDto.Etime = ticketSale.Etime;
            ticketSaleDto.CustomerName = _nameCacheService.GetCustomerName(ticketSale.CustomerId);
            ticketSaleDto.CashierName = _nameCacheService.GetStaffName(ticketSale.CashierId);
            ticketSaleDto.SalePointName = _nameCacheService.GetSalePointName(ticketSale.SalePointId);
            ticketSaleDto.Ctime = ticketSale.Ctime;

            var trade = await _tradeRepository.FirstOrDefaultAsync(ticketSale.TradeId);
            ticketSaleDto.ContactName = trade.ContactName;
            ticketSaleDto.ContactMobile = trade.Mobile;
            ticketSaleDto.ContactCertNo = trade.ContactCertNo;

            foreach (var ticketGround in ticketSale.TicketGrounds)
            {
                var ticketGroundDto = new TicketGroundDto();
                ticketGroundDto.Id = ticketGround.Id;
                ticketGroundDto.GroundName = _nameCacheService.GetGroundName(ticketGround.GroundId);
                ticketGroundDto.ChangCiName = _nameCacheService.GetChangCiName(ticketGround.ChangCiId);
                ticketGroundDto.SurplusNum = ticketGround.SurplusNum.Value;
                ticketGroundDto.Stime = ticketGround.Stime;
                ticketGroundDto.Etime = ticketGround.Etime;

                ticketSaleDto.Grounds.Add(ticketGroundDto);
            }

            ticketSaleDto.TicketChecks = (await QueryTicketChecksAsync(new QueryTicketCheckInput { TicketId = ticketSale.Id, ShouldPage = false, ShouldSum = false })).Items;

            var seats = await GetTicketSeatsAsync(new GetTicketSeatsInput { TicketId = ticketSale.Id });
            foreach (var seat in seats)
            {
                seat.GroundName = _nameCacheService.GetGroundName(seat.GroundId);
                seat.ChangCiName = _nameCacheService.GetChangCiName(seat.ChangCiId);
                seat.SeatName = _nameCacheService.GetSeatName(seat.SeatId);
                seat.StadiumName = _nameCacheService.GetStadiumName(seat.StadiumId);
                seat.RegionName = _nameCacheService.GetRegionName(seat.RegionId);

                ticketSaleDto.Seats.Add(seat);
            }

            ticketSaleDto.Tourists = await GetTicketTouristsAsync(ticketSale.Id);

            return ticketSaleDto;
        }

        public async Task<List<TicketSaleSeatDto>> GetTicketSeatsAsync(GetTicketSeatsInput input)
        {
            return await _ticketSaleSeatRepository.GetTicketSeatsAsync(input);
        }

        public async Task<List<TouristDto>> GetTicketTouristsAsync(long ticketId)
        {
            var query = from tourist in _ticketSaleBuyerRepository.GetAll()
                        where tourist.TicketId == ticketId
                        select new TouristDto
                        {
                            Name = tourist.BuyerName,
                            Mobile = tourist.Mobile,
                            CertNo = tourist.CertNo,
                            Birthday = tourist.Birthday
                        };

            return await _ticketSaleBuyerRepository.ToListAsync(query);
        }

        public async Task<PagedResultDto<MemberTicketSaleDto>> GetTicketsByMemberAsync(GetTicketsByMemberInput input)
        {
            input.ETime = DateTime.Now;

            var ticketDtos = new List<MemberTicketSaleDto>();
            var tickets = await _ticketSaleRepository.GetTicketsByMemberAsync(input);
            foreach (var ticket in tickets.Items)
            {
                TicketType ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.Id == ticket.TicketTypeId);
                var ticketDto = new MemberTicketSaleDto();
                ticketDto.TicketCode = ticket.TicketCode;
                ticketDto.TicketTypeName = _nameCacheService.GetTicketTypeDisplayName(ticket.TicketTypeId);
                ticketDto.StatusName = ticket.TicketStatusName;
                ticketDto.StartDate = ticket.Stime.Substring(0, 10);
                ticketDto.EndDate = ticket.Etime.Substring(0, 10);
                ticketDto.CTime = ticket.Ctime.Value.ToDateTimeString();
                ticketDto.ListNo = ticket.OrderListNo;
                if (ticketType != null)
                {
                    ticketDto.WxShowQrCode = ticketType.WxShowQrCode;
                }
                ticketDtos.Add(ticketDto);

                var seats = await GetTicketSeatsAsync(new GetTicketSeatsInput { TicketId = ticket.Id });

                var ticketGrounds = await _ticketGroundCacheRepository.GetAll().Where(g => g.TicketId == ticket.Id && g.ChangCiId != null).ToListAsync();
                foreach (var ticketGround in ticketGrounds)
                {
                    var groundChangCi = new MemberTicketSaleDto.GroundChangCi();
                    groundChangCi.GroundName = _nameCacheService.GetGroundName(ticketGround.GroundId);
                    groundChangCi.ChangCiName = _nameCacheService.GetChangCiName(ticketGround.ChangCiId);
                    ticketDto.AddGroundChangCi(groundChangCi);

                    groundChangCi.Seats = seats
                        .Where(s => s.GroundId == ticketGround.GroundId && s.ChangCiId == ticketGround.ChangCiId)
                        .Select(s => _nameCacheService.GetSeatName(s.SeatId))
                        .ToList();
                }
            }

            return new PagedResultDto<MemberTicketSaleDto>(tickets.TotalCount, ticketDtos);
        }

        public async Task<CheckTicketOutput> GetLastCheckTicketInfoAsync(GetLastCheckTicketInfoInput input)
        {
            var startCtime = DateTime.Now.Date;
            var endCtime = DateTime.Now;

            var output = await _ticketCheckRepository.GetAll()
                .Where(t => t.Ctime >= startCtime && t.Ctime <= endCtime)
                .Where(t => t.CheckerId == input.StaffId)
                .Where(t => t.GateId == input.GateId)
                .OrderByDescending(t => t.Ctime)
                .Select(t => new CheckTicketOutput
                {
                    CardNo = t.CardNo,
                    TicketTypeId = t.TicketTypeId,
                    Stime = t.Stime,
                    Etime = t.Etime,
                    TotalNum = t.TotalNum,
                    CheckNum = t.CheckNum,
                    SurplusNum = t.SurplusNum,
                    GroundId = t.GroundId,
                    CheckerId = t.CheckerId,
                    CheckTime = t.Ctime
                })
                .FirstOrDefaultAsync();

            if (output == null)
            {
                throw new UserFriendlyException("暂无数据");
            }

            output.TicketTypeName = _nameCacheService.GetTicketTypeName(output.TicketTypeId);
            output.GroundName = _nameCacheService.GetGroundName(output.GroundId);
            output.CheckerName = _nameCacheService.GetStaffName(output.CheckerId);

            return output;
        }

        public async Task<DynamicColumnResultDto> StatTouristByAgeRangeAsync(StatTouristByAgeRangeInput input)
        {
            var data = await _ticketSaleBuyerRepository.StatTouristByAgeRangeAsync(input);
            data.ColumnSum();

            return new DynamicColumnResultDto(data);
        }

        public async Task<DynamicColumnResultDto> StatTouristByAgeRangeSimpleAsync(StatTouristByAgeRangeInput input)
        {
            DataTable dataTable = await _ticketSaleBuyerRepository.StatTouristByAgeRangeSimpleAsync(input);

            return new DynamicColumnResultDto(dataTable);
        }

        public async Task<DynamicColumnResultDto> StatTouristByAreaAsync(StatTouristByAreaInput input)
        {
            var data = await _ticketSaleBuyerRepository.StatTouristByAreaAsync(input);
            data.ColumnSum();

            return new DynamicColumnResultDto(data);
        }

        public async Task<DynamicColumnResultDto> StatTouristBySexAsync(StatTouristBySexInput input)
        {
            var data = await _ticketSaleBuyerRepository.StatTouristBySexAsync(input);

            decimal total = 0;
            foreach (DataRow row in data.Rows)
            {
                decimal.TryParse(row["人数"].ToString(), out decimal value);
                total += value;
            }

            DataColumn newColumn = new DataColumn();
            newColumn.ColumnName = "比例";
            data.Columns.Add(newColumn);
            foreach (DataRow row in data.Rows)
            {
                decimal.TryParse(row["人数"].ToString(), out decimal value);
                if (value > 0)
                {
                    row["比例"] = $"{(value / total * 100).ToString("F2")}%";
                }
            }

            return new DynamicColumnResultDto(data);
        }

        public async Task<PagedResultDto<TicketCheckListDto>> QueryTicketChecksAsync(QueryTicketCheckInput input)
        {
            var result = await _ticketCheckRepository.QueryTicketChecksAsync(input);

            if (input.ShouldSum && result.TotalCount > 0)
            {
                var total = new TicketCheckListDto();
                total.RowNum = "合计";
                total.CheckNum = result.Items.Sum(t => t.CheckNum);
                result.Items.Add(total);
            }

            return result;
        }

        /// <summary>
        /// 检票入园统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DynamicColumnResultDto> StatTicketCheckInAsync(StatTicketCheckInInput input)
        {
            if (_scenicOptions.IsMultiPark)
            {
                return await StatTicketCheckInByParkAsync(input);
            }

            DataTable data = null;
            if (input.StatType == 1)
            {
                if (input.IfByGround)
                {
                    data = await _ticketCheckRepository.StatTicketCheckDayInByTimeAsync(input);
                }
                else
                {
                    data = await _ticketCheckRepository.StatTicketCheckInByTimeAsync(input);
                }

                for (int i = 1; i < data.Columns.Count - 1; i++)
                {
                    if (int.TryParse(data.Columns[i].ColumnName, out int value))
                    {
                        data.Columns[i].ColumnName = $"{data.Columns[i].ColumnName}-{(value + 1).ToString().PadLeft(2, '0')}";
                    }
                }
                data.ColumnSum();
            }
            else
            {
                data = await _ticketCheckRepository.StatTicketCheckInAsync(input);
                if (input.StatType == 2)
                {
                    string[] weeks = new string[] { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };
                    var rows = data.Rows.Cast<DataRow>().OrderBy(row => Array.IndexOf(weeks, row["星期"].ToString()));
                    var newTable = data.Clone();
                    foreach (var row in rows)
                    {
                        newTable.ImportRow(row);
                    }
                    data = newTable;
                }
            }

            return new DynamicColumnResultDto(data);
        }

        /// <summary>
        /// 客流按日期统计导出Excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<byte[]> StatTicketCheckInToExcelAsync(StatTicketCheckInInput input)
        {
            DynamicColumnResultDto dynamicColumnResultDto = await StatTicketCheckInAsync(input);
            return await ExcelHelper.ExportToExcelAsync(dynamicColumnResultDto.Data, "客流按日期统计", "");
        }

        private async Task<DynamicColumnResultDto> StatTicketCheckInByParkAsync(StatTicketCheckInInput input)
        {
            DataTable data = null;
            if (input.StatType == 1)
            {
                data = await _ticketCheckRepository.StatTicketCheckInByParkAndTimeAsync(input);
                for (int i = 1; i < data.Columns.Count - 1; i++)
                {
                    if (int.TryParse(data.Columns[i].ColumnName, out int value))
                    {
                        data.Columns[i].ColumnName = $"{data.Columns[i].ColumnName}-{(value + 1).ToString().PadLeft(2, '0')}";
                    }
                }
                data.ColumnSum();
            }
            else
            {
                data = await _ticketCheckRepository.StatTicketCheckInByParkAsync(input);
                data.ColumnSum();
            }

            data.Columns["景点"].ReadOnly = false;
            foreach (DataRow row in data.Rows)
            {
                row["景点"] = _nameCacheService.GetParkName(Convert.ToInt32(row["景点"]));
            }

            return new DynamicColumnResultDto(data);
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckInByDateAndParkAsync(StatTicketCheckInInput input)
        {
            var parks = await _scenicAppService.GetParkComboboxItemsAsync();

            var table = await _ticketCheckRepository.StatTicketCheckInByDateAndParkAsync(input, parks);

            foreach (DataColumn column in table.Columns)
            {
                if (int.TryParse(column.ColumnName, out int parkId))
                {
                    column.ColumnName = _nameCacheService.GetParkName(parkId);
                }
            }

            table.ColumnSum();
            table.RowSum();

            return new DynamicColumnResultDto(table);
        }

        public async Task<DataTable> StatTicketCheckInByGateGroupAsync(StatTicketCheckInInput input)
        {
            var table = await _ticketCheckRepository.StatTicketCheckInByGateGroupAsync(input);

            table.Columns.Add("ParkName");
            table.Columns.Add("GateGroupName");
            table.Columns.Add("GateName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["ParkID"].ToString(), out int parkId))
                {
                    row["ParkName"] = _nameCacheService.GetParkName(parkId);
                }
                if (int.TryParse(row["GateGroupID"].ToString(), out int gateGroupId))
                {
                    row["GateGroupName"] = _nameCacheService.GetGateGroupName(gateGroupId);
                }
                if (int.TryParse(row["GateID"].ToString(), out int gateId))
                {
                    row["GateName"] = _nameCacheService.GetGateName(gateId);
                }
            }
            table.Columns.Remove("ParkID");
            table.Columns.Remove("GateGroupID");
            table.Columns.Remove("GateID");

            return table;
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckInByGroundAndGateGroupAsync(StatTicketCheckInInput input)
        {
            var table = await _ticketCheckRepository.StatTicketCheckInByGroundAndGateGroupAsync(input);

            table.Columns.Add("GroundName");
            table.Columns.Add("GateGroupName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["GroundID"].ToString(), out int groundId))
                {
                    row["GroundName"] = _nameCacheService.GetGroundName(groundId);
                }
                if (int.TryParse(row["GateGroupID"].ToString(), out int gateGroupId))
                {
                    row["GateGroupName"] = _nameCacheService.GetGateGroupName(gateGroupId);
                }
            }
            table.Columns.Remove("GroundID");
            table.Columns.Remove("GateGroupID");
            table.Columns["GroundName"].SetOrdinal(0);
            table.Columns["GateGroupName"].SetOrdinal(1);

            if (!table.IsNullOrEmpty())
            {
                table.RowSum(0, "合计", "GateGroupName");
            }

            return new DynamicColumnResultDto(table);
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckByGroundAndTimeAsync(StatTicketCheckInInput input)
        {
            var table = await _ticketCheckRepository.StatTicketCheckByGroundAndTimeAsync(input);

            table.Columns["检票区域"].ReadOnly = false;
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["检票区域"].ToString(), out int groundId))
                {
                    row["检票区域"] = _nameCacheService.GetGroundName(groundId);
                }
            }

            return new DynamicColumnResultDto(table);
        }

        public async Task<DynamicColumnResultDto> StatStadiumTicketCheckInAsync(StatTicketCheckInInput input)
        {
            var data = await _ticketCheckDayStatRepository.StatStadiumTicketCheckInAsync(input);

            return new DynamicColumnResultDto(data);
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckByTradeSourceAsync(StatTicketCheckInInput input)
        {
            var data = await _ticketCheckRepository.StatTicketCheckByTradeSourceAsync(input);
            data.RemoveEmptyColumn();
            data.ColumnSum();

            return new DynamicColumnResultDto(data);
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckInByTicketTypeAsync(StatTicketCheckInInput input)
        {
            var table = await _ticketCheckRepository.StatTicketCheckInByTicketTypeAsync(input);
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("TicketTypeID");

            return new DynamicColumnResultDto(table);
        }

        /// <summary>
        /// 检票年度对比统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DynamicColumnResultDto> StatTicketCheckInYearOverYearComparisonAsync(StatTicketCheckInInput input)
        {
            var today = DateTime.Now.Date;
            input.StartCTime = new DateTime(today.Year - input.StatType, 1, 1);
            input.EndCTime = new DateTime(today.Year, 12, 31);

            var data = await _ticketCheckDayStatRepository.StatTicketCheckInYearOverYearComparisonAsync(input);
            data.Columns["月份"].ReadOnly = false;
            foreach (DataRow row in data.Rows)
            {
                int.TryParse(row["月份"].ToString(), out int month);
                row["月份"] = $"{month}月";
            }

            return new DynamicColumnResultDto(data);
        }

        public async Task<TicketCheckOverviewResult> GetTicketCheckOverviewAsync(GetTicketCheckOverviewInput input)
        {
            var overview = new TicketCheckOverviewResult();
            overview.StadiumOverview = await GetStadiumTicketCheckOverviewAsync(input);
            overview.ScenicCheckInQuantity = await _ticketCheckDayStatRepository.GetScenicCheckInQuantityAsync(input.StartDate, input.EndDate);
            overview.ScenicCheckOutQuantity = await _ticketCheckDayStatRepository.GetScenicCheckOutQuantityAsync(input.StartDate, input.EndDate);

            overview.ScenicRealTimeQuantity = overview.ScenicCheckInQuantity - overview.ScenicCheckOutQuantity;
            int stadiumRealTimeQuantity = overview.StadiumOverview.Rows.Cast<DataRow>().Sum(r => Convert.ToInt32(r["RealTime"]));
            if (overview.ScenicRealTimeQuantity < stadiumRealTimeQuantity)
            {
                overview.ScenicRealTimeQuantity = stadiumRealTimeQuantity;
            }
            if (overview.ScenicRealTimeQuantity < 0)
            {
                overview.ScenicRealTimeQuantity = 0;
            }
            if (overview.ScenicCheckInQuantity < overview.ScenicRealTimeQuantity)
            {
                overview.ScenicCheckInQuantity = overview.ScenicRealTimeQuantity;
            }

            string closeTime = string.IsNullOrEmpty(_scenicOptions.ParkCloseTime) ? ("23:59") : _scenicOptions.ParkCloseTime;
            var parkCloseTime = $"{DateTime.Now.Date.ToDateString()} { closeTime }:00".To<DateTime>();
            if (DateTime.Now > parkCloseTime)
            {
                overview.ScenicRealTimeQuantity = 0;
            }

            return overview;
        }

        public async Task<DataTable> GetStadiumTicketCheckOverviewAsync(GetTicketCheckOverviewInput input)
        {
            var stadiumOverview = await _ticketCheckDayStatRepository.GetStadiumTicketCheckOverviewAsync(input.StartDate, input.EndDate);
            string closeTime = string.IsNullOrEmpty(_scenicOptions.ParkCloseTime) ? ("23:59") : _scenicOptions.ParkCloseTime;

            var parkCloseTime = $"{DateTime.Now.Date.ToDateString()} { closeTime }:00".To<DateTime>();

            stadiumOverview.Columns.Add("RealTime", typeof(int));
            foreach (DataRow row in stadiumOverview.Rows)
            {
                int.TryParse(row["CheckIn"].ToString(), out int checkIn);
                int.TryParse(row["CheckOut"].ToString(), out int checkOut);
                int realTime = checkIn - checkOut;

                row["RealTime"] = realTime > 0 ? realTime : 0;
                if (DateTime.Now > parkCloseTime)
                {
                    row["RealTime"] = 0;
                }
            }

            return stadiumOverview;
        }

        public async Task<int> GetScenicCheckInQuantityAsync(GetTicketCheckOverviewInput input)
        {
            return await _ticketCheckDayStatRepository.GetScenicCheckInQuantityAsync(input.StartDate, input.EndDate);
        }

        public async Task<DynamicColumnResultDto> StatTicketCheckInAverageAsync(StatTicketCheckInInput input)
        {
            DataTable data = null;
            if (input.StatType == 1)
            {
                data = await _ticketCheckDayStatRepository.StatTicketCheckInAverageByTimeslotAsync(input);
            }
            else if (input.StatType == 2)
            {
                data = await _ticketCheckDayStatRepository.StatTicketCheckInAverageByDateAsync(input);
            }
            else
            {
                data = await _ticketCheckDayStatRepository.StatTicketCheckInAverageByMonthAsync(input);
            }

            return new DynamicColumnResultDto(data);
        }

        public async Task<byte[]> StatTicketSaleToExcelAsync(StatTicketSaleInput input)
        {
            var items = await StatTicketSaleAsync(input);

            return await ExcelHelper.ExportToExcelAsync(
                items,
                "售票统计",
                string.Empty,
                new Dictionary<string, Func<string>>
                {
                    { "StatType", () => input.StatType.ToString() }
                });
        }

        public async Task<List<StatTicketSaleListDto>> StatTicketSaleAsync(StatTicketSaleInput input)
        {
            var items = await _ticketSaleRepository.StatAsync(input);

            var columns = new Dictionary<TicketSaleStatType, Func<int, string>>
            {
                { TicketSaleStatType.票类, id => _nameCacheService.GetTicketTypeName(id) },
                { TicketSaleStatType.购买类型, id => ((TradeSource)id).ToString() }
            };
            if (columns.ContainsKey(input.StatType))
            {
                foreach (var item in items)
                {
                    if (int.TryParse(item.StatType, out int id))
                    {
                        item.StatType = columns[input.StatType](id);
                    }
                }
            }

            if (!items.IsNullOrEmpty())
            {
                items.Add(new StatTicketSaleListDto
                {
                    StatType = "合计",
                    SaleNum = items.Sum(i => i.SaleNum),
                    SalePersonNum = items.Sum(i => i.SalePersonNum),
                    SaleMoney = items.Sum(i => i.SaleMoney),
                    ReturnNum = items.Sum(i => i.ReturnNum),
                    ReturnPersonNum = items.Sum(i => i.ReturnPersonNum),
                    ReturnMoney = items.Sum(i => i.ReturnMoney),
                    RealNum = items.Sum(i => i.RealNum),
                    RealPersonNum = items.Sum(i => i.RealPersonNum),
                    RealMoney = items.Sum(i => i.RealMoney)
                });
            }

            return items;
        }

        public List<ComboboxItemDto<int>> GetTicketSaleStatTypeComboboxItems()
        {
            var items = typeof(TicketSaleStatType).ToComboboxItems();

            return items;
        }

        public async Task<StatCashierSaleDto> StatCashierSaleAsync(StatCashierSaleInput input)
        {
            var table = await _ticketSaleRepository.StatCashierSaleAsync(input);

            table.Columns.Add("CashierName");
            List<string> payTypeIds = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["CashierID"].ToString(), out int cashierId))
                {
                    row["CashierName"] = _nameCacheService.GetStaffName(cashierId);
                }
                if (input.StatTypeId == 2)
                {
                    string payTypeId = row["PayTypeName"].ToString();
                    if (!payTypeIds.Contains(payTypeId))
                    {
                        payTypeIds.Add(payTypeId);
                    }
                }
            }
            table.Columns.Remove("CashierID");
            StatCashierSaleDto statCashierSaleDto = new StatCashierSaleDto();
            statCashierSaleDto.ResultData = table;
            statCashierSaleDto.PayTypeNum = payTypeIds.Count();

            return statCashierSaleDto;
        }

        public async Task<DataTable> StatPromoterSaleAsync(StatPromoterSaleInput input)
        {
            input.IncludeGroundRefund = await _rightDomainService.HasFeatureAsync(Guid.Parse(Permissions.TMS_TicketGroundReturn));

            var table = await _ticketSaleRepository.StatPromoterSaleAsync(input);

            table.Columns.Add("PromoterName");
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["PromoterId"].ToString(), out int promoterId))
                {
                    row["PromoterName"] = _nameCacheService.GetPromoterName(promoterId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("PromoterId");
            table.Columns.Remove("TicketTypeID");

            return table;
        }

        public async Task<DataTable> StatTicketSaleByTradeSourceAsync(StatTicketSaleByTradeSourceInput input)
        {
            input.TicketTypeSearchGroupId = _session.SearchGroupId;

            var table = await _ticketSaleRepository.StatByTradeSourceAsync(input);

            table.Columns.Add("TradeSourceName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["TradeSource"].ToString(), out int tradeSource))
                {
                    row["TradeSourceName"] = ((TradeSource)tradeSource).ToString();
                }
            }
            table.Columns.Remove("TradeSource");

            return table;
        }

        public async Task<DataTable> StatTicketSaleByTicketTypeClassAsync(StatTicketSaleByTicketTypeClassInput input)
        {
            var table = await _ticketSaleRepository.StatByTicketTypeClassAsync(input);

            table.Columns.Add("TicketTypeClassName");
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["TicketTypeClassID"].ToString(), out int ticketTypeClassId))
                {
                    row["TicketTypeClassName"] = _nameCacheService.GetTicketTypeClassName(ticketTypeClassId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("TicketTypeClassID");
            table.Columns.Remove("TicketTypeID");

            return table;
        }

        public async Task<byte[]> StatTicketSaleByPayTypeToExcelAsync(StatTicketSaleByPayTypeInput input)
        {
            var result = await StatTicketSaleByPayTypeAsync(input);

            return await ExcelHelper.ExportToExcelAsync(result.Data, "门票收款汇总", string.Empty);
        }

        public async Task<DynamicColumnResultDto> StatTicketSaleByPayTypeAsync(StatTicketSaleByPayTypeInput input)
        {
            var payTypes = await _payTypeAppService.GetPayTypeComboboxItemsAsync();

            var table = await _ticketSaleRepository.StatByPayTypeAsync(input, payTypes);
            foreach (DataColumn column in table.Columns)
            {
                if (int.TryParse(column.ColumnName, out int payTypeId))
                {
                    column.ColumnName = payTypes.FirstOrDefault(p => p.Value == payTypeId).DisplayText;
                }
            }

            DataColumn dataColumn = new DataColumn("票类");
            table.Columns.Add(dataColumn);
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row[dataColumn] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("TicketTypeID");
            dataColumn.SetOrdinal(0);

            if (!table.IsNullOrEmpty())
            {
                table.ColumnSum();
                table.RowSum();
            }

            return new DynamicColumnResultDto(table);
        }

        public async Task<DataTable> StatTicketSaleBySalePointAsync(StatTicketSaleBySalePointInput input)
        {
            var table = await _ticketSaleRepository.StatBySalePointAsync(input);

            table.Columns.Add("ParkName");
            table.Columns.Add("SalePointName");
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["ParkID"].ToString(), out int parkId))
                {
                    row["ParkName"] = _nameCacheService.GetParkName(parkId);
                }
                if (int.TryParse(row["SalePointID"].ToString(), out int salePointId))
                {
                    row["SalePointName"] = _nameCacheService.GetSalePointName(salePointId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("ParkID");
            table.Columns.Remove("SalePointID");
            table.Columns.Remove("TicketTypeID");

            return table;
        }

        public async Task<DataTable> StatTicketSaleGroundSharingAsync(StatGroundSharingInput input)
        {
            var table = await _ticketSaleRepository.StatGroundSharingAsync(input);

            table.Columns.Add("SalePointName");
            table.Columns.Add("TicketTypeName");
            table.Columns.Add("GroundName");
            table.Columns.Add("SharingPrice", typeof(decimal));
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["SalePointID"].ToString(), out int salePointId))
                {
                    row["SalePointName"] = _nameCacheService.GetSalePointName(salePointId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
                if (int.TryParse(row["GroundID"].ToString(), out int groundId))
                {
                    row["GroundName"] = _nameCacheService.GetGroundName(groundId);
                }

                if (!decimal.TryParse(row["SharingRate"].ToString(), out decimal sharingRate))
                {
                    sharingRate = 100;
                }
                sharingRate = sharingRate / 100;
                row["SharingRate"] = sharingRate;

                decimal.TryParse(row["ReaPrice"].ToString(), out decimal realPrice);
                row["SharingPrice"] = Math.Round(realPrice * sharingRate, 2);
            }
            table.Columns.Remove("SalePointID");
            table.Columns.Remove("TicketTypeID");
            table.Columns.Remove("GroundID");

            return table;
        }

        public async Task<DataTable> StatTicketSaleByCustomerAsync(StatTicketSaleByCustomerInput input)
        {
            var table = await _ticketSaleRepository.StatByCustomerAsync(input);

            table.Columns.Add("CustomerName");
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (Guid.TryParse(row["CustomerID"].ToString(), out Guid customerId))
                {
                    row["CustomerName"] = _nameCacheService.GetCustomerName(customerId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            table.Columns.Remove("CustomerID");
            table.Columns.Remove("TicketTypeID");

            return table;
        }

        public async Task<DataTable> StatTicketSaleJbAsync(StatJbInput input)
        {
            var table = await _ticketSaleRepository.StatJbAsync(input);

            if (input.StatTicketByPayType)
            {
                table.Columns.Add("PayTypeName");
            }
            table.Columns.Add("TradeTypeName");
            table.Columns.Add("TicketTypeName");
            foreach (DataRow row in table.Rows)
            {
                if (input.StatTicketByPayType && int.TryParse(row["PayTypeID"].ToString(), out int payTypeId))
                {
                    row["PayTypeName"] = _nameCacheService.GetPayTypeName(payTypeId);
                }
                if (int.TryParse(row["TradeTypeID"].ToString(), out int tradeTypeId))
                {
                    row["TradeTypeName"] = _nameCacheService.GetTradeTypeName(tradeTypeId);
                }
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
            }
            if (input.StatTicketByPayType)
            {
                table.Columns.Remove("PayTypeID");
            }
            table.Columns.Remove("TradeTypeID");
            table.Columns.Remove("TicketTypeID");

            return table;
        }

        public async Task<PagedResultDto<TicketReprintLogListDto>> QueryReprintLogsAsync(QueryReprintLogInput input)
        {
            var query = _ticketReprintLogRepository.GetAll()
                .Where(r => r.Ctime >= input.StartCTime)
                .Where(r => r.Ctime < input.EndCTime)
                .WhereIf(input.TicketTypeId.HasValue, r => r.TicketTypeId == input.TicketTypeId)
                .WhereIf(!input.TicketCode.IsNullOrEmpty(), r => r.TicketCode == input.TicketCode)
                .WhereIf(!input.CardNo.IsNullOrEmpty(), r => r.CardNo == input.CardNo)
                .WhereIf(input.CashierId.HasValue, r => r.CashierId == input.CashierId)
                .WhereIf(input.CashpcId.HasValue, r => r.CashPcid == input.CashpcId)
                .WhereIf(input.SalePointId.HasValue, r => r.SalePointId == input.SalePointId);

            var count = await _ticketReprintLogRepository.CountAsync(query);

            var resultQuery = query.OrderByDescending(r => r.Id).PageBy(input).Select(log => new TicketReprintLogListDto
            {
                Id = log.Id,
                TicketId = log.TicketId,
                TicketTypeId = log.TicketTypeId,
                TicketCode = log.TicketCode,
                CardNo = log.CardNo,
                CashierId = log.CashierId,
                CashPcid = log.CashPcid,
                SalePointId = log.SalePointId,
                ParkId = log.ParkId,
                Ctime = log.Ctime
            });

            var items = await _ticketReprintLogRepository.ToListAsync(resultQuery);
            int rowNum = input.StartRowNum;
            foreach (var item in items)
            {
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(item.TicketTypeId);
                item.CashierName = _nameCacheService.GetStaffName(item.CashierId);
                item.CashPcname = _nameCacheService.GetPcName(item.CashPcid);
                item.SalePointName = _nameCacheService.GetSalePointName(item.SalePointId);
                item.ParkName = _nameCacheService.GetParkName(item.ParkId);
                item.RowNum = rowNum;
                rowNum++;
            }

            return new PagedResultDto<TicketReprintLogListDto>(count, items);
        }

        public async Task<PagedResultDto<TicketExchangeHistoryListDto>> QueryExchangeHistorysAsync(QueryExchangeHistoryInput input)
        {
            var query = _ticketExchangeHistoryRepository.GetAll()
                .Where(e => e.Ctime >= input.StartCTime)
                .Where(e => e.Ctime < input.EndCTime)
                .WhereIf(!input.OrderListNo.IsNullOrEmpty(), e => e.OrderListNo == input.OrderListNo)
                .WhereIf(!input.OldTicketCode.IsNullOrEmpty(), e => e.OldTicketCode == input.OldTicketCode)
                .WhereIf(input.TicketTypeId.HasValue, e => e.TicketTypeId == input.TicketTypeId)
                .WhereIf(input.CashierId.HasValue, e => e.CashierId == input.CashierId)
                .WhereIf(input.SalePointId.HasValue, e => e.SalePointId == input.SalePointId);

            var count = await _ticketExchangeHistoryRepository.CountAsync(query);

            var resultQuery = query.OrderByDescending(e => e.Id).PageBy(input).Select(e => new TicketExchangeHistoryListDto
            {
                Id = e.Id,
                OrderListNo = e.OrderListNo,
                TicketTypeId = e.TicketTypeId,
                OldTicketCode = e.OldTicketCode,
                OldCardNo = e.OldCardNo,
                NewTicketCode = e.NewTicketCode,
                NewCardNo = e.NewCardNo,
                Tkid = e.Tkid,
                SalePointId = e.SalePointId,
                CashierId = e.CashierId,
                Ctime = e.Ctime
            });

            var items = await _ticketExchangeHistoryRepository.ToListAsync(resultQuery);
            int rowNum = input.StartRowNum;
            foreach (var item in items)
            {
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(item.TicketTypeId);
                item.CashierName = _nameCacheService.GetStaffName(item.CashierId);
                item.Tkname = item.Tkid?.ToString();
                item.SalePointName = _nameCacheService.GetSalePointName(item.SalePointId);
                item.RowNum = rowNum;
                rowNum++;
            }

            return new PagedResultDto<TicketExchangeHistoryListDto>(count, items);
        }

        public async Task<DataTable> StatExchangeHistoryJbAsync(StatJbInput input)
        {
            var table = await _ticketExchangeHistoryRepository.StatJbAsync(input);

            table.Columns.Add("TicketTypeName");
            table.Columns.Add("TKName");
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["TicketTypeID"].ToString(), out int ticketTypeId))
                {
                    row["TicketTypeName"] = _nameCacheService.GetTicketTypeName(ticketTypeId);
                }
                if (int.TryParse(row["TKID"].ToString(), out int tkid))
                {
                    row["TKName"] = ((TicketKind)tkid).ToString();
                }
            }
            table.Columns.Remove("TicketTypeID");
            table.Columns.Remove("TKID");

            return table;
        }

        public async Task<byte[]> StatGroundChangCiSaleToExcelAsync(StatGroundChangCiSaleInput input)
        {
            var result = await StatGroundChangCiSaleAsync(input);

            result.Data.Columns["GroundName"].ColumnName = "项目";
            result.Data.Columns["ChangCiName"].ColumnName = "场次";
            result.Data.Columns["STime"].ColumnName = "起始时间";
            result.Data.Columns["ETime"].ColumnName = "截止时间";
            result.Data.Columns["TotalNum"].ColumnName = "总数量";
            result.Data.Columns["SaleNum"].ColumnName = "已售数量";
            result.Data.Columns["SurplusNum"].ColumnName = "剩余数量";

            return await ExcelHelper.ExportToExcelAsync(result.Data, "场次座位销售统计", string.Empty);
        }

        public async Task<DynamicColumnResultDto> StatGroundChangCiSaleAsync(StatGroundChangCiSaleInput input)
        {
            var table = await _ticketSaleSeatRepository.StatGroundChangCiSaleAsync(input);

            table.Columns.Add("GroundName");
            table.Columns.Add("ChangCiName");
            table.Columns["SurplusNum"].ReadOnly = false;
            table.Columns["SurplusNum"].AllowDBNull = true;
            foreach (DataRow row in table.Rows)
            {
                if (int.TryParse(row["GroundID"].ToString(), out int groundId))
                {
                    row["GroundName"] = _nameCacheService.GetGroundName(groundId);
                }
                if (int.TryParse(row["ChangCiID"].ToString(), out int changCiId))
                {
                    row["ChangCiName"] = _nameCacheService.GetChangCiName(changCiId);
                }
                int.TryParse(row["TotalNum"].ToString(), out int totalNum);
                int.TryParse(row["SaleNum"].ToString(), out int saleNum);
                row["SurplusNum"] = totalNum - saleNum;
            }
            table.Columns.Remove("GroundID");
            table.Columns.Remove("ChangCiID");
            table.Columns["GroundName"].SetOrdinal(0);
            table.Columns["ChangCiName"].SetOrdinal(1);

            var count = await _groundRepository.GetAll().Where(g => g.ChangCiSaleFlag == true || g.SeatSaleFlag == true).CountAsync();
            if (!table.IsNullOrEmpty() && count == 1)
            {
                table.RowSum(0, "合计", "TotalNum", "SurplusNum");
            }

            return new DynamicColumnResultDto(table);
        }

        public async Task<byte[]> QueryTicketConsumesToExcelAsync(QueryTicketConsumeInput input)
        {
            input.ShouldPage = false;

            var result = await QueryTicketConsumesAsync(input);

            return await ExcelHelper.ExportToExcelAsync(result.Items, "核销查询", string.Empty);
        }

        public async Task<PagedResultDto<TicketConsumeListDto>> QueryTicketConsumesAsync(QueryTicketConsumeInput input)
        {
            var result = await _ticketConsumeRepository.QueryTicketConsumesAsync(input);
            foreach (var item in result.Items)
            {
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(item.TicketTypeId);
                item.ConsumeMoney = item.Price * item.ConsumeNum;
                item.ConsumeTypeName = item.ConsumeType.ToString();
            }

            if (result.TotalCount > 0)
            {
                var total = new TicketConsumeListDto();
                total.RowNum = "合计";
                total.ConsumeNum = result.Items.Sum(i => i.ConsumeNum);
                total.ConsumeMoney = result.Items.Sum(i => i.ConsumeMoney);

                result.Items.Add(total);
            }

            return result;
        }

        public async Task<byte[]> StatTicketConsumeToExcelAsync(StatTicketConsumeInput input)
        {
            var items = await StatTicketConsumeAsync(input);

            return await ExcelHelper.ExportToExcelAsync(items, "核销统计", string.Empty);
        }

        public async Task<List<StatTicketConsumeListDto>> StatTicketConsumeAsync(StatTicketConsumeInput input)
        {
            var items = await _ticketConsumeRepository.StatTicketConsumeAsync(input);
            foreach (var item in items)
            {
                item.CustomerName = string.IsNullOrEmpty(item.CustomerName) ? "微信散客" : item.CustomerName;
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(item.TicketTypeId);
                item.CheckMoney = item.CheckNum * item.Price;
                item.ConsumeMoney = item.ConsumeNum * item.Price;
            }

            if (!items.IsNullOrEmpty())
            {
                var total = new StatTicketConsumeListDto();
                total.CustomerName = "合计";
                total.CheckNum = items.Sum(i => i.CheckNum);
                total.CheckMoney = items.Sum(i => i.CheckMoney);
                total.ConsumeNum = items.Sum(i => i.ConsumeNum);
                total.ConsumeMoney = items.Sum(i => i.ConsumeMoney);

                items.Add(total);
            }

            return items;
        }

        public async Task<DataTable> StatTouristNumAsync(StatTouristNumInput input)
        {
            DataTable dataTable = await _ticketCheckRepository.StatTouristNumAsync(input);

            return dataTable;
        }

        public async Task<SelfHelpGetOrderTicketDto> GetSelfHelpOrderTicketByTicketSales(List<TicketSale> ticketSales, List<OrderTourist> orderTourists)
        {
            SelfHelpGetOrderTicketDto selfHelpGetOrderTicketDto = new SelfHelpGetOrderTicketDto();
            foreach (OrderTourist orderTourist in orderTourists)
            {
                List<TicketSale> touristTicketSales = await _ticketSaleRepository.GetAll().AsNoTracking().Where(a => a.OrderDetailId == orderTourist.OrderDetailId).ToListAsync();
                foreach (TicketSale touristTicketSale in touristTicketSales)
                {
                    TicketSale ticketSale = ticketSales.FirstOrDefault(a => a.Id == touristTicketSale.Id);
                    if (ticketSale == null)
                    {
                        ticketSales.Add(touristTicketSale);
                    }
                }
            }

            int rowNum = 1;
            foreach (TicketSale ticketSale in ticketSales)
            {
                int surplusNum = await _ticketSaleDomainService.GetSurplusNumAsync(ticketSale);
                bool isUsable = await _ticketSaleDomainService.IsUsableAsync(ticketSale);
                DateTime eDateTime = DateTime.Now;
                if (isUsable && surplusNum > 0 && ticketSale.TicketTypeId.HasValue && ticketSale.PersonNum.HasValue && ticketSale.Ctime.HasValue && ticketSale.ReaPrice.HasValue &&
                    DateTime.TryParse(ticketSale.Etime, out eDateTime))
                {
                    SelfHelpGetTicket selfHelpGetTicket = new SelfHelpGetTicket();
                    selfHelpGetTicket.TicketTypeId = ticketSale.TicketTypeId.Value;
                    selfHelpGetTicket.TicketTypeName = ticketSale.TicketTypeName;
                    selfHelpGetTicket.Etime = eDateTime.ToDateString();
                    selfHelpGetTicket.PersonNum = ticketSale.PersonNum.Value;
                    selfHelpGetTicket.TicketCode = ticketSale.TicketCode;
                    selfHelpGetTicket.Ctime = ticketSale.Ctime.Value.ToDateString();
                    selfHelpGetTicket.ReaMoney = ticketSale.ReaPrice.Value * surplusNum;
                    selfHelpGetTicket.SalePointName = ticketSale.SalePointName;
                    selfHelpGetTicket.RowNum = rowNum++;
                    selfHelpGetOrderTicketDto.SelfHelpGetTickets.Add(selfHelpGetTicket);
                }
            }
            return selfHelpGetOrderTicketDto;
        }

        public async Task<DataTable> StatCzkSaleAsync(StatCzkSaleInput input)
        {
            DataTable dataTable = await _ticketSaleRepository.StatCzkSaleAsync(input);
            return dataTable;
        }

        public async Task<DataTable> StatCzkSaleJbAsync(StatJbInput input)
        {
            DataTable dataTable = await _ticketSaleRepository.StatCzkSaleJbAsync(input);
            return dataTable;
        }
    }
}
