using Egoal.Trades;
using System;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Wares.Dto
{
    public class WareTradeListDto
    {
        public int RowNum { get; set; }
        [Display(Name = "交易时间")]
        public string Ctime { get; set; }
        [Display(Name = "卡号")]
        public string CzkCardNo { get; set; }
        [Display(Name = "总金额")]
        public decimal PayMoney { get; set; }

        [Display(Name = "交易类型")]
        public string TradeTypeName { get; set; }

        [Display(Name = "付款方式")]
        public string PayTypeName { get; set; }
        [Display(Name = "客户名称")]
        public string CustomerName { get; set; }
        [Display(Name = "收银员")]
        public string CashierName { get; set; }
        [Display(Name = "收银机")]
        public string CashPcname { get; set; }
        [Display(Name = "商店")]
        public string ShopName { get; set; }

        [Display(Name = "单号")]
        public string ListNo { get; set; }
        [Display(Name = "备注")]
        public string Memo { get; set; }

    }
}
