using System.Collections.Generic;
using System.Linq;

namespace DotnetStatus.Core
{
    public class ProjectResult
    {
        public string Name { get; set; }
        public List<FrameworkResult> Frameworks { get; set; } = new List<FrameworkResult>();
        public bool OutOfDate => OutOfDatePackages.Any();
        public List<PackageResult> OutOfDatePackages => Frameworks.SelectMany(f => f.Packages.Where(p => p.IsUpToDate == false)).ToList();
    }
}
