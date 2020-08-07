using Egoal.Application.Services.Dto;
using Egoal.Tickets.Dto;
using Egoal.Wares.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Wares
{
    public interface IWareRepository
    {
        Task<PagedResultDto<WareListDto>> QueryWareAsync(QueryWareInput input);
        Task<PagedResultDto<WareIODetailListDto>> QueryWareIODetailAsync(QueryWareIODetailInput input);
        Task<PagedResultDto<WareTradeListDto>> QueryWareTradeAsync(QueryWareTradeInput input);
        Task<DataTable> StatWareTradeAsync(StatWareTradeInput input, IEnumerable<ComboboxItemDto<int>> payTypes);
        Task<DataTable> StatWareSaleByWareTypeAsync(StatWareSaleByWareTypeInput input);
        Task<DataTable> StatWareRentSaleRentAsync(StatWareRentSaleInput input);
        Task<DataTable> StatWareRentSaleSaleAsync(StatWareRentSaleInput input);
        Task<DataTable> StatWareTradeTotal(StatWareTradeTotalInput input);
        Task<DataTable> StatWareSaleAsync(StatWareSaleInput input);
        Task<DataTable> StatWareSaleShiftAsync(StatJbInput input);



        Task<string> GetCardLastMemberAccountID(string cardNo);
    }
}
