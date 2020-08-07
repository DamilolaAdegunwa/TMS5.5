namespace Egoal.Payment.Dto
{
    public class QueryNetPayJobArgs
    {
        public string ListNo { get; set; }
        public int PayTypeId { get; set; }
        public OnlinePayTradeType OnlinePayTradeType { get; set; }
        public NetPayType? NetPayTypeId { get; set; }
        public string Attach { get; set; }
        public int IntervalSecond { get; set; }
    }
}
