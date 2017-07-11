using DotnetStatus.Core.Models;
using DotnetStatus.Worker.Services.NuGet;
using Microsoft.Extensions.Options;

namespace DotnetStatus.Worker.Services
{
    class GitRepositoryStatusService : IGitRepositoryStatusService
    {
        private readonly ITransientGitService _gitService;
        private readonly IRestoreService _restoreService;
        private readonly IDependencyGraphService _dependencyGraphService;
        private readonly string _dgFileName;

        public GitRepositoryStatusService(
            ITransientGitService transientGitService,
            IRestoreService restoreService,
            IDependencyGraphService dependencyGraphService,
            IOptions<WorkerConfiguration> options)
        {
            _gitService = transientGitService;
            _restoreService = restoreService;
            _dependencyGraphService = dependencyGraphService;
            _dgFileName = options.Value.DependencyGraphFileName;
        }

        public RepositoryStatusResult GetRepositoryStatus(string repositoryUrl)
        {
            var repoPath = _gitService.GetSource(repositoryUrl);
            var dependencyGraphPath = $"{repoPath}/{_dgFileName}";
            var status = _restoreService.Restore(repoPath, dependencyGraphPath);

            if (status.Success == false)
                return new RepositoryStatusResult(status);

            var projectResults = _dependencyGraphService.GetProjectResults(dependencyGraphPath);

            return new RepositoryStatusResult(status, projectResults);
        }
    }
}
