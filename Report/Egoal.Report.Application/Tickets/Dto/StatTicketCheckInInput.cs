namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketCheckInInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? ParkId { get; set; }
        public int? GateGroupId { get; set; }
        public int StatType { get; set; }
    }
}
