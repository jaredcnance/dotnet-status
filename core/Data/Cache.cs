using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Core.Services
{
    public class Cache : ICache
    {
        private readonly IMemoryCache _memoryCache;

        public Cache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

#pragma warning disable CS1998 // async method lacks 'await' operators and will run synchronously
        public async Task<T> GetAsync<T>(string key)
        {
            // TODO: support distributed caching
            return _memoryCache.Get<T>(key);
        }

        public void Add(string key, object value)
        {
            var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
        }
    }
}
