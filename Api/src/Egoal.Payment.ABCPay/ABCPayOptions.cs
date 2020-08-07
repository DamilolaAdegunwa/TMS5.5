using System.Security.Cryptography.X509Certificates;

namespace Egoal.Payment.ABCPay
{
    public class ABCPayOptions
    {
        public const string DateTimeFormat = "yyyyMMddHHmmss";

        public string ABCTrustPayCertPath { get; set; } = @"E:\TrustPayClient_ASP.NET_V3.1.7\demo\cert\TrustPay.cer";
        public X509Certificate2 TrustPayCert { get; set; }
        public string ABCTrustStorePath { get; set; } = @"E:\TrustPayClient_ASP.NET_V3.1.7\demo\cert\abc.truststore";
        public string ABCTrustStorePassword { get; set; } = "changeit";
        public string ABCMerchantId { get; set; } = "103884420000338";
        public string ABCMerchantCertPath { get; set; } = @"E:\TrustPayClient_ASP.NET_V3.1.7\demo\cert\338.pfx";
        public string ABCMerchantCertPassword { get; set; } = "ABC12345";
        public X509Certificate2 MerchantCert { get; set; }

        public string WxAppID { get; set; }
        public string WxMiniprogramAppID { get; set; }
        public string WebApiUrl { get; set; }
    }
}
