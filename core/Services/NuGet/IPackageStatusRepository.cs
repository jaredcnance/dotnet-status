using System.Collections.Generic;
using NuGet.Configuration;
using DotnetStatus.Core.Models;

namespace DotnetStatus.Core.Services.NuGet
{
    public interface IPackageStatusRepository
    {
        PackageStatus GetStatus(string packageId);
        void LoadPackageMetadataResources(IList<PackageSource> sources);
    }
}