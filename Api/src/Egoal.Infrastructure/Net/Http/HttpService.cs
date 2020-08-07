using Egoal.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Net.Http
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            return await _httpClient.GetStringAsync(requestUri);
        }

        public async Task<string> GetStringAsync(string requestUri, Encoding responseEncoding)
        {
            return await _httpClient.GetStringAsync(requestUri, responseEncoding);
        }

        public async Task<string> PostJsonAsync(string requestUri, object obj)
        {
            return await _httpClient.PostJsonAsync(requestUri, obj.ToJson());
        }

        public async Task<string> PostJsonAsync(string requestUri, string json)
        {
            return await _httpClient.PostJsonAsync(requestUri, json);
        }

        public async Task<string> PostJsonAsync(string requestUri, string json, Encoding requestEncoding, Encoding responseEncoding)
        {
            return await _httpClient.PostJsonAsync(requestUri, json, requestEncoding, responseEncoding);
        }

        public async Task<string> PostXmlAsync(string requestUri, string xml)
        {
            return await _httpClient.PostXmlAsync(requestUri, xml);
        }

        public async Task<string> PostXmlAsync(string requestUri, string xml, Encoding requestEncoding, Encoding responseEncoding)
        {
            return await _httpClient.PostXmlAsync(requestUri, xml, requestEncoding, responseEncoding);
        }

        public async Task<string> PostFormDataAsync(string requestUri, string args)
        {
            return await _httpClient.PostFormDataAsync(requestUri, args.FromUrlArgs());
        }

        public async Task<string> PostFormDataAsync(string requestUri, IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            return await _httpClient.PostFormDataAsync(requestUri, nameValueCollection);
        }

        public async Task<string> PostFormDataAsync(string requestUri, IEnumerable<KeyValuePair<string, string>> nameValueCollection, Encoding responseEncoding)
        {
            return await _httpClient.PostFormDataAsync(requestUri, nameValueCollection, responseEncoding);
        }
    }
}
