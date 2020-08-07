using Egoal.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Wares.Dto
{
    public class WareListDto : EntityDto<Guid>
    {
        [Display(Name = "序号")]
        public int RowNum { get; set; }

        [Display(Name = "商品名称")]
        public string Name { get; set; }

        [Display(Name = "商品条码")]
        public string WareCode { get; set; }

        [Display(Name = "助记符")]
        public string Zjf { get; set; }

        [Display(Name = "排序号")]
        public string SortCode { get; set; }

        [Display(Name = "商品条码")]
        public string BarCode { get; set; }

        [Display(Name = "商品类型")]
        public string WareTypeName { get; set; }

        [Display(Name = "需要库存")]
        public bool? NeedWareHouseFlag { get; set; }

        [Display(Name = "单位")]
        public string WareUnit { get; set; }

        [Display(Name = "规格")]
        public string WareStandard { get; set; }

        [Display(Name="成本价")]
        public decimal? CostPrice { get; set; }

        [Display(Name = "零售价")]
        public decimal? RetailPrice { get; set; }

        [Display(Name = "出租价")]
        public decimal? RentPrice { get; set; }

        [Display(Name = "押金")]
        public decimal? YaJin { get; set; }

        [Display(Name = "租售类型")]
        public string WareRsTypeName { get; set; }

        [Display(Name = "免费时长")]
        public string FreeJsMinutes { get; set; }

        [Display(Name = "计费单位")]
        public string FeeJsUnit { get; set; }

        [Display(Name = "计费价格")]
        public decimal? RentJsPrice { get; set; }

        [Display(Name = "商品颜色")]
        public string WareColour { get; set; }

        [Display(Name = "生产商")]
        public string WareProducter { get; set; }

        [Display(Name = "供应商")]
        public string SupplierName { get; set; }

        [Display(Name = "商家")]
        public string MerchantName { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }
    }
}
