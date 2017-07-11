using System.Collections.Generic;
using NuGet.Configuration;

namespace DotnetStatus.Worker.Services.NuGet
{
    class PackageStatusStore : IPackageStatusStore
    {
        private readonly IPackageStatusRepository _repository;
        private readonly Dictionary<string, PackageStatus> _cachedStatuses =  new Dictionary<string, PackageStatus>();

        public PackageStatusStore(IPackageStatusRepository repository)
        {
            _repository = repository;
        }

        public void ReloadSources(IList<PackageSource> sources)
        {
            _cachedStatuses.Clear();
            _repository.LoadPackageMetadataResources(sources);
        }

        public PackageStatus GetStatus(string packageId)
        {
            if (_cachedStatuses.TryGetValue(packageId, out PackageStatus cachedStatus))
                return cachedStatus;

            var status = _repository.GetStatus(packageId);

            _cachedStatuses[packageId] = status;

            return status;
        }
    }
}
