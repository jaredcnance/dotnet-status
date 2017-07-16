namespace DotnetStatus.Core.Models
{
    public class PackageResult
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public PackageVersion LatestVersion { get; set; }
        public PackageVersion LatestStableVersion { get; set; }
        public PackageVersion ResolvedVersion { get; set; }
        public bool IsPreRelease { get; set; }
        public bool IsUpToDate { get; set; }
        public bool AutoReferenced { get; set; }
    }
}
