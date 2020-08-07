using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class StatWareTradeInput
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public string ListNo { get; set; }

        public string Memo { get; set; }

        public int? TradeTypeTypeId { get; set; }

        public int? TradeTypeId { get; set; }

        public int? CashierId { get; set; }

        public int? CashPcId { get; set; }

        public int? MerchantId { get; set; }

        public int? ShopTypeId { get; set; }

        public int? ShopId { get; set; }

        public int StatTypeId { get; set; }

        /// <summary>
        /// 统计类型
        /// </summary>
        public string[] statColumns { get; set; } = new[] { "b.CDate", "b.CWeek", "b.CMonth", "b.CQuarter", "b.CYear", "b.TradeTypeId", "b.ShopId", "b.CashierId" };
    }

    public enum PayDetailStatType
    {
        日期 = 0,
        星期 = 1,
        月份 = 2,
        季度 = 3,
        年份 = 4,
        交易类型 = 5,
        商店 = 6,
        收银员  = 7
    }
}
