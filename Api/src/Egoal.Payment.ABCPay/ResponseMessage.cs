namespace Egoal.Payment.ABCPay
{
    public class ResponseMessage<T> : MessageBase
    {
        public ResponseMessage()
            : base()
        { }

        public T TrxResponse { get; set; }
    }
}
