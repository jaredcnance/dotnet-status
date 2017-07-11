using NuGet.Configuration;
using System.Collections.Generic;

namespace DotnetStatus.Worker.Services.NuGet
{
    interface IPackageStatusStore
    {
        PackageStatus GetStatus(string packageId);
        void ReloadSources(IList<PackageSource> sources);
    }
}