using Egoal.Application.Services.Dto;
using Egoal.Trades;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTicketSaleInput
    {
        public DateTime? StartCTime { get; set; }

        [EndTime]
        public DateTime? EndCTime { get; set; }
        public string StartTravelDate { get; set; }
        public string EndTravelDate { get; set; }
        public int? TicketTypeTypeId { get; set; }
        public int? TicketTypeId { get; set; }
        public TicketStatus? TicketStatusId { get; set; }
        public int? SalePointId { get; set; }
        public int? CashierId { get; set; }
        public int? CashpcId { get; set; }
        public TradeSource? TradeSource { get; set; }
        public TicketSaleStatType StatType { get; set; }
    }

    public enum TicketSaleStatType
    {
        日期 = 1,
        星期 = 2,
        月份 = 3,
        季度 = 4,
        年份 = 5,
        票类 = 6,
        购买类型 = 7
    }
}
