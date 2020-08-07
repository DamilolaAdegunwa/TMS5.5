namespace Egoal.Report.Tickets.Dto
{
    public class StatJbInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? ParkId { get; set; }
        public int? SalePointId { get; set; }
        public int? CashierId { get; set; }
        public bool? HasShift { get; set; }
        public bool StatTicketByPayType { get; set; }
        public bool IncludeWareDetail { get; set; }
    }
}
