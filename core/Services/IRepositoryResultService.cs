using System.Threading.Tasks;
using DotnetStatus.Core.Models;

namespace Core.Services
{
    public interface IRepositoryResultService
    {
        /// <summary>
        /// Find the RepositoryResult by its URL
        /// </summary>
        Task<RepositoryResult> FindAsync(string repositoryUrl);

        /// <summary>
        /// Set the status of a RepositoryResult and create if it does not exist
        /// </summary>
        Task SetStatusAsync(string repositoryUrl, EvaluationStatus evalStatus);
    }
}