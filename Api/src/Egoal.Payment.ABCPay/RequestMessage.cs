namespace Egoal.Payment.ABCPay
{
    public class RequestMessage : MessageBase
    {
        public RequestMessage()
            : base()
        { }

        public object TrxRequest { get; set; }
    }
}
