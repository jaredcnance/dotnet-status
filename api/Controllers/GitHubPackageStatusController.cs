using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Messaging;

namespace DotnetStatus.Controllers
{
    public class GitHubPackageStatusController : Controller
    {
        private readonly ILogger<GitHubPackageStatusController> _log;
        private readonly IPublishStringMessage _publish;

        public GitHubPackageStatusController(
            ILogger<GitHubPackageStatusController> log,
            IPublishStringMessage publish)
        {
            _log = log;
            _publish = publish;
        }

        [HttpGet("/api/status/gh/{user}/{repo}")]
        public async Task<IActionResult> GetGithub(string user, string repo)
        {
            var link = GetGithubLink(user, repo);

            await _publish.PublishMessageAsync("git-remote-queue", link);

            return Accepted();
        }

        private string GetGithubLink(string user, string repo) => $"https://github.com/{user}/{repo}.git";
    }
}
