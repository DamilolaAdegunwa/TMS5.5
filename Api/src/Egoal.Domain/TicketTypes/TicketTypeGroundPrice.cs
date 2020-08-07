using Egoal.Domain.Entities;
using Egoal.Trades;

namespace Egoal.TicketTypes
{
    public class TicketTypeGroundPrice : Entity
    {
        public int TicketTypeId { get; set; }
        public decimal TicketTypePrice { get; set; }
        public TradeSource? TradeSource { get; set; }
        public int GroundId { get; set; }
        public decimal TicPrice { get; set; }
    }
}
