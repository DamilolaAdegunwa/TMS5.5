using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.Excel;
using Egoal.Extensions;
using Egoal.Payment;
using Egoal.Tickets.Dto;
using Egoal.Wares.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Wares
{
    public class WareQueryAppService : ApplicationService, IWareQueryAppService
    {
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Shop> _shopRepository;
        private readonly IRepository<ShopType> _shopTypeRepository;
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<WareType> _wareTypeRepository;
        private readonly IRepository<WareTypeType> _wareTypeTypeRepository;
        private readonly IWareRepository _wareRepository;
        private readonly IPayTypeAppService _payTypeAppService;
        private readonly INameCacheService _nameCacheService;

        public WareQueryAppService(IRepository<Merchant> merchantRepository,
            IRepository<Shop> shopRepository,
            IRepository<ShopType> shopTypeRepository,
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<WareType> wareTypeRepository,
            IRepository<WareTypeType> wareTypeTypeRepository,
            IWareRepository wareRepository,
            IPayTypeAppService payTypeAppService,
            INameCacheService nameCacheService)
        {
            _merchantRepository = merchantRepository;
            _shopRepository = shopRepository;
            _shopTypeRepository = shopTypeRepository;
            _supplierRepository = supplierRepository;
            _wareTypeRepository = wareTypeRepository;
            _wareTypeTypeRepository = wareTypeTypeRepository;
            _wareRepository = wareRepository;
            _payTypeAppService = payTypeAppService;
            _nameCacheService = nameCacheService;
        }

        public async Task<List<ComboboxItemDto<int>>> GetMerchantComboBoxItemsAsync()
        {
            var query = _merchantRepository.GetAll().Select(t => new ComboboxItemDto<int> { Value = t.Id, DisplayText = t.MerchantName });
            List<ComboboxItemDto<int>> MerchantComboBoxItems = await _merchantRepository.ToListAsync(query);
            return MerchantComboBoxItems;
        }

        public async Task<List<ComboboxItemDto<int>>> GetShopComboBoxItemsAsync()
        {
            var query = _shopRepository.GetAll().Select(t => new ComboboxItemDto<int> { Value = t.Id, DisplayText = t.Name });
            List<ComboboxItemDto<int>> ShopComboBoxItems = await _shopRepository.ToListAsync(query);
            return ShopComboBoxItems;
        }

        public async Task<List<ComboboxItemDto<int>>> GetShopTypeComboBoxItemsAsync()
        {
            var query = _shopTypeRepository.GetAll().Select(t => new ComboboxItemDto<int> { Value = t.Id, DisplayText = t.Name });
            List<ComboboxItemDto<int>> ShopTypeComboBoxItems = await _shopTypeRepository.ToListAsync(query);
            return ShopTypeComboBoxItems;
        }

        public async Task<List<ComboboxItemDto<Guid>>> GetSupplierComboBoxItemsAsync()
        {
            var query = _supplierRepository.GetAll().Select(t => new ComboboxItemDto<Guid> { Value = t.Id, DisplayText = t.Name });
            List<ComboboxItemDto<Guid>> SupplierComboBoxItems = await _supplierRepository.ToListAsync(query);
            return SupplierComboBoxItems;
        }

        public async Task<List<ComboboxItemDto<int>>> GetWareTypeComboBoxItemsAsync()
        {
            var query = _wareTypeRepository.GetAll().Select(t => new ComboboxItemDto<int> { Value = t.Id, DisplayText = t.Name });
            List<ComboboxItemDto<int>> wareTypeComboBoxItems = await _wareTypeRepository.ToListAsync(query);
            return wareTypeComboBoxItems;
        }

        public async Task<List<ComboboxItemDto<int>>> GetWareTypeTypeComboBoxItemsAsync()
        {
            var query = _wareTypeTypeRepository.GetAll().Select(t => new ComboboxItemDto<int> { Value = t.Id, DisplayText = t.Name });
            List<ComboboxItemDto<int>> WareTypeTypeComboBoxItems = await _wareTypeTypeRepository.ToListAsync(query);
            return WareTypeTypeComboBoxItems;
        }

        public List<ComboboxItemDto<int>> GetPayDetailStatTypeComboBoxItems()
        {
            List<ComboboxItemDto<int>> comboBoxItemDtos = typeof(PayDetailStatType).ToComboboxItems();
            return comboBoxItemDtos;
        }

        public async Task<PagedResultDto<WareListDto>> QueryWareAsync(QueryWareInput input)
        {
            var query = await _wareRepository.QueryWareAsync(input);
            return query;
        }

        public async Task<byte[]> QueryWareToExcelAsync(QueryWareInput input)
        {
            input.ShouldPage = false;

            var result = await QueryWareAsync(input);
            return await ExcelHelper.ExportToExcelAsync(result.Items, "商品查询", string.Empty);
        }

        public async Task<PagedResultDto<WareIODetailListDto>> QueryWareIODetailAsync(QueryWareIODetailInput input)
        {
            var query = await _wareRepository.QueryWareIODetailAsync(input);
            return query;
        }

        public async Task<byte[]> QueryWareIODetailToExcelAsync(QueryWareIODetailInput input)
        {
            var result = await QueryWareIODetailAsync(input);
            return await ExcelHelper.ExportToExcelAsync(result.Items, "商品租住查询", string.Empty);
        }

        public async Task<PagedResultDto<WareTradeListDto>> QueryWareTradeAsync(QueryWareTradeInput input)
        {
            var query = await _wareRepository.QueryWareTradeAsync(input);
            return query;
        }

        public async Task<byte[]> QueryWareTradeToExcelAsync(QueryWareTradeInput input)
        {
            var result = await QueryWareTradeAsync(input);
            return await ExcelHelper.ExportToExcelAsync(result.Items, "商品交易查询", string.Empty);
        }

        public async Task<DynamicColumnResultDto> StatWareTradeAsync(StatWareTradeInput input)
        {
            List<ComboboxItemDto<int>> payTypes = await _payTypeAppService.GetPayTypeComboboxItemsAsync();
            DataTable dataTable = await _wareRepository.StatWareTradeAsync(input, payTypes);
            List<Shop> shops = await _shopRepository.GetAllListAsync();

            foreach (DataColumn column in dataTable.Columns)
            {
                if (int.TryParse(column.ColumnName, out int payTypeId))
                {
                    column.ColumnName = payTypes.FirstOrDefault(p => p.Value == payTypeId).DisplayText;
                }
            }

            string statColumn = input.statColumns[input.StatTypeId].Substring(2);
            Enum.TryParse(input.StatTypeId.ToString(), out PayDetailStatType payDetailStatType);
            string statTypeName = payDetailStatType.ToString();
            if (input.StatTypeId > 4)
            {
                DataColumn dataColumn = new DataColumn(statTypeName);
                dataTable.Columns.Add(dataColumn);
                foreach (DataRow row in dataTable.Rows)
                {
                    if (int.TryParse(row[statColumn].ToString(), out int statValue))
                    {
                        switch (input.StatTypeId)
                        {
                            case (int)PayDetailStatType.交易类型:
                                row[statTypeName] = _nameCacheService.GetTradeTypeName(statValue);
                                break;
                            case (int)PayDetailStatType.商店:
                                Shop shop = shops.FirstOrDefault(s => s.Id == statValue);
                                row[statTypeName] = shop == null ? "" : shop.Name;
                                break;
                            case (int)PayDetailStatType.收银员:
                                row[statTypeName] = _nameCacheService.GetStaffName(statValue);
                                break;
                        }
                    }
                }
                dataTable.Columns.Remove(statColumn);
                dataColumn.SetOrdinal(0);
            }
            else
            {
                dataTable.Columns[statColumn].ColumnName = statTypeName;
            }


            if (!dataTable.IsNullOrEmpty())
            {
                dataTable.RowSum();
                dataTable.ColumnSum();
            }

            return new DynamicColumnResultDto(dataTable);
        }

        public async Task<byte[]> StatWareTradeToExcelAsync(StatWareTradeInput input)
        {
            DynamicColumnResultDto result = await StatWareTradeAsync(input);
            return await ExcelHelper.ExportToExcelAsync(result.Data, "商品交易统计", string.Empty);
        }

        public async Task<DynamicColumnResultDto> StatWareSaleByWareTypeAsync(StatWareSaleByWareTypeInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareSaleByWareTypeAsync(input);
            if (!dataTable.IsNullOrEmpty())
            {
                dataTable.RowSum();
            }
            return new DynamicColumnResultDto(dataTable);
        }

        public async Task<byte[]> StatWareSaleByWareTypeToExcelAsync(StatWareSaleByWareTypeInput input)
        {
            DynamicColumnResultDto result = await StatWareSaleByWareTypeAsync(input);
            return await ExcelHelper.ExportToExcelAsync(result.Data, "商品销售按商品类型统计", string.Empty);
        }

        public async Task<DataTable> StatWareRentSaleRentAsync(StatWareRentSaleInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareRentSaleRentAsync(input);
            return dataTable;
        }

        public async Task<DataTable> StatWareRentSaleSaleAsync(StatWareRentSaleInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareRentSaleSaleAsync(input);
            return dataTable;
        }

        public async Task<DataSet> StatWareRentSaleAsync(StatWareRentSaleInput input)
        {
            DataSet dataSet = new DataSet();
            DataTable rentDataTable = await StatWareRentSaleRentAsync(input);
            DataTable saleDataTable = await StatWareRentSaleSaleAsync(input);
            dataSet.Tables.Add(rentDataTable);
            dataSet.Tables.Add(saleDataTable);
            return dataSet;
        }

        public async Task<DataTable> StatWareTradeTotalAsync(StatWareTradeTotalInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareTradeTotal(input);
            return dataTable;
        }

        public async Task<DataTable> StatWareSaleAsync(StatWareSaleInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareSaleAsync(input);
            return dataTable;
        }

        public async Task<DataTable> StatWareSaleShiftAsync(StatJbInput input)
        {
            DataTable dataTable = await _wareRepository.StatWareSaleShiftAsync(input);
            return dataTable;
        }
    }
}
