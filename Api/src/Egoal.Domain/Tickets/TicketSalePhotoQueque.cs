using Egoal.Domain.Entities;
using System;

namespace Egoal.Tickets
{
    public class TicketSalePhotoQueque : Entity<long>
    {
        public FaceRegOpType? OpTypeId { get; set; }
        public long? TicketSalePhotoId { get; set; }
        public int? TicketTypeId { get; set; }
        public DateTime? CTime { get; set; }
    }
}
