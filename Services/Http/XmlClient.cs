using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DotnetHealth.Services.Http
{
    public class XmlClient : IDisposable
    {
        private readonly HttpClient _client;

        public XmlClient()
        {
            _client = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _client.GetAsync(uri);
            
            if(response.IsSuccessStatusCode == false)
                throw new HttpRequestException($"Request to {uri} failed with status {response.StatusCode}");

            var body = await response.Content.ReadAsStreamAsync();
            
            var serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(body);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
