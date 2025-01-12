using Moq;
using Xunit;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Commands.DisconnectDriver;

namespace ProvidersMS.Tests.Drivers.Application.Commands.DisconnectDriver
{
    public class DisconnectDriverCommandHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;

        public DisconnectDriverCommandHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
        }

        [Fact]
        public async Task ShouldDisconnectDriverSuccess()
        {
            _driverRepositoryMock.Setup(x => x.ValidateUpdateTimeDriver()).Returns(Task.CompletedTask);

            var handler = new DisconnectDriverCommandHandler(_driverRepositoryMock.Object);

            var result = await handler.Execute();

            Assert.True(result);
            _driverRepositoryMock.Verify(x => x.ValidateUpdateTimeDriver(), Times.Once);
        }
    }
}