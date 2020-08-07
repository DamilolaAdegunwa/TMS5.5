using System.Collections.Generic;

namespace Egoal.Scenics.Dto
{
    public class BookGroundChangCiInput
    {
        public int TicketTypeId { get; set; }
        public string Date { get; set; }
        public int? GroundId { get; set; }
        public int? ChangCiId { get; set; }
        public string StartTime { get; set; }
        public int? SeatTypeId { get; set; }
        public int Quantity { get; set; }
        public string ListNo { get; set; }
        public bool IsRemote { get; set; }
        public List<GroundChangCiDto> GroundChangCis { get; set; }
    }
}
