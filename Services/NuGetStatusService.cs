using System;
using System.Threading.Tasks;
using DotnetHealth.Services.Http;
using Semver;
using System.Collections.Generic;
using System.Linq;

namespace DotnetHealth.Services
{
    public class NuGetStatusService : IDisposable
    {
        private readonly XmlClient _xmlClient;
        private readonly JsonClient _jsonClient;

        public NuGetStatusService()
        {
            _xmlClient = new XmlClient();
            _jsonClient = new JsonClient();
        }

        public async Task<Result> GetStatusAsync(string csprojUrl)
        {
            var csproj = await _xmlClient.GetAsync<Csproj>(csprojUrl);

            var result = new Result();
            foreach(var package in csproj.ItemGroups.SelectMany(ig => ig.PackageReferences))
            {
                var uri = $"https://api.nuget.org/v3-flatcontainer/{package.Include}/index.json";
                var nuget = await _jsonClient.GetAsync<Nuget>(uri);
                var latestStable = GetLatestStableVersion(nuget.Versions);
                result.Packages.Add(new PackageResult {
                    Name = package.Include,
                    CurrentVersion = package.Version,
                    LatestVersion = nuget.Versions.Last(),
                    LatestStableVersion = latestStable
                });
            }

            return result;
        }

        private string GetLatestStableVersion(List<string> versions)
        {
            SemVersion latest = "0.0";
            foreach(var version in versions)
            {
                var semVersion = SemVersion.Parse(version);
                if(semVersion > latest && semVersion.Prerelease == string.Empty)
                    latest = semVersion;
            }
            return latest.ToString();
        }

        public void Dispose()
        {
            _xmlClient.Dispose();
            _jsonClient.Dispose();
        }
    }
}
