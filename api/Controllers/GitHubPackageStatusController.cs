using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Messaging;
using Core.Services;
using DotnetStatus.Core.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DotnetStatus.Controllers
{
    public class GitHubPackageStatusController : Controller
    {
        private readonly ILogger<GitHubPackageStatusController> _log;
        private readonly IPublishStringMessage _publish;
        private readonly IRepositoryResultService _repoStatus;
        private const int ResultTTLMinutes = 1440; // 24 hrs

        public GitHubPackageStatusController(
            ILogger<GitHubPackageStatusController> log,
            IPublishStringMessage publish,
            IRepositoryResultService repoStatus)
        {
            _log = log;
            _publish = publish;
            _repoStatus = repoStatus;
        }

        [HttpGet("/api/status/gh/{user}/{repo}")]
        public async Task<IActionResult> GetGithub(string user, string repo)
        {
            var repositoryUri = GetGithubUri(user, repo);

            var result = await _repoStatus.FindAsync(repositoryUri);

            return await GetActionResultAsync(repositoryUri, result);
        }

        [HttpGet("/api/status/gh/{user}/{repo}/{projectName}")]
        public async Task<IActionResult> GetGithub(string user, string repo, string projectName)
        {
            var repositoryUri = GetGithubUri(user, repo);

            var result = await _repoStatus.FindAsync(repositoryUri);

            result.ProjectResults = new List<ProjectResult> {
                result.ProjectResults?.FirstOrDefault(p => p?.Name == projectName)
            };

            return await GetActionResultAsync(repositoryUri, result);
        }

        private async Task<IActionResult> GetActionResultAsync(string repositoryUri, RepositoryResult result)
        {
            var currentStatus = result?.EvaluationStatus;

            if (currentStatus == EvaluationStatus.Complete || currentStatus == EvaluationStatus.Failed)
            {
                // if it is expired, queue a job to update it
                if (result.UpdatedAt.AddMinutes(ResultTTLMinutes) <= DateTimeOffset.UtcNow)
                    await QueueProcessing(repositoryUri);

                return Ok(result);
            }

            if (currentStatus == EvaluationStatus.Processing)
                return Accepted();

            await QueueProcessing(repositoryUri);

            return Accepted();
        }

        private async Task QueueProcessing(string repositoryUri)
        {
            await _publish.PublishMessageAsync("git-remote-queue", repositoryUri);

            await _repoStatus.SetStatusAsync(repositoryUri, EvaluationStatus.Processing);
        }

        private string GetGithubUri(string user, string repo) => $"https://github.com/{user}/{repo}.git";
    }
}
