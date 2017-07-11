using System.Collections.Generic;

namespace DotnetStatus.Core
{
    public class FrameworkResult
    {
        public string Name { get; set; }
        public List<PackageResult> Packages { get; set; } = new List<PackageResult>();
    }
}
