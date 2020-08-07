using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetStringAsync(this HttpClient httpClient, string requestUri, Encoding responseEncoding)
        {
            var result = await httpClient.GetByteArrayAsync(requestUri.Trim());
            return responseEncoding.GetString(result);
        }

        public static async Task<string> PostJsonAsync(this HttpClient httpClient, string requestUri, string json)
        {
            return await httpClient.PostJsonAsync(requestUri, json, Encoding.UTF8, Encoding.UTF8);
        }

        public static async Task<string> PostJsonAsync(this HttpClient httpClient, string requestUri, string json, Encoding requestEncoding, Encoding responseEncoding)
        {
            return await httpClient.PostStringAsync(requestUri, json, requestEncoding, responseEncoding, "application/json");
        }

        public static async Task<string> PostXmlAsync(this HttpClient httpClient, string requestUri, string xml)
        {
            return await httpClient.PostXmlAsync(requestUri, xml, Encoding.UTF8, Encoding.UTF8);
        }

        public static async Task<string> PostXmlAsync(this HttpClient httpClient, string requestUri, string xml, Encoding requestEncoding, Encoding responseEncoding)
        {
            return await httpClient.PostStringAsync(requestUri, xml, requestEncoding, responseEncoding, "text/xml");
        }

        public static async Task<string> PostFormDataAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            return await httpClient.PostFormDataAsync(requestUri, nameValueCollection, Encoding.UTF8);
        }

        public static async Task<string> PostFormDataAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> nameValueCollection, Encoding responseEncoding)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(nameValueCollection);
            return await httpClient.PostAsync(requestUri, content, responseEncoding);
        }

        public static async Task<string> PostStringAsync(this HttpClient httpClient, string requestUri, string content, Encoding requestEncoding, Encoding responseEncoding, string contentType)
        {
            StringContent stringContent = new StringContent(content, requestEncoding, contentType);
            return await httpClient.PostAsync(requestUri, stringContent, responseEncoding);
        }

        public static async Task<string> PostAsync(this HttpClient httpClient, string requestUri, HttpContent content, Encoding responseEncoding)
        {
            var response = await httpClient.PostAsync(requestUri.Trim(), content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsByteArrayAsync();
            return responseEncoding.GetString(result);
        }
    }
}
