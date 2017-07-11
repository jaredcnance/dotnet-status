using DotnetStatus.Core.Models;

namespace DotnetStatus.Worker.Services
{
    interface IGitRepositoryStatusService
    {
        RepositoryStatusResult GetRepositoryStatus(string repositoryUrl);
    }
}