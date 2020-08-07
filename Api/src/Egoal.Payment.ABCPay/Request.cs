using Newtonsoft.Json;

namespace Egoal.Payment.ABCPay
{
    public class Request
    {
        public Request()
        {
            Message = new RequestMessage();
        }

        public RequestMessage Message { get; set; }

        [JsonProperty("Signature-Algorithm")]
        public string SignatureAlgorithm { get; set; }

        public string Signature { get; set; }
    }
}
