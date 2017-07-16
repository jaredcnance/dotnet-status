using DotnetStatus.Core.Models;
using System.Threading.Tasks;

namespace DotnetStatus.Core.Data
{
    public interface IRepositoryResultPersistence
    {
        Task SaveAsync(RepositoryResult repoStatus);
        Task<RepositoryResult> GetAsync(string id);
    }
}
