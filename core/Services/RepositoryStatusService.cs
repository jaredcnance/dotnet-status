using DotnetStatus.Core.Data;
using DotnetStatus.Core.Models;
using System.Threading.Tasks;

namespace Core.Services
{
    public class RepositoryStatusService : IRepositoryStatusService
    {
        private readonly ICache _cache;
        private readonly IRepositoryResultPersistence _resultService;

        public RepositoryStatusService(
            ICache cache,
            IRepositoryResultPersistence resultService)
        {
            _cache = cache;
            _resultService = resultService;
        }

        public async Task<RepositoryResult> FindAsync(string repositoryUrl)
        {
            repositoryUrl = FormatUrl(repositoryUrl);

            // check cache
            var result = await _cache.GetAsync<RepositoryResult>(repositoryUrl);
            if (result != null)
                return result;

            // check database
            result = await _resultService.GetAsync(repositoryUrl);

            if (result != null)
                _cache.Add(repositoryUrl, result);

            return result;
        }

        public async Task SetStatusAsync(string repositoryUrl, EvaluationStatus evalStatus)
        {
            repositoryUrl = FormatUrl(repositoryUrl);

            var repositoryResult = await _resultService.GetAsync(repositoryUrl);

            if (repositoryResult == null)
                repositoryResult = new RepositoryResult(repositoryUrl, evalStatus);

            await _resultService.SaveAsync(repositoryResult);
        }

        private string FormatUrl(string repositoryUrl) => repositoryUrl.ToLower();
    }
}
