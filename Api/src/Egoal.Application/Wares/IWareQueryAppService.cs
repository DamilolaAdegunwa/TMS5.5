using Egoal.Application.Services.Dto;
using Egoal.Tickets.Dto;
using Egoal.Wares.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Wares
{
    public interface IWareQueryAppService
    {
        Task<List<ComboboxItemDto<int>>> GetMerchantComboBoxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetShopComboBoxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetShopTypeComboBoxItemsAsync();
        Task<List<ComboboxItemDto<Guid>>> GetSupplierComboBoxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetWareTypeComboBoxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetWareTypeTypeComboBoxItemsAsync();
        List<ComboboxItemDto<int>> GetPayDetailStatTypeComboBoxItems();
        Task<PagedResultDto<WareListDto>> QueryWareAsync(QueryWareInput input);
        Task<byte[]> QueryWareToExcelAsync(QueryWareInput input);
        Task<PagedResultDto<WareIODetailListDto>> QueryWareIODetailAsync(QueryWareIODetailInput input);
        Task<byte[]> QueryWareIODetailToExcelAsync(QueryWareIODetailInput input);
        Task<PagedResultDto<WareTradeListDto>> QueryWareTradeAsync(QueryWareTradeInput input);
        Task<byte[]> QueryWareTradeToExcelAsync(QueryWareTradeInput input);
        Task<DynamicColumnResultDto> StatWareTradeAsync(StatWareTradeInput input);
        Task<byte[]> StatWareTradeToExcelAsync(StatWareTradeInput input);
        Task<DynamicColumnResultDto> StatWareSaleByWareTypeAsync(StatWareSaleByWareTypeInput input);
        Task<byte[]> StatWareSaleByWareTypeToExcelAsync(StatWareSaleByWareTypeInput input);
        Task<DataTable> StatWareRentSaleRentAsync(StatWareRentSaleInput input);
        Task<DataTable> StatWareRentSaleSaleAsync(StatWareRentSaleInput input);
        Task<DataSet> StatWareRentSaleAsync(StatWareRentSaleInput input);
        Task<DataTable> StatWareTradeTotalAsync(StatWareTradeTotalInput input);
        Task<DataTable> StatWareSaleAsync(StatWareSaleInput input);
        Task<DataTable> StatWareSaleShiftAsync(StatJbInput input);
    }
}
