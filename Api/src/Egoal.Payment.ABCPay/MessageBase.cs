namespace Egoal.Payment.ABCPay
{
    public class MessageBase
    {
        public MessageBase()
        {
            Merchant = new Merchant();
        }

        public string Version { get; set; } = "V3.0.0";
        public string Format { get; set; } = "JSON";
        public Merchant Merchant { get; set; }
    }
}
