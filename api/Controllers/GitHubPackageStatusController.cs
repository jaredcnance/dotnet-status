using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Messaging;
using Core.Services;

namespace DotnetStatus.Controllers
{
    public class GitHubPackageStatusController : Controller
    {
        private readonly ILogger<GitHubPackageStatusController> _log;
        private readonly IPublishStringMessage _publish;
        private readonly IRepositoryStatusService _repoStatus;

        public GitHubPackageStatusController(
            ILogger<GitHubPackageStatusController> log,
            IPublishStringMessage publish,
            IRepositoryStatusService repoStatus)
        {
            _log = log;
            _publish = publish;
            _repoStatus = repoStatus;
        }

        [HttpGet("/api/status/gh/{user}/{repo}")]
        public async Task<IActionResult> GetGithub(string user, string repo)
        {
            var link = GetGithubLink(user, repo);

            var result = await _repoStatus.FindAsync(link);

            if (result != null)
                return Ok(result);

            await _publish.PublishMessageAsync("git-remote-queue", link);

            return Accepted();
        }

        private string GetGithubLink(string user, string repo) => $"https://github.com/{user}/{repo}.git";
    }
}
