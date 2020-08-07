using Egoal.Cryptography.RSA;
using Egoal.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Egoal.Payment.Alipay
{
    public class AlipaySignature
    {
        public static string SignData(IDictionary<string, string> parameters, string privateKey, string charset, string signType)
        {
            string signContent = BuildSignContent(parameters);

            if (File.Exists(privateKey))
            {
                privateKey = File.ReadAllText(privateKey);
            }
            var key = RSAKeyHelper.CreatePKCS1PrivateKey(privateKey);
            var encoding = charset.IsNullOrEmpty() ? Encoding.UTF8 : Encoding.GetEncoding(charset);
            var halg = signType.ToUpper() == "RSA2" ? "SHA256" : "SHA1";
            var dwKeySize = signType.ToUpper() == "RSA2" ? 2048 : 1024;

            return RSAHelper.SignData(encoding.GetBytes(signContent), halg, dwKeySize, key);
        }

        private static string BuildSignContent(IDictionary<string, string> parameters)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters);
            var dem = sortedParams.GetEnumerator();

            StringBuilder queryBuilder = new StringBuilder();
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !key.Equals("sign", StringComparison.OrdinalIgnoreCase))
                {
                    queryBuilder.Append(key).Append("=").Append(value).Append("&");
                }
            }

            return queryBuilder.ToString().TrimEnd('&');
        }

        public static string VerifyResponseData(string data, string method, string publicKey, string charset, string signType)
        {
            string responseSign = GetResponseSign(data);
            string responseBody = GetResponseData(method, data);

            VerifyResponseSign(responseBody, responseSign, publicKey, charset, signType);

            return responseBody;
        }

        private static string GetResponseSign(string data)
        {
            int signIndex = data.IndexOf("\"sign\"");
            int startIndex = signIndex + 8;
            int length = data.Length - startIndex - 2;

            return data.Substring(startIndex, length);
        }

        private static string GetResponseData(string method, string data)
        {
            string dataNodeName = $"{method.Replace(".", "_")}_response";
            int dataNodeIndex = data.IndexOf(dataNodeName);
            if (dataNodeIndex > 0)
            {
                return ParseResponseData(data, dataNodeName, dataNodeIndex);
            }
            else
            {
                string errorNodeName = "error_response";
                int errorNodeIndex = data.IndexOf(errorNodeName);
                return ParseResponseData(data, errorNodeName, errorNodeIndex);
            }
        }

        private static string ParseResponseData(string data, string nodeName, int nodeIndex)
        {
            int startIndex = nodeIndex + nodeName.Length + 2;
            int signIndex = data.IndexOf("\"sign\"");
            int endIndex = signIndex < 0 ? data.Length - 1 : signIndex - 1;
            int length = endIndex - startIndex;

            return data.Substring(startIndex, length);
        }

        public static void VerifyResponseSign(string signContent, string sign, string publicKey, string charset, string signType)
        {
            if (publicKey.IsNullOrEmpty() || charset.IsNullOrEmpty() || sign.IsNullOrEmpty())
            {
                return;
            }

            bool success = VerifySign(signContent, sign, publicKey, charset, signType);
            if (!success)
            {
                if (!string.IsNullOrEmpty(signContent) && signContent.Contains("\\/"))
                {
                    signContent = signContent.Replace("\\/", "/");
                    success = VerifySign(signContent, sign, publicKey, charset, signType);
                    if (!success)
                    {
                        throw new ApiException("支付宝Response签名验证错误", $"{signContent}--{sign}");
                    }
                }
                else
                {
                    throw new ApiException("支付宝Response签名验证错误", $"{signContent}--{sign}");
                }
            }
        }

        public static bool VerifySign(string signContent, string sign, string publicKey, string charset, string signType)
        {
            if (File.Exists(publicKey))
            {
                publicKey = File.ReadAllText(publicKey);
            }
            var key = RSAKeyHelper.CreatePKCS1PublicKey(publicKey);
            var encoding = charset.IsNullOrEmpty() ? Encoding.UTF8 : Encoding.GetEncoding(charset);
            var halg = signType.ToUpper() == "RSA2" ? "SHA256" : "SHA1";

            return RSAHelper.VerifyData(encoding.GetBytes(signContent), halg, Convert.FromBase64String(sign), key);
        }
    }
}
