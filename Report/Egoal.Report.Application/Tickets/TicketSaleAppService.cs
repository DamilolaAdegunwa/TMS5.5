using Egoal.Report.Dto;
using Egoal.Report.Net;
using Egoal.Report.Tickets.Dto;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets
{
    public class TicketSaleAppService
    {
        public async Task<StatCashierSaleDto> StatCashierSaleAsync(StatCashierSaleInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatCashierSaleAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<StatCashierSaleDto>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleByTradeSourceAsync(StatTicketSaleByTradeSourceInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleByTradeSourceAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleByTicketTypeClassAsync(StatTicketSaleByTicketTypeClassInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleByTicketTypeClassAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleBySalePointAsync(StatTicketSaleBySalePointInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleBySalePointAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleGroundSharingAsync(StatTicketSaleGroundSharingInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleGroundSharingAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleByCustomerAsync(StatTicketSaleByCustomerInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleByCustomerAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketCheckInByGateGroupAsync(StatTicketCheckInInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketCheckInByGateGroupAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTicketSaleJbAsync(StatJbInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTicketSaleJbAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatExchangeHistoryJbAsync(StatJbInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatExchangeHistoryJbAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatPromoterSaleAsync(StatPromoterSaleInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatPromoterSaleAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatTouristNumAsync(StatTouristNumInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatTouristNumAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatCzkSaleJbAsync(StatJbInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatCzkSaleJbAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatCzkSaleAsync(StatCzkSaleInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ticket/StatCzkSaleAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }
    }
}
