using DotnetStatus.Core.Models;
using NuGet.Configuration;
using System.Collections.Generic;

namespace DotnetStatus.Core.Services.NuGet
{
    public interface IPackageStatusStore
    {
        PackageStatus GetStatus(string packageId);
        void ReloadSources(IList<PackageSource> sources);
    }
}