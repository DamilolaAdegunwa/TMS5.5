using Egoal.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Egoal.Wares.Dto
{
    public class WareIODetailListDto : EntityDto<long>
    {
        [Display(Name = "序号")]
        public int RowNum { get; set; }

        [Display(Name = "商品名称")]
        public string WareName { get; set; }

        [Display(Name = "单号")]
        public string ListNo { get; set; }

        [Display(Name = "交易类型")]
        public string TradeTypeName { get; set; }

        

        [Display(Name = "成本价")]
        public decimal? CostPrice { get; set; }
        [Display(Name = "零售价")]
        public decimal? RetailPrice { get; set; }
        [Display(Name = "租金")]
        public decimal? RentPrice { get; set; }
        [Display(Name = "押金")]
        public decimal? YaJin { get; set; }
        [Display(Name = "出入数量")]
        public decimal? Ionum { get; set; }

        [Display(Name = "金额")]
        public decimal? ReaMoney { get; set; }

        [Display(Name = "折扣类型")]
        public string DiscountTypeName { get; set; }

        

        [Display(Name = "收银员")]
        public string CashierName { get; set; }

        [Display(Name = "收银机")]
        public string CashPcname { get; set; }

        [Display(Name = "商店")]
        public string WareShopName { get; set; }

        [Display(Name = "创建时间")]
        public string Ctime { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }
    }

}
