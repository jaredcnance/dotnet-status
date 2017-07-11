using DotnetStatus.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IRepositoryStatusRepository
    {
        Task SaveAsync(RepositoryStatusResult repoStatus, CancellationToken cancellationToken);
    }
}
