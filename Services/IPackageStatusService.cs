using System.Threading.Tasks;
using DotnetStatus.Services.Http;

namespace DotnetStatus.Services
{
    public interface IPackageStatusService
    {
        Task<Result> GetStatusAsync(string csprojUrl);
    }
}
