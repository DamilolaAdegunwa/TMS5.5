using Egoal.Report.Dto;
using Egoal.Report.Net;
using Egoal.Report.Tickets.Dto;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Report.Trades
{
    public class TradeAppService
    {
        public async Task<DataTable> StatPayDetailJbAsync(StatJbInput input, string token)
        {
            var json = await HttpHelper.PostFormDataAsync("/trade/StatPayDetailJbAsync", input, token);

            var response = JsonConvert.DeserializeObject<AjaxResponse<DataTable>>(json);

            return response.Result;
        }
    }
}
