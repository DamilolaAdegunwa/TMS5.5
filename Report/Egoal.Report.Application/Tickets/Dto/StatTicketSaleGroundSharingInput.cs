namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketSaleGroundSharingInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? SalePointId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? GroundId { get; set; }
    }
}
