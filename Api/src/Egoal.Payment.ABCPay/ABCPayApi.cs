using Egoal.Extensions;
using Egoal.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Egoal.Payment.ABCPay
{
    public class ABCPayApi
    {
        private readonly ABCPayOptions _options;
        private readonly ILogger _logger;
        private readonly HttpService _httpService;

        public ABCPayApi(
            IOptions<ABCPayOptions> options,
            ILogger<ABCPayApi> logger,
            HttpService httpService)
        {
            _options = options.Value;
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<T> ExecuteAsync<T>(object trxRequest)
        {
            string requestData = string.Empty;
            string responseData = string.Empty;

            try
            {
                var request = new Request();
                request.Message.Merchant.MerchantID = _options.ABCMerchantId;
                request.Message.TrxRequest = trxRequest;
                request.SignatureAlgorithm = "SHA1withRSA";
                request.Signature = SignHash(request.Message.ToJson());

                requestData = request.ToJson();

                var url = "https://pay.abchina.com/ebus/trustpay/ReceiveMerchantTrxReqServlet";
                responseData = await _httpService.PostJsonAsync(url, requestData, Encoding.UTF8, Encoding.GetEncoding("gb2312"));

                VerifySign(responseData.GetJsonKeyValue("Message"), responseData.GetJsonKeyValue("Signature"));

                var responseObj = responseData.GetJsonKeyValue("MSG").JsonToObject<Response<T>>();

                Log(requestData, responseData);

                return responseObj.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}--{requestData}--{responseData}");

                throw;
            }
        }

        private string SignHash(string data)
        {
            LoadMerchantCert();
            var rsa = (RSA)_options.MerchantCert.PrivateKey;
            var buffer = rsa.SignHash(new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(data)), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(buffer);
        }

        private void LoadMerchantCert()
        {
            if (_options.MerchantCert == null)
            {
                _options.MerchantCert = new X509Certificate2(_options.ABCMerchantCertPath, _options.ABCMerchantCertPassword, X509KeyStorageFlags.MachineKeySet);
            }
        }

        private void VerifySign(string message, string signature)
        {
            LoadTrustPayCert();
            byte[] signatureBuffer = Convert.FromBase64String(signature);
            var rsa = (RSA)_options.TrustPayCert.PublicKey.Key;
            if (!rsa.VerifyData(Encoding.GetEncoding("gb2312").GetBytes(message), signatureBuffer, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1))
            {
                throw new ApiException("农业银行Response签名验证错误");
            }
        }

        private void LoadTrustPayCert()
        {
            if (_options.TrustPayCert == null)
            {
                _options.TrustPayCert = new X509Certificate2(_options.ABCTrustPayCertPath);
            }
        }

        private void Log(string requestData, string responseData)
        {
            if (responseData.GetJsonKeyValue("ReturnCode") == "0000")
            {
                _logger.LogInformation($"{requestData}--{responseData}");
            }
            else
            {
                _logger.LogError($"{responseData.GetJsonKeyValue("ErrorMessage")}--{requestData}--{responseData}");
            }
        }

        public NotifyRequest DeserializeNotify(string data)
        {
            var args = data.FromUrlArgs();
            string xml = Encoding.GetEncoding("gb2312").GetString(Convert.FromBase64String(args["MSG"].UrlDecode()));
            XElement xElement = XElement.Parse(xml);
            var response = xElement.Element("Message").Element("TrxResponse");

            VerifySign(response.ToString(SaveOptions.DisableFormatting), xElement.Element("Signature").Value);

            return NotifyRequest.FromXml(response);
        }
    }
}
