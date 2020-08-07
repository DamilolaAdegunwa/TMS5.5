using Egoal.Authorization;
using Egoal.Mvc.Auditing;
using Egoal.Mvc.Authorization;
using Egoal.TicketTypes;
using Egoal.TicketTypes.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class TicketTypeController : TmsControllerBase
    {
        private Runtime.Session.ISession _session;
        private readonly ITicketTypeAppService _ticketTypeAppService;
        private readonly ITicketTypeQueryAppService _ticketTypeQueryAppService;

        public TicketTypeController(
            Runtime.Session.ISession session,
            ITicketTypeAppService ticketTypeAppService,
            ITicketTypeQueryAppService ticketTypeQueryAppService)
        {
            _session = session;
            _ticketTypeAppService = ticketTypeAppService;
            _ticketTypeQueryAppService = ticketTypeQueryAppService;
        }

        [HttpPost]
        public async Task<JsonResult> GetTicketTypesForWeiXinSaleAsync(GetTicketTypesForSaleInput input)
        {
            input.SaleChannel = SaleChannel.Net;
            var result = await _ticketTypeQueryAppService.GetTicketTypesForSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetTicketTypesForLocalSaleAsync(GetTicketTypesForSaleInput input)
        {
            input.SaleChannel = SaleChannel.Local;
            var result = await _ticketTypeQueryAppService.GetTicketTypesForSaleAsync(input);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetTicketTypesForSelfHelpAsync(GetTicketTypesForSaleInput input)
        {
            input.SaleChannel = SaleChannel.Net;
            var result = await _ticketTypeQueryAppService.GetTicketTypesForSelfHelpAsync(input);

            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_TicketTypeDescription)]
        public async Task<JsonResult> GetTicketTypeDescriptionsAsync(GetTicketTypeDescriptionsInput input)
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeDescriptionsAsync(input);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketTypeDescriptionAsync(int ticketTypeId)
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeDescriptionAsync(ticketTypeId);

            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_TicketTypeDescription)]
        public async Task CreateDescriptionAsync(TicketTypeDescriptionDto input)
        {
            await _ticketTypeAppService.CreateDescriptionAsync(input);
        }

        [HttpPost]
        [Auditing]
        [PermissionFilter(Permissions.TMSAdmin_TicketTypeDescription)]
        public async Task UpdateDescriptionAsync(TicketTypeDescriptionDto input)
        {
            await _ticketTypeAppService.UpdateDescriptionAsync(input);
        }

        [HttpDelete]
        [PermissionFilter(Permissions.TMSAdmin_TicketTypeDescription)]
        public async Task DeleteDescriptionAsync(int ticketTypeId)
        {
            await _ticketTypeAppService.DeleteDescriptionAsync(ticketTypeId);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketTypeForWeiXinSaleAsync(int ticketTypeId)
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeForNetSaleAsync(ticketTypeId, SaleChannel.Net, _session.MemberId.Value);

            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetGroundChangCisDtosVariedAsync(int ticketTypeId, DateTime date)
        {
            var result = await _ticketTypeQueryAppService.GetGroundChangCisDtosVariedAsync(ticketTypeId, date);

            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetTicketTypeChangCiComboboxItemsAsync(int ticketTypeId, DateTime date)
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeChangCiComboboxItemsAsync(ticketTypeId, date);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketTypeTypeComboboxItemsAsync()
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeTypeComboboxItemsAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketTypeComboboxItemsAsync(TicketTypeType? ticketTypeTypeId)
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeComboboxItemsAsync(ticketTypeTypeId);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetNetSaleTicketTypeComboboxItemsAsync()
        {
            var result = await _ticketTypeQueryAppService.GetNetSaleTicketTypeComboboxItemsAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetTicketTypeClassComboboxItemsAsync()
        {
            var result = await _ticketTypeQueryAppService.GetTicketTypeClassComboboxItemsAsync();

            return Json(result);
        }
    }
}