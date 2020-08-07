namespace Egoal.Report.Tickets.Dto
{
    public class StatCashierSaleInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? CashierId { get; set; }
        public int? SalePointId { get; set; }
        public int? StatTypeId { get; set; }
    }
}
