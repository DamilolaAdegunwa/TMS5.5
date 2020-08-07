namespace Egoal.Common.Dto
{
    public class ChangCiPlanDto
    {
        public string Date { get; set; }
        public int? GroundId { get; set; }
        public int ChangCiId { get; set; }
        public string ChangCiName { get; set; }
        public string STime { get; set; }
        public string ETime { get; set; }
        public int? ChangCiNum { get; set; }
        public int ReservedNum { get; set; }
        public int SurplusNum { get; set; }
    }
}
