using System;
using System.Threading.Tasks;

namespace DotnetStatus.Services.Http
{
    public interface IHttpClient : IDisposable
    {
        Task<T> GetAsync<T>(string uri) where T : class;
    }
}
