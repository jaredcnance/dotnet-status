using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotnetHealth.Services.Http
{
    public class JsonClient : IDisposable
    {
        private readonly HttpClient _client;

        public JsonClient()
        {
            _client = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await _client.GetAsync(uri);
            
            if(response.IsSuccessStatusCode == false)
                throw new HttpRequestException($"Request to {uri} failed with status {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(body);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
