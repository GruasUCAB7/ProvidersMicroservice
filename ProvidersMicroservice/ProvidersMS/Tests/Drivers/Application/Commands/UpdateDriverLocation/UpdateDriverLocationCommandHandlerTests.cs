using Moq;
using Xunit;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;

namespace ProvidersMS.Tests.Drivers.Application.Commands.UpdateDriverLocation
{
    public class UpdateDriverLocationCommandHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly UpdateDriverLocationCommandHandler _handler;

        public UpdateDriverLocationCommandHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _handler = new UpdateDriverLocationCommandHandler(_driverRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldUpdateDriverLocationSuccess()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverLocationCommand(10.0, 10.0);

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _driverRepositoryMock.Setup(x => x.UpdateDriverLocation(driverId, command.Latitude, command.Longitude)).Returns(Task.CompletedTask);

            var result = await _handler.Execute((driverId, command));

            Assert.True(result.IsSuccessful);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _driverRepositoryMock.Verify(x => x.UpdateDriverLocation(driverId, command.Latitude, command.Longitude), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenDriverNotFound()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverLocationCommand(10.0, 10.0);

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _handler.Execute((driverId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver not found.", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
        }
    }
}