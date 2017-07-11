using DotnetStatus.Core.Configuration;
using DotnetStatus.Core.Data;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.NuGet;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DotnetStatus.Core.Services
{
    public class GitRepositoryStatusService : IGitRepositoryStatusService
    {
        private readonly ITransientGitService _gitService;
        private readonly IRestoreService _restoreService;
        private readonly IDependencyGraphService _dependencyGraphService;
        private readonly string _dgFileName;
        private readonly IRepositoryResultService _repository;

        public GitRepositoryStatusService(
            ITransientGitService transientGitService,
            IRestoreService restoreService,
            IDependencyGraphService dependencyGraphService,
            IOptions<WorkerConfiguration> options,
            IRepositoryResultService repository)
        {
            _gitService = transientGitService;
            _restoreService = restoreService;
            _dependencyGraphService = dependencyGraphService;
            _dgFileName = options.Value.DependencyGraphFileName;
            _repository = repository;
        }

        public async Task<RepositoryResult> GetRepositoryStatusAsync(string repositoryUrl)
        {
            var repoPath = _gitService.GetSource(repositoryUrl);
            var dependencyGraphPath = $"{repoPath}/{_dgFileName}";
            var status = _restoreService.Restore(repoPath, dependencyGraphPath);

            if (status.Success == false)
                return await GetFailedResultAsync(repositoryUrl, status);

            var projectResults = _dependencyGraphService.GetProjectResults(dependencyGraphPath);

            var result = new RepositoryResult(repositoryUrl, status, projectResults);

            await _repository.SaveAsync(result);

            return result;
        }

        private async Task<RepositoryResult> GetFailedResultAsync(string repositoryUrl, RestoreStatus status)
        {
            var failedResult = new RepositoryResult(repositoryUrl, status);

            await _repository.SaveAsync(failedResult);

            return failedResult;
        }
    }
}
