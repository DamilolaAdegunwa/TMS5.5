namespace Egoal.Payment.Dto
{
    public class PrePayInput
    {
        public string ListNo { get; set; }
        public decimal PayMoney { get; set; }
        public string ProductInfo { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string Attach { get; set; }
    }
}
