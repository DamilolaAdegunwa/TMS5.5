using Egoal.Authorization;
using Egoal.Extensions;
using Egoal.Models;
using Egoal.Mvc.Authorization;
using Egoal.Mvc.Uow;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Tickets
{
    public class TicketController : TmsControllerBase
    {
        private readonly Runtime.Session.ISession _session;
        private readonly QueryTicketAppService _queryTicketAppService;

        public TicketController(
            Runtime.Session.ISession session,
            QueryTicketAppService ticketSaleQueryAppService)
        {
            _session = session;
            _queryTicketAppService = ticketSaleQueryAppService;
        }

        [HttpGet]
        public JsonResult GetConsumeTypeComboboxItems()
        {
            var items = typeof(ConsumeType).ToComboboxItems();

            return Json(items);
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberTicketsForMobileAsync(GetTicketsByMemberInput input)
        {
            input.MemberId = _session.MemberId.Value;
            input.CustomerId = _session.CustomerId;
            input.TradeSource = Trades.TradeSource.微信;

            var tickets = await _queryTicketAppService.GetTicketsByMemberAsync(input);

            return new JsonResult(tickets);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketSalePhotosByListNoAsync(string listNo)
        {
            var result = await _queryTicketAppService.GetTicketSalePhotosByListNoAsync(listNo);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetLocalTicketsForEnrollFaceAsync()
        {
            var result = await _queryTicketAppService.GetLocalTicketsForEnrollFaceAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetLocalTicketForEnrollFaceAsync(string ticketCode)
        {
            var input = new GetLocalTicketForEnrollFaceInput();
            input.TicketCode = ticketCode;

            var result = await _queryTicketAppService.GetLocalTicketForEnrollFaceAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTicketSale)]
        public async Task<JsonResult> QueryTicketSalesAsync([FromForm] QueryTicketSaleInput input)
        {
            var result = await _queryTicketAppService.QueryTicketSalesAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTicketSale)]
        public async Task<FileContentResult> QueryTicketSalesToExcelAsync([FromForm] QueryTicketSaleInput input)
        {
            var fileContents = await _queryTicketAppService.QueryTicketSalesToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        public async Task<JsonResult> GetTicketFullInfoAsync(GetTicketFullInfoInput input)
        {
            var result = await _queryTicketAppService.GetTicketFullInfoAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        public async Task<JsonResult> GetLastCheckTicketInfoAsync(GetLastCheckTicketInfoInput input)
        {
            var result = await _queryTicketAppService.GetLastCheckTicketInfoAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTouristByAgeRangeAsync([FromForm] StatTouristByAgeRangeInput input)
        {
            var result = await _queryTicketAppService.StatTouristByAgeRangeAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTouristByAgeRangeSimpleAsync([FromForm]StatTouristByAgeRangeInput input)
        {
            var result = await _queryTicketAppService.StatTouristByAgeRangeSimpleAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTouristByAreaAsync([FromForm] StatTouristByAreaInput input)
        {
            var result = await _queryTicketAppService.StatTouristByAreaAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTouristBySexAsync([FromForm] StatTouristBySexInput input)
        {
            var result = await _queryTicketAppService.StatTouristBySexAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTicketCheck)]
        public async Task<JsonResult> QueryTicketChecksAsync([FromForm] QueryTicketCheckInput input)
        {
            var result = await _queryTicketAppService.QueryTicketChecksAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketCheckIn)]
        public async Task<JsonResult> StatTicketCheckInAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketCheckIn)]
        public async Task<FileContentResult> StatTicketCheckInToExcelAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInToExcelAsync(input);
            return Excel(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketCheckByPark)]
        public async Task<JsonResult> StatTicketCheckInByDateAndParkAsync(StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInByDateAndParkAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketCheckByGateGroup)]
        public async Task<JsonResult> StatTicketCheckInByGateGroupAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInByGateGroupAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSWeChat_TicketCheckStat)]
        public async Task<JsonResult> StatTicketCheckInByGroundAndGateGroupAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInByGroundAndGateGroupAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSWeChat_TicketCheckStat)]
        public async Task<JsonResult> StatTicketCheckByGroundAndTimeAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckByGroundAndTimeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatStadiumTicketCheckInAsync(StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatStadiumTicketCheckInAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTicketCheckByTradeSourceAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckByTradeSourceAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTicketCheckInByTicketTypeAsync([FromForm] StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInByTicketTypeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTicketCheckInYearOverYearComparisonAsync(StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInYearOverYearComparisonAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> GetTicketCheckOverviewAsync(GetTicketCheckOverviewInput input)
        {
            var result = await _queryTicketAppService.GetTicketCheckOverviewAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> GetStadiumTicketCheckOverviewAsync(GetTicketCheckOverviewInput input)
        {
            var result = await _queryTicketAppService.GetStadiumTicketCheckOverviewAsync(input);

            return new JsonResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<int> GetScenicCheckInQuantityAsync(GetTicketCheckOverviewInput input)
        {
            return await _queryTicketAppService.GetScenicCheckInQuantityAsync(input);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatTicketCheckInAverageAsync(StatTicketCheckInInput input)
        {
            var result = await _queryTicketAppService.StatTicketCheckInAverageAsync(input);

            return new JsonResult(result);
        }

        [HttpGet]
        public JsonResult GetTicketStatusComboboxItems()
        {
            var items = _queryTicketAppService.GetTicketStatusComboboxItems();

            return Json(items);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSale)]
        public async Task<FileContentResult> StatTicketSaleToExcelAsync([FromForm] StatTicketSaleInput input)
        {
            var fileContents = await _queryTicketAppService.StatTicketSaleToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false, 1000 * 60 * 5)]
        [PermissionFilter(Permissions.TMSWeChat_TicketSaleStat, Permissions.TMSAdmin_StatTicketSale)]
        public async Task<JsonResult> StatTicketSaleAsync([FromForm] StatTicketSaleInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleAsync(input);

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetTicketSaleStatTypeComboboxItems()
        {
            var result = _queryTicketAppService.GetTicketSaleStatTypeComboboxItems();

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatCashierSale)]
        public async Task<JsonResult> StatCashierSaleAsync([FromForm] StatCashierSaleInput input)
        {
            var result = await _queryTicketAppService.StatCashierSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatPromoterSale)]
        public async Task<JsonResult> StatPromoterSaleAsync([FromForm] StatPromoterSaleInput input)
        {
            var result = await _queryTicketAppService.StatPromoterSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleByTradeSource)]
        public async Task<JsonResult> StatTicketSaleByTradeSourceAsync([FromForm] StatTicketSaleByTradeSourceInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleByTradeSourceAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchReprintLog)]
        public async Task<JsonResult> QueryReprintLogAsync([FromForm] QueryReprintLogInput input)
        {
            var result = await _queryTicketAppService.QueryReprintLogsAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchExchangeHistory)]
        public async Task<JsonResult> QueryExchangeHistoryAsync([FromForm] QueryExchangeHistoryInput input)
        {
            var result = await _queryTicketAppService.QueryExchangeHistorysAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatShift)]
        public async Task<JsonResult> StatExchangeHistoryJbAsync([FromForm] StatJbInput input)
        {
            var result = await _queryTicketAppService.StatExchangeHistoryJbAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleByTicketTypeClass)]
        public async Task<JsonResult> StatTicketSaleByTicketTypeClassAsync([FromForm] StatTicketSaleByTicketTypeClassInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleByTicketTypeClassAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleByPayType)]
        public async Task<FileContentResult> StatTicketSaleByPayTypeToExcelAsync([FromForm] StatTicketSaleByPayTypeInput input)
        {
            var fileContents = await _queryTicketAppService.StatTicketSaleByPayTypeToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleByPayType)]
        public async Task<JsonResult> StatTicketSaleByPayTypeAsync([FromForm] StatTicketSaleByPayTypeInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleByPayTypeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleBySalePoint)]
        public async Task<JsonResult> StatTicketSaleBySalePointAsync([FromForm] StatTicketSaleBySalePointInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleBySalePointAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleByCustomer)]
        public async Task<JsonResult> StatTicketSaleByCustomerAsync([FromForm] StatTicketSaleByCustomerInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleByCustomerAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketSaleGroundSharing)]
        public async Task<JsonResult> StatTicketSaleGroundSharingAsync([FromForm] StatGroundSharingInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleGroundSharingAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatShift)]
        public async Task<JsonResult> StatTicketSaleJbAsync([FromForm] StatJbInput input)
        {
            var result = await _queryTicketAppService.StatTicketSaleJbAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatGroundChangCiSale)]
        public async Task<FileContentResult> StatGroundChangCiSaleToExcelAsync(StatGroundChangCiSaleInput input)
        {
            var fileContents = await _queryTicketAppService.StatGroundChangCiSaleToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatGroundChangCiSale)]
        public async Task<JsonResult> StatGroundChangCiSaleAsync(StatGroundChangCiSaleInput input)
        {
            var result = await _queryTicketAppService.StatGroundChangCiSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTicketConsume)]
        public async Task<FileContentResult> QueryTicketConsumesToExcelAsync([FromForm] QueryTicketConsumeInput input)
        {
            var fileContents = await _queryTicketAppService.QueryTicketConsumesToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTicketConsume)]
        public async Task<JsonResult> QueryTicketConsumesAsync([FromForm] QueryTicketConsumeInput input)
        {
            var result = await _queryTicketAppService.QueryTicketConsumesAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketConsume)]
        public async Task<FileContentResult> StatTicketConsumeToExcelAsync([FromForm] StatTicketConsumeInput input)
        {
            var fileContents = await _queryTicketAppService.StatTicketConsumeToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTicketConsume)]
        public async Task<JsonResult> StatTicketConsumeAsync([FromForm] StatTicketConsumeInput input)
        {
            var result = await _queryTicketAppService.StatTicketConsumeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTouristNum)]
        public async Task<JsonResult> StatTouristNumAsync([FromForm] StatTouristNumInput input)
        {
            var result = await _queryTicketAppService.StatTouristNumAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatCzkSale)]
        public async Task<JsonResult> StatCzkSaleAsync([FromForm] StatCzkSaleInput input)
        {
            var result = await _queryTicketAppService.StatCzkSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatShift)]
        public async Task<JsonResult> StatCzkSaleJbAsync([FromForm] StatJbInput input)
        {
            var result = await _queryTicketAppService.StatCzkSaleJbAsync(input);

            return Json(result);
        }
    }
}
