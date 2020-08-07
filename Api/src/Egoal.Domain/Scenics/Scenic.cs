using Egoal.Domain.Entities;

namespace Egoal.Scenics
{
    public class Scenic : Entity
    {
        public string ScenicName { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string Photos { get; set; }
        public string ScenicIntro { get; set; }
        public string ScenicFeature { get; set; }
        public string NoticeTitle { get; set; }
        public string NoticeContent { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string Address { get; set; }
    }
}
