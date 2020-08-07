using Egoal.Domain.Entities;

namespace Egoal.Stadiums
{
    public class Region : Entity
    {
        public string Name { get; set; }
        public string SortCode { get; set; }
        public int? StadiumId { get; set; }
        public int? FloorId { get; set; }
        public string InDoor { get; set; }
        public string OutDoor { get; set; }
        public int? SeatTypeId { get; set; }
        public int? SeatNum { get; set; }
        public int? Xcount { get; set; }
        public int? Ycount { get; set; }
        public string SeatCodePrefix { get; set; }
        public int? SeatCodeLen { get; set; }
        public int? SeatCodeStartIndex { get; set; }
        public int? StartRowNum { get; set; }
        public int? StartColumnCode { get; set; }
    }
}
