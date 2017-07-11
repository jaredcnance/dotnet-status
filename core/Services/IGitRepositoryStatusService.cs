using DotnetStatus.Core.Models;
using System.Threading.Tasks;

namespace DotnetStatus.Core.Services
{
    public interface IGitRepositoryStatusService
    {
        Task<RepositoryResult> GetRepositoryStatusAsync(string repositoryUrl);
    }
}