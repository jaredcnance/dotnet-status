using System.Collections.Generic;

namespace DotnetStatus.Core.Configuration
{
    public class WorkerConfiguration
    {
        public string SourceRootDirectory { get; set; }
        public string DependencyGraphFileName { get; set; }
        public string NuGetPath { get; set; }
        public List<string> RestoreArguments { get; set; } = new List<string>();
        public int RestoreTimeoutMilliseconds { get; set; }
        public int CleanupTimeout { get; set; }
        public int MaxCleanupAttempts { get; set; }
    }
}
