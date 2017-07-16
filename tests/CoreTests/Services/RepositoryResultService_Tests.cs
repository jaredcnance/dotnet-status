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
            var service = new RepositoryStatusService(cacheMock.Object, dbMock.Object);

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
            var service = new RepositoryStatusService(cacheMock.Object, dbMock.Object);

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

            var service = new RepositoryStatusService(cacheMock.Object, dbMock.Object);

            // act
            await service.FindAsync(url);

            // assert
            cacheMock.Verify(m => m.GetAsync<RepositoryResult>(It.Is<string>(s => s == url)), Times.Once);
            dbMock.Verify(m => m.GetAsync(It.Is<string>(s => s == url)), Times.Once);
            cacheMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }
    }
}
