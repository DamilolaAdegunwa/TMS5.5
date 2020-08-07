using Egoal.Report.Dto;
using Egoal.Report.Net;
using Egoal.Report.Tickets.Dto;
using Egoal.Report.Wares.Dto;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Report.Wares
{
    public class WareAppService
    {
        public async Task<DataSet> StatWareRentSaleAsync(StatWareRentSaleInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ware/StatWareRentSaleAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataSet>>(json);

            return response.Result;
        }

        public async Task<DataTable> StatWareTradeTotalAsync(StatWareTradeTotalInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ware/StatWareTradeTotalAsync", input, token);
            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);
            return response.Result;
        }

        public async Task<DataTable> StatWareSaleAsync(StatWareSaleInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/ware/StatWareSaleAsync", input, token);
            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);
            return response.Result;
        }

        public async Task<DataTable> StatWareSaleShiftAsync(StatJbInput input, string token)
        {
            var json = await HttpHelper.PostJsonAsync("/ware/StatWareSaleShiftAsync", input, token);
            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);
            return response.Result;
        }
    }
}
