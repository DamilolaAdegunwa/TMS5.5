using Egoal.Report.Extensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Net
{
    public static class HttpHelper
    {
        public const int Timeout = 10;
        public static string BaseUrl = string.Empty;

        public static async Task<string> PostJsonAsync(string url, string json, string token = "")
        {
            return await PostJsonAsync(url, json, Encoding.UTF8, token);
        }

        public static async Task<string> PostJsonAsync(string url, object obj, string token = "")
        {
            return await PostJsonAsync(url, JsonConvert.SerializeObject(obj), Encoding.UTF8, token);
        }

        public static async Task<string> PostJsonAsync(string url, string json, Encoding encoding, string token = "")
        {
            return await SendAsync(url, json, encoding, "POST", "application/json", token);
        }

        public static async Task<string> PostFormDataAsync(string url, object obj, string token = "")
        {
            return await PostFormDataAsync(url, obj.ToUrlArgs(), token);
        }

        public static async Task<string> PostFormDataAsync(string url, string data, string token = "")
        {
            return await PostFormDataAsync(url, data, Encoding.UTF8, token);
        }

        public static async Task<string> PostFormDataAsync(string url, string data, Encoding encoding, string token = "")
        {
            return await SendAsync(url, data, encoding, "POST", $"application/x-www-form-urlencoded;charset={encoding.WebName}", token);
        }

        public static async Task<string> SendAsync(string url, string data, Encoding encoding, string method, string contentType, string token = "")
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(BaseUrl))
                {
                    url = $"{BaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
                }
                var uri = new Uri(url);

                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = method;
                request.Timeout = Timeout * 1000;
                request.Proxy = null;
                request.ContentType = contentType;
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.ConnectionLimit = 200;
                if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    request.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", $"Bearer {token}");
                }

                var sendBytes = encoding.GetBytes(data);
                if (sendBytes != null && sendBytes.Length > 0)
                {
                    request.ContentLength = sendBytes.Length;
                    using (var reqStream = await request.GetRequestStreamAsync())
                    {
                        await reqStream.WriteAsync(sendBytes, 0, sendBytes.Length);
                    }
                }

                response = (HttpWebResponse)await request.GetResponseAsync();

                using (var resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, encoding))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
    }
}
