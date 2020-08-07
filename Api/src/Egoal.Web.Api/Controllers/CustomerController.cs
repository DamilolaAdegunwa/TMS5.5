using Egoal.Application.Services.Dto;
using Egoal.Customers;
using Egoal.Customers.Dto;
using Egoal.Extensions;
using Egoal.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class CustomerController : TmsControllerBase
    {
        private readonly ICustomerAppService _customerAppService;

        public CustomerController(
            ICustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        [HttpPost]
        public async Task RegistFromMobileAsync(MobileRegistInput input)
        {
            await _customerAppService.RegistFromMobileAsync(input);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task CreateAsync(EditDto input)
        {
            await _customerAppService.CreateAsync(input);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetForEditAsync(Guid id)
        {
            var editDto = await _customerAppService.GetForEditAsync(id);

            return new JsonResult(editDto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task EditAsync(EditDto input)
        {
            await _customerAppService.EditAsync(input);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task AuditAsync(AuditInput input)
        {
            await _customerAppService.AuditAsync(input);
        }

        [HttpPost]
        public async Task ChangePasswordAsync(ChangePasswordInput input)
        {
            await _customerAppService.ChangePasswordAsync(input);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _customerAppService.DeleteAsync(input.Id);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task UnBindMemberAsync(UnBindMemberInput input)
        {
            await _customerAppService.UnBindMemberAsync(input);
        }

        [HttpPost]
        public async Task<JsonResult> LoginFromWeChatAsync(CustomerLoginInput input)
        {
            var result = await _customerAppService.LoginFromWeChatAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetCustomersAsync(GetCustomersInput input)
        {
            var result = await _customerAppService.GetCustomersAsync(input);

            return new JsonResult(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetCustomerTypeComboboxItemsAsync()
        {
            var items = await _customerAppService.GetCustomerTypeComboboxItemsAsync();

            return new JsonResult(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetCustomerComboboxItemsAsync()
        {
            var items = await _customerAppService.GetCustomerComboboxItemsAsync();

            return new JsonResult(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetConsumeCustomerComboBoxItemsAsync()
        {
            var items = await _customerAppService.GetConsumeCustomerComboBoxItemsAsync();

            return new JsonResult(items);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCustomerStatusComboboxItemsAsync()
        {
            var items = typeof(MemberStatus).ToComboboxItems();

            return new JsonResult(items);
        }
    }
}
