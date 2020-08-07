using Egoal.Excel;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Tickets.Dto
{
    public class StatTicketSaleListDto
    {
        [DynamicDisplay]
        public string StatType { get; set; }

        [Display(Name = "售票张数")]
        public int SaleNum { get; set; }

        [Display(Name = "售票人数")]
        public int SalePersonNum { get; set; }

        [Display(Name = "售票金额")]
        public decimal SaleMoney { get; set; }

        [Display(Name = "退票张数")]
        public int ReturnNum { get; set; }

        [Display(Name = "退票人数")]
        public int ReturnPersonNum { get; set; }

        [Display(Name = "退票金额")]
        public decimal ReturnMoney { get; set; }

        [Display(Name = "实售张数")]
        public int RealNum { get; set; }

        [Display(Name = "实售人数")]
        public int RealPersonNum { get; set; }

        [Display(Name = "实售金额")]
        public decimal RealMoney { get; set; }
    }
}
