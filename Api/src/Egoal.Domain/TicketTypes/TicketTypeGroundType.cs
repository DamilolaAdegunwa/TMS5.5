using Egoal.Domain.Entities;

namespace Egoal.TicketTypes
{
    public class TicketTypeGroundType : Entity
    {
        public int TicketTypeId { get; set; }
        public int GroundTypeId { get; set; }
        public int TotalNum { get; set; }
    }
}
