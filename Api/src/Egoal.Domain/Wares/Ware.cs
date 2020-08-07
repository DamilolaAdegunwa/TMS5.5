using Egoal.Domain.Entities;
using System;

namespace Egoal.Wares
{
    public class Ware : Entity<Guid>
    {
        public string Name { get; set; }
        public string WareCode { get; set; }
        public string SortCode { get; set; }
        public string Barcode { get; set; }
        public string Zjf { get; set; }
        public int? WareTypeId { get; set; }
        public string WareTypeName { get; set; }
        public int? WareKindId { get; set; }
        public int? WareRsTypeId { get; set; }
        public string WareRsTypeName { get; set; }
        public string WareUnit { get; set; }
        public string WareStandard { get; set; }
        public string WareColour { get; set; }
        public string WareProducter { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RentPrice { get; set; }
        public decimal? MemberRetailPrice { get; set; }
        public decimal? YaJin { get; set; }
        public int? RentTypeId { get; set; }
        public string RentTypeName { get; set; }
        public int? MeterTypeId { get; set; }
        public string MeterTypeName { get; set; }
        public int? FreeJsMinutes { get; set; }
        public int? FeeJsUnit { get; set; }
        public decimal? RentJsPrice { get; set; }
        public decimal? MinJsRentFee { get; set; }
        public decimal? MaxJsRentFee { get; set; }
        public int? StockNum { get; set; }
        public decimal? StockMoney { get; set; }
        public int? SotckMaxNum { get; set; }
        public int? SotckMinNum { get; set; }
        public int? StockAlarmNum { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public bool? SaleFlag { get; set; }
        public bool? StatFlag { get; set; }
        public bool? JbStatFlag { get; set; }
        public bool? SalePermitFlag { get; set; }
        public bool? CanDiscount { get; set; }
        public bool? PointFlag { get; set; }
        public bool? NeedWarehouseFlag { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public int? ShopId { get; set; }
        public int? MerchantId { get; set; }
        public bool? BdFlag { get; set; }
        public bool? TcFlag { get; set; }
        public string Memo { get; set; }
        public int? Cid { get; set; }
        public DateTime? Ctime { get; set; }
        public int? Mid { get; set; }
        public DateTime? Mtime { get; set; }
        public int? ParkId { get; set; }
        public string ParkName { get; set; }
    }
}
