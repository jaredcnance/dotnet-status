using DotnetStatus.Core.Models;
using System.Threading.Tasks;

namespace DotnetStatus.Core.Services
{
    public interface IRepositoryResultEvaluator
    {
        Task<RepositoryResult> EvaluateAsync(string repositoryUrl);
    }
}