using System.Threading.Tasks;
using DotnetHealth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace DotnetHealth.Controllers
{
    [Route("api/[controller]")]
    public class PackageStatusController : Controller
    {
        private readonly ILogger<PackageStatusController> _log;
        private readonly NuGetStatusService _statusService;

        public PackageStatusController(ILogger<PackageStatusController> log)
        {
            _statusService = new NuGetStatusService();
            _log = log;
        }

        [HttpGet("{csprojUrl}")]
        public async Task<IActionResult> Get(string csprojUrl)
        {
            var result = await _statusService.GetStatusAsync(csprojUrl);
            return Ok(result);
        }        
    }
}
