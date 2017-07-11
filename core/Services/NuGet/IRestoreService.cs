using DotnetStatus.Core;
using DotnetStatus.Core.Models;

namespace DotnetStatus.Core.Services.NuGet
{
    public interface IRestoreService
    {
        RestoreStatus Restore(string projectPath, string dependencyGraphOutputPath);
    }
}