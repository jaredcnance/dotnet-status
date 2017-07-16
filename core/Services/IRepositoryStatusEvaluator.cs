using DotnetStatus.Core.Models;
using System.Threading.Tasks;

namespace DotnetStatus.Core.Services
{
    public interface IRepositoryStatusEvaluator
    {
        Task<RepositoryResult> EvaluateAsync(string repositoryUrl);
    }
}