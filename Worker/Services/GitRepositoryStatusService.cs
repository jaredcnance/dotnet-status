using Core.Data;
using DotnetStatus.Core.Models;
using DotnetStatus.Worker.Services.NuGet;
using Microsoft.Extensions.Options;
using System.Threading;

namespace DotnetStatus.Worker.Services
{
    class GitRepositoryStatusService : IGitRepositoryStatusService
    {
        private readonly ITransientGitService _gitService;
        private readonly IRestoreService _restoreService;
        private readonly IDependencyGraphService _dependencyGraphService;
        private readonly string _dgFileName;
        private readonly IRepositoryStatusRepository _repository;

        public GitRepositoryStatusService(
            ITransientGitService transientGitService,
            IRestoreService restoreService,
            IDependencyGraphService dependencyGraphService,
            IOptions<WorkerConfiguration> options,
            IRepositoryStatusRepository repository)
        {
            _gitService = transientGitService;
            _restoreService = restoreService;
            _dependencyGraphService = dependencyGraphService;
            _dgFileName = options.Value.DependencyGraphFileName;
            _repository = repository;
        }

        public RepositoryStatusResult GetRepositoryStatus(string repositoryUrl)
        {
            var repoPath = _gitService.GetSource(repositoryUrl);
            var dependencyGraphPath = $"{repoPath}/{_dgFileName}";
            var status = _restoreService.Restore(repoPath, dependencyGraphPath);

            if (status.Success == false)
                return new RepositoryStatusResult(repositoryUrl, status);

            var projectResults = _dependencyGraphService.GetProjectResults(dependencyGraphPath);

            var result = new RepositoryStatusResult(repositoryUrl, status, projectResults);

            _repository.SaveAsync(result, CancellationToken.None).Wait();

            return result;
        }
    }
}
