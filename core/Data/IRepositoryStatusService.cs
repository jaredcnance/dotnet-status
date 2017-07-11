using DotnetStatus.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IRepositoryStatusService
    {
        Task SaveAsync(RepositoryResult repoStatus, CancellationToken cancellationToken);
    }
}
