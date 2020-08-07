using System.Collections.Generic;

namespace Egoal.Tickets.Dto
{
    public class TicketSalePhotoDto
    {
        public TicketSalePhotoDto()
        {
            Photos = new List<object>();
        }

        public long TicketId { get; set; }
        public string TicketCode { get; set; }
        public int SurplusQuantity { get; set; }
        public string TicketTypeName { get; set; }
        public List<object> Photos { get; set; }
        public int MaxPhotoQuantity { get; set; }
        public string TicketSaleStatusName { get; set; }
        public bool AllowEnrollFace { get; set; }
    }
}
