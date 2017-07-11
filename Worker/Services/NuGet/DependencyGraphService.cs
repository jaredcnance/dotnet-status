using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;
using System.Collections.Generic;
using System.IO;
using NuGet.LibraryModel;
using DotnetStatus.Core;

namespace DotnetStatus.Worker.Services.NuGet
{
    class DependencyGraphService : IDependencyGraphService
    {
        private readonly IPackageStatusStore _packageStatusStore;

        public DependencyGraphService(IPackageStatusStore packageStatusStore)
        {
            _packageStatusStore = packageStatusStore;
        }

        public List<ProjectResult> GetProjectResults(string dependencyGraphPath)
        {
            var dg = GetDependencyGraph(dependencyGraphPath);

            var projectResults = new List<ProjectResult>();
            foreach (var proj in dg.Projects)
                projectResults.Add(GetProjectResult(proj));

            return projectResults;
        }

        private DependencyGraphSpec GetDependencyGraph(string path)
        {
            var json = File.ReadAllTextAsync(path).GetAwaiter().GetResult();
            var obj = (JObject)JsonConvert.DeserializeObject(json);
            return new DependencyGraphSpec(obj);
        }

        private ProjectResult GetProjectResult(PackageSpec proj)
        {
            var projectResult = new ProjectResult
            {
                Name = proj.Name
            };

            var sources = proj.RestoreMetadata.Sources;
            _packageStatusStore.ReloadSources(sources);

            foreach (var fw in proj.TargetFrameworks)
                projectResult.Frameworks.Add(GetFrameworkResult(fw));

            return projectResult;
        }

        private FrameworkResult GetFrameworkResult(TargetFrameworkInformation fw)
        {
            var frameworkResult = new FrameworkResult
            {
                Name = fw.FrameworkName.Framework
            };

            foreach (var dep in fw.Dependencies)
                frameworkResult.Packages.Add(GetPackageResult(dep));

            return frameworkResult;
        }

        private PackageResult GetPackageResult(LibraryDependency dep)
        {
            var vr = dep.LibraryRange.VersionRange;
            var nugetVersion = _packageStatusStore.GetStatus(dep.Name);
            var result = new PackageResult
            {
                Name = dep.Name,
                CurrentVersion = vr.OriginalString,
                LatestStableVersion = new PackageVersion
                {
                    Source = nugetVersion.LatestStableSource,
                    Version = nugetVersion.LatestStable.ToString()
                },
                LatestVersion = new PackageVersion
                {
                    Source = nugetVersion.LatestSource,
                    Version = nugetVersion.Latest.ToString()
                },
                AutoReferenced = dep.AutoReferenced
            };

            var resolved = vr.FindBestMatch(nugetVersion.AllVersions);

            if (resolved == nugetVersion.Latest)
            {
                result.IsUpToDate = true;
                result.ResolvedVersion = result.LatestVersion;
            }
            else if (resolved == nugetVersion.LatestStable)
            {
                result.IsUpToDate = true;
                result.ResolvedVersion = result.LatestStableVersion;
            }
            else
            {
                result.IsUpToDate = false;
            }

            return result;
        }
    }
}
