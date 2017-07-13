using Core.Services.Git;
using DotnetStatus.Core.Configuration;
using DotnetStatus.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using Xunit;

namespace CoreTests.Services.Git
{
    public class TransientGitService_Tests
    {
        [Fact]
        public void GetSource_Clones_Repository()
        {
            // arrange
            var config = new WorkerConfiguration
            {
                SourceRootDirectory = ".",
                MaxCleanupAttempts = 1
            };

            var optionsMock = new Mock<IOptions<WorkerConfiguration>>();
            optionsMock.Setup(m => m.Value).Returns(config);

            var gitServiceMock = new Mock<IGitCloneService>();

            var transientGitService = new TransientGitService(optionsMock.Object, gitServiceMock.Object);

            var repoUrl = string.Empty;

            // act
            var path = transientGitService.GetSource(repoUrl);

            // assert
            Assert.NotNull(path);
            gitServiceMock.Verify(m => m.Clone(It.Is<string>(s => s == repoUrl), It.IsAny<string>()));

            Assert.True(Directory.Exists(path));
            transientGitService.Dispose();
            Assert.False(Directory.Exists(path));
        }
    }
}
