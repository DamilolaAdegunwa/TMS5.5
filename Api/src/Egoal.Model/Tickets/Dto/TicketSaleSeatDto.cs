using Newtonsoft.Json;
using System;

namespace Egoal.Tickets.Dto
{
    public class TicketSaleSeatDto
    {
        public Guid? TradeId { get; set; }
        public long? TicketId { get; set; }
        [JsonIgnore]
        public int GroundId { get; set; }
        public string GroundName { get; set; }
        [JsonIgnore]
        public long? SeatId { get; set; }
        public string SeatName { get; set; }
        [JsonIgnore]
        public int? StadiumId { get; set; }
        public string StadiumName { get; set; }
        [JsonIgnore]
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public string Sdate { get; set; }
        [JsonIgnore]
        public int? ChangCiId { get; set; }
        public string ChangCiName { get; set; }
    }
}
