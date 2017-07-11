using NuGet.Versioning;
using System.Collections.Generic;

namespace DotnetStatus.Core.Models
{
    public class PackageStatus
    {
        public NuGetVersion LatestStable { get; set; }
        public NuGetVersion Latest { get; set; }
        public string LatestSource { get; set; }
        public string LatestStableSource { get; set; }
        public List<NuGetVersion> AllVersions { get; set; } = new List<NuGetVersion>();
    }
}
