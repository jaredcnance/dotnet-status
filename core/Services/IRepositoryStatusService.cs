using System.Threading.Tasks;
using DotnetStatus.Core.Models;

namespace Core.Services
{
    public interface IRepositoryStatusService
    {
        Task<RepositoryResult> FindAsync(string repositoryUrl);
    }
}