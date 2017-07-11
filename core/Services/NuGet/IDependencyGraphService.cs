using DotnetStatus.Core.Models;
using System.Collections.Generic;

namespace DotnetStatus.Core.Services.NuGet
{
    public interface IDependencyGraphService
    {
        List<ProjectResult> GetProjectResults(string dependencyGraphPath);
    }
}