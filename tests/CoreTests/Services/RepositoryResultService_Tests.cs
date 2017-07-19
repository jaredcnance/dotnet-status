using Core.Services;
using DotnetStatus.Core.Data;
using DotnetStatus.Core.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CoreTests.Services
{
    public class RepositoryResultService_Tests
    {
        [Fact]
        public async Task FindAsync_Returns_From_Cache_If_Available()
        {
            // arrange
            var url = Guid.NewGuid().ToString();
            var cacheMock = new Mock<ICache>();
            cacheMock.Setup(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)))
                .ReturnsAsync(new RepositoryResult());
            var dbMock = new Mock<IRepositoryResultPersistence>();
            var service = new RepositoryResultService(cacheMock.Object, dbMock.Object);

            // act
            await service.FindAsync(url);

            // assert
            cacheMock.Verify(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)), Times.Once);
            dbMock.Verify(m => m.GetAsync(It.IsAny<string>()), Times.Never);
            cacheMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task FindAsync_Returns_From_Db_If_Cache_DNE()
        {
            // arrange
            var url = Guid.NewGuid().ToString();
            var cacheMock = new Mock<ICache>();
            cacheMock.Setup(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)))
                .ReturnsAsync((RepositoryResult)null);
            var dbMock = new Mock<IRepositoryResultPersistence>();
            var service = new RepositoryResultService(cacheMock.Object, dbMock.Object);

            // act
            await service.FindAsync(url);

            // assert
            cacheMock.Verify(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)), Times.Once);
            dbMock.Verify(m => m.GetAsync(It.Is<string>(s => s == url)), Times.Once);
            cacheMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task FindAsync_Caches_Result_If_Found_In_Db()
        {
            // arrange
            var url = Guid.NewGuid().ToString();
            var cacheMock = new Mock<ICache>();
            cacheMock.Setup(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)))
                .ReturnsAsync((RepositoryResult)null);

            var dbMock = new Mock<IRepositoryResultPersistence>();
            dbMock.Setup(m => m.GetAsync(It.Is<string>(s => s == url)))
                .ReturnsAsync(new RepositoryResult());

            var service = new RepositoryResultService(cacheMock.Object, dbMock.Object);

            // act
            await service.FindAsync(url);

            // assert
            cacheMock.Verify(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)), Times.Once);
            dbMock.Verify(m => m.GetAsync(It.Is<string>(s => s == url)), Times.Once);
            cacheMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task SetStatusAsync_Creates_New_Result_If_DNE()
        {
            // arrange
            var url = Guid.NewGuid().ToString();
            var cacheMock = new Mock<ICache>();
            const EvaluationStatus expectedStatus = EvaluationStatus.Complete;

            var dbMock = new Mock<IRepositoryResultPersistence>();
            dbMock.Setup(m => m.GetAsync(It.Is<string>(s => s == url)))
                .ReturnsAsync((RepositoryResult)null);

            var result = new RepositoryResult();
            dbMock.Setup(m => m.SaveAsync(It.IsAny<RepositoryResult>()))
                .Callback<RepositoryResult>(r => result = r)
                .Returns(Task.FromResult(default(object)));

            var service = new RepositoryResultService(cacheMock.Object, dbMock.Object);

            // act
            await service.SetStatusAsync(url, expectedStatus);

            // assert
            dbMock.Verify(m => m.GetAsync(url), Times.Once);
            Assert.Equal(expectedStatus, result.EvaluationStatus);
        }

        [Fact]
        public async Task SetStatusAsync_Sets_Status_On_Result()
        {
            // arrange
            var url = Guid.NewGuid().ToString();
            var cacheMock = new Mock<ICache>();
            const EvaluationStatus expectedStatus = EvaluationStatus.Complete;

            var result = new RepositoryResult();

            var dbMock = new Mock<IRepositoryResultPersistence>();
            dbMock.Setup(m => m.GetAsync(It.Is<string>(s => s == url)))
                .ReturnsAsync(result);

            var service = new RepositoryResultService(cacheMock.Object, dbMock.Object);

            // act
            await service.SetStatusAsync(url, expectedStatus);

            // assert
            dbMock.Verify(m => m.GetAsync(url), Times.Once);
            Assert.Equal(expectedStatus, result.EvaluationStatus);
        }
    }
}
