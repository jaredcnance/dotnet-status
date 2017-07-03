using System.Collections.Generic;
using System.Linq;
using Semver;

namespace DotnetHealth.Services.Http
{
    public class Result
    {
        public bool IsPreRelease => Packages.Any(p => p.IsPreRelease);
        public bool UpToDate => Packages.All(p => p.UpToDate);
        public List<PackageResult> Packages { get; set; } = new List<PackageResult>();
    }

    public class PackageResult
    {
        public string Name { get; set; }
        private SemVersion _currentVersion;
        private SemVersion _latestStableVersion { get; set; }
        private SemVersion _latestVersion { get; set; }

        public string CurrentVersion { get => _currentVersion.ToString(); set => _currentVersion = (SemVersion)value; }
        public string LatestStableVersion { get => _latestStableVersion.ToString(); set => _latestStableVersion = (SemVersion)value; }
        public string LatestVersion { get => _latestVersion.ToString(); set => _latestVersion = (SemVersion)value; }

        public bool UpToDate => _currentVersion >= _latestStableVersion;
        public bool IsPreRelease => _currentVersion.Prerelease != string.Empty;
    }
}
