using System.Linq;
using System.Threading.Tasks;
using DotnetStatus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace DotnetStatus.Controllers
{
    public class GitHubPackageStatusController : Controller
    {
        private readonly ILogger<GitHubPackageStatusController> _log;
        private readonly IPackageStatusService _statusService;

        public GitHubPackageStatusController(
            IPackageStatusService packageStatusService,
            ILogger<GitHubPackageStatusController> log)
        {
            _statusService = packageStatusService;
            _log = log;
        }

        // api/status/gh/{*path}
        public async Task<IActionResult> GetGithub()
        {
            var route = Request.Path.Value.Split('/').ToList();
            
            // api/status/gh/user/project/path..csproj
            if(route.Count < 6)
                return BadRequest();

            // api/status/gh/{*path} -> {*path} => 
            route.RemoveRange(0, 4);

            var formattedRoute = string.Join("/", route);

            _log.LogInformation($"routes: {formattedRoute}");

            var link = GetGithubLink(formattedRoute);

            _log.LogInformation($"link: {link}");

            var result = await _statusService.GetStatusAsync(link);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        private string GetGithubLink(string path) => $"https://raw.githubusercontent.com/{path}".TrimEnd('/');
    }
}
