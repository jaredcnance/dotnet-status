using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICache
    {
        Task<T> GetAsync<T>(string key);
        void Add(string key, object value);
    }
}