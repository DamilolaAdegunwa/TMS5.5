using System.Collections.Generic;

namespace Egoal.Orders.Dto
{
    public class OrderGroundChangCiDto
    {
        public string GroundName { get; set; }
        public string ChangCiName { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public List<OrderSeatDto> Seats { get; set; }
    }

    public class OrderSeatDto
    {
        public string SeatName { get; set; }
    }
}
