using System.Collections.Generic;

namespace Egoal.Tickets.Dto
{
    public class MemberTicketSaleDto
    {
        public string TicketCode { get; set; }
        public string TicketTypeName { get; set; }
        public string StatusName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CTime { get; set; }
        public bool WxShowQrCode { get; set; }
        public string ListNo { get; set; }
        public List<GroundChangCi> GroundChangCis { get; set; }

        public void AddGroundChangCi(GroundChangCi groundChangCi)
        {
            if (GroundChangCis == null)
            {
                GroundChangCis = new List<GroundChangCi>();
            }

            GroundChangCis.Add(groundChangCi);
        }

        public class GroundChangCi
        {
            public string GroundName { get; set; }
            public string ChangCiName { get; set; }
            public List<string> Seats { get; set; }

            public void AddSeat(string seat)
            {
                if (Seats == null)
                {
                    Seats = new List<string>();
                }

                Seats.Add(seat);
            }
        }
    }
}
