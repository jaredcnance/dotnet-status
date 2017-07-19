using Core.Messaging;
using Core.Services;
using DotnetStatus.Controllers;
using DotnetStatus.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace APITests
{
    public class GitHubPackageStatusController_Tests
    {
        [Theory]
        [InlineData(EvaluationStatus.Complete, typeof(OkObjectResult))]
        [InlineData(EvaluationStatus.Failed, typeof(OkObjectResult))]
        [InlineData(EvaluationStatus.Processing, typeof(AcceptedResult))]
        public async Task GetGithub_Returns_Expected_Status_Based_On_Result_EvaluationStatus(EvaluationStatus evalStatus, Type responseType)
        {
            // arrange
            var logger = new Mock<ILogger<GitHubPackageStatusController>>().Object;
            var publishMock = new Mock<IPublishStringMessage>();
            var repoStatusMock = new Mock<IRepositoryResultService>();
            var expectedRepoResult = new RepositoryResult(string.Empty, evalStatus);
            repoStatusMock.Setup(m => m.FindAsync(It.IsAny<string>())).ReturnsAsync(expectedRepoResult);

            var controller = new GitHubPackageStatusController(logger, publishMock.Object, repoStatusMock.Object);

            // act
            var actionResult = await controller.GetGithub(string.Empty, string.Empty);

            // assert
            Assert.IsType(responseType, actionResult);
            publishMock.Verify(m => m.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetGithub_Queues_Job_If_Result_Not_Found()
        {
            // arrange
            const string gitUser = "jaredcnance";
            const string gitProject = "dotnet-status";
            const string expectedGitRemote = "https://github.com/jaredcnance/dotnet-status.git";

            var logger = new Mock<ILogger<GitHubPackageStatusController>>().Object;
            var publishMock = new Mock<IPublishStringMessage>();

            var repoStatusMock = new Mock<IRepositoryResultService>();
            repoStatusMock.Setup(m => m.FindAsync(It.IsAny<string>())).ReturnsAsync((RepositoryResult)null);

            var controller = new GitHubPackageStatusController(logger, publishMock.Object, repoStatusMock.Object);

            // act
            var actionResult = await controller.GetGithub(gitUser, gitProject);

            // assert
            Assert.IsType<AcceptedResult>(actionResult);
            publishMock.Verify(m => m.PublishMessageAsync(It.IsAny<string>(), expectedGitRemote));
        }

        [Fact]
        public async Task GetGithub_WithProjectName_Filters_By_Project()
        {
            // arrange
            const string gitUser = "jaredcnance";
            const string gitProject = "dotnet-status";

            var logger = new Mock<ILogger<GitHubPackageStatusController>>().Object;
            var publishMock = new Mock<IPublishStringMessage>();

            var projectResults = new List<ProjectResult>
            {
                new ProjectResult { Name = Guid.NewGuid().ToString() },
                new ProjectResult { Name = Guid.NewGuid().ToString() }
            };

            var expectedRepoResult = new RepositoryResult
            {
                EvaluationStatus = EvaluationStatus.Complete,
                ProjectResults = projectResults
            };

            var repoStatusMock = new Mock<IRepositoryResultService>();
            repoStatusMock.Setup(m => m.FindAsync(It.IsAny<string>())).ReturnsAsync(expectedRepoResult);

            var controller = new GitHubPackageStatusController(logger, publishMock.Object, repoStatusMock.Object);
            var expectedProject = projectResults.First();

            // act
            var actionResult = await controller.GetGithub(gitUser, gitProject, expectedProject.Name);

            // assert
            var objResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<RepositoryResult>(objResult.Value);
            Assert.Equal(1, result.ProjectResults.Count);
            Assert.Equal(expectedProject.Name, result.ProjectResults[0].Name);
        }
    }
}
