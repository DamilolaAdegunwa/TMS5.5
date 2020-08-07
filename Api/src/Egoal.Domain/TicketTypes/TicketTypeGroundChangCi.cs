using Egoal.Domain.Entities;

namespace Egoal.TicketTypes
{
    public class TicketTypeGroundChangCi : Entity
    {
        public int? TicketTypeId { get; set; }
        public int? GroundId { get; set; }
        public int? ChangCiId { get; set; }
    }
}
