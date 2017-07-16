using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.NuGet;
using Moq;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreTests.Services.NuGet
{
    public class PackageStatusStore_Tests
    {
        [Fact]
        public void ReloadSources_LoadsRepositoryResources()
        {
            // arrange
            var repositoryMoq = new Mock<IPackageStatusRepository>();
            var store = new PackageStatusStore(repositoryMoq.Object);
            var sources = new List<PackageSource>();

            // act
            store.ReloadSources(sources);

            // assert
            repositoryMoq.Verify(m => m.LoadPackageMetadataResources(It.IsAny<IList<PackageSource>>()));
        }

        [Fact]
        public void GetStatus_Caches_Requests()
        {
            // arrange
            var repositoryMoq = new Mock<IPackageStatusRepository>();
            repositoryMoq.Setup(m => m.GetStatus(It.IsAny<string>())).Returns(new PackageStatus());

            var store = new PackageStatusStore(repositoryMoq.Object);
            var packageId = Guid.NewGuid().ToString();

            // act
            var status1 = store.GetStatus(packageId);
            var status2 = store.GetStatus(packageId);

            // assert
            repositoryMoq.Verify(m => m.GetStatus(It.Is<string>(s => s == packageId)), Times.Once);
            Assert.Equal(status1, status2);
        }
    }
}
