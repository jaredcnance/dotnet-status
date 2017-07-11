using DotnetStatus.Core;

namespace DotnetStatus.Worker.Services.NuGet
{
    interface IRestoreService
    {
        RestoreStatus Restore(string projectPath, string dependencyGraphOutputPath);
    }
}