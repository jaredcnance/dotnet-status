using Core.Services;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.NuGet;
using Moq;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoreTests.Services.NuGet
{
    public class DependencyGraphService_Tests
    {
        [Fact]
        public void GetProjectResults_Loads_Graph_For_Each_Project()
        {
            // arrange
            var packageStatusStoreMock = GetStore();
            var typedReaderMock = GetReader();

            var dependencyGraphService = new DependencyGraphService(packageStatusStoreMock.Object, typedReaderMock.Object);

            // act
            var projectResults = dependencyGraphService.GetProjectResults(string.Empty);

            // assert
            Assert.Equal(2, projectResults.Count);
            var proj = projectResults.Single(p => p.Name == nameof(CoreTests));
            Assert.NotNull(proj);
            Assert.Equal(true, proj.OutOfDate);
            Assert.True(proj.OutOfDatePackages.Count > 0);
            Assert.Equal(proj.OutOfDatePackages.Count, proj.Frameworks.SelectMany(f => f.Packages).Count());
        }

        [Fact]
        public void GetProjectResults_Reloads_Sources_ForEach_Project()
        {
            // arrange
            var packageStatusStoreMock = GetStore();
            var typedReaderMock = GetReader();

            var dependencyGraphService = new DependencyGraphService(packageStatusStoreMock.Object, typedReaderMock.Object);

            // act
            var projectResults = dependencyGraphService.GetProjectResults(string.Empty);

            // assert
            packageStatusStoreMock.Verify(m => m.ReloadSources(It.IsAny<IList<PackageSource>>()), Times.Exactly(projectResults.Count));
        }

        private Mock<IPackageStatusStore> GetStore()
        {
            var packageStatusStoreMock = new Mock<IPackageStatusStore>();
            packageStatusStoreMock.Setup(m => m.GetStatus(It.IsAny<string>())).Returns(new PackageStatus
            {
                // won't be able to find a match, so all packages _should_ be considered out-of-date
                AllVersions = new List<NuGetVersion> { new NuGetVersion("0.0.0") },
                Latest = new NuGetVersion("0.0.0"),
                LatestSource = string.Empty,
                LatestStable = new NuGetVersion("0.0.0"),
                LatestStableSource = string.Empty
            });

            return packageStatusStoreMock;
        }

        private Mock<ITypedReader> GetReader()
        {
            var json = JObject.Parse(ResourceTestUtility.GetResource("CoreTests.resources.test.dg", typeof(DependencyGraphService_Tests)));

            var typedReaderMock = new Mock<ITypedReader>();
            typedReaderMock.Setup(m => m.ReadAt<JObject>(It.IsAny<string>()))
                .Returns(json);

            return typedReaderMock;
        }
    }
}
