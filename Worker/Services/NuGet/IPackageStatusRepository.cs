using System.Collections.Generic;
using NuGet.Configuration;

namespace DotnetStatus.Worker.Services.NuGet
{
    interface IPackageStatusRepository
    {
        PackageStatus GetStatus(string packageId);
        void LoadPackageMetadataResources(IList<PackageSource> sources);
    }
}