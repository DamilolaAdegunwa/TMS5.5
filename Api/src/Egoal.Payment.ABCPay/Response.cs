using Newtonsoft.Json;

namespace Egoal.Payment.ABCPay
{
    public class Response<T>
    {
        public T Message { get; set; }

        [JsonProperty("Signature-Algorithm")]
        public string SignatureAlgorithm { get; set; }

        public string Signature { get; set; }
    }
}
