using DotnetStatus.Core;
using System.Collections.Generic;

namespace DotnetStatus.Worker.Services.NuGet
{
    interface IDependencyGraphService
    {
        List<ProjectResult> GetProjectResults(string dependencyGraphPath);
    }
}