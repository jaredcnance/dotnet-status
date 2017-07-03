using System.Linq;
using System.Threading.Tasks;
using DotnetStatus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace DotnetStatus.Controllers
{
    public class PackageStatusController : Controller
    {
        private readonly ILogger<PackageStatusController> _log;
        private readonly NuGetStatusService _statusService;

        public PackageStatusController(ILogger<PackageStatusController> log)
        {
            _statusService = new NuGetStatusService();
            _log = log;
        }
        
        // api/status/gh/{*path}
        public async Task<IActionResult> Get()
        {
            var route = Request.Path.Value.Split('/').ToList();
            
            // api/status/gh/{*path} -> {*path}
            route.RemoveRange(0, 4);

            var formattedRoute = string.Join("/", route);

            _log.LogInformation($"routes: {formattedRoute}");

            var link = GetGithubLink(formattedRoute);

            _log.LogInformation($"link: {link}");
            
            var result = await _statusService.GetStatusAsync(link);

            return Ok(result);
        }

        private string GetGithubLink(string path) => $"https://raw.githubusercontent.com/{path}".TrimEnd('/');
    }
}
