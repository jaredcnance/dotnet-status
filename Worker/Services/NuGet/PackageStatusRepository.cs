using System.Collections.Generic;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol;
using NuGet.Configuration;
using System.Threading;
using NuGet.Common;
using NuGet.Versioning;

namespace DotnetStatus.Worker.Services.NuGet
{
    class PackageStatusRepository : IPackageStatusRepository
    {
        private List<(PackageMetadataResource MetadataResource, string Source)> _packageMetaDataResources = new List<(PackageMetadataResource MetadataResource, string Source)>();
        private readonly ILogger _log;

        public PackageStatusRepository(ILogger log)
        {
            _log = log;
        }

        public void LoadPackageMetadataResources(IList<PackageSource> sources)
        {
            foreach (var src in sources)
            {
                if (src.IsLocal) continue;

                var v2Repo = Repository.Factory.GetCoreV2(src);
                var packageMetadataResource = v2Repo.GetResourceAsync<PackageMetadataResource>().GetAwaiter().GetResult();
                _packageMetaDataResources.Add((packageMetadataResource, src.Source));
            }
        }

        public PackageStatus GetStatus(string packageId)
        {
            var status = new PackageStatus
            {
                LatestStable = new NuGetVersion("0.0.0"),
                Latest = new NuGetVersion("0.0.0")
            };

            foreach (var src in _packageMetaDataResources)
            {
                var packageMetadata = src.MetadataResource.GetMetadataAsync(packageId, true, false, _log, CancellationToken.None).GetAwaiter().GetResult();
                
                foreach(PackageSearchMetadataRegistration versionMeta in packageMetadata)
                {
                    if (versionMeta.Version > status.Latest)
                    {
                        status.Latest = versionMeta.Version;
                        status.LatestSource = src.Source;
                    }

                    if(versionMeta.Version > status.LatestStable && versionMeta.Version.IsPrerelease == false)
                    {
                        status.LatestStable = versionMeta.Version;
                        status.LatestStableSource = src.Source;
                    }

                    status.AllVersions.Add(versionMeta.Version);
                }
            }

            return status;
        }
    }
}
