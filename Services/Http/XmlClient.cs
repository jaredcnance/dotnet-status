using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DotnetStatus.Services.Http
{
    public class XmlClient : IXmlClient
    {
        private readonly HttpClient _client;

        public XmlClient()
        {
            _client = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string uri) where T : class
        {
            var response = await _client.GetAsync(uri);

            if(response.StatusCode == HttpStatusCode.NotFound)
                return null;
            
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
