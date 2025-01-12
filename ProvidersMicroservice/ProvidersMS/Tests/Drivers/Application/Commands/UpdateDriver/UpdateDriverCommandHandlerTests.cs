using Moq;
using Xunit;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using static ProvidersMS.Core.Infrastructure.GoogleMaps.GoogleApiService;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver;
using ProvidersMS.src.Drivers.Application.Exceptions;

namespace ProvidersMS.Tests.Drivers.Application.Commands.UpdateDriver
{
    public class UpdateDriverCommandHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly Mock<IGoogleApiService> _googleApiServiceMock;
        private readonly UpdateDriverCommandHandler _handler;

        public UpdateDriverCommandHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _googleApiServiceMock = new Mock<IGoogleApiService>();
            _handler = new UpdateDriverCommandHandler(_driverRepositoryMock.Object, _googleApiServiceMock.Object);
        }

        [Fact]
        public async Task ShouldUpdateDriverSuccess()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverCommand(true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", true, "El paraiso, caracas", true);

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Success(new Coordinates { Latitude = 10.0, Longitude = 10.0 }));
            _driverRepositoryMock.Setup(x => x.Update(driver)).ReturnsAsync(Result<Driver>.Success(driver));

            var result = await _handler.Execute((driverId, command));

            Assert.True(result.IsSuccessful);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _googleApiServiceMock.Verify(x => x.GetCoordinatesFromAddress(command.DriverLocation), Times.Once);
            _driverRepositoryMock.Verify(x => x.Update(driver), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenDriverNotFound()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverCommand(true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", true, "El paraiso, caracas", true);

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _handler.Execute((driverId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver not found.", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenCoordinatesrNotFound()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverCommand(true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", true, "El paraiso, caracas", true);

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Failure(new CoordinatesNotFoundException("Driver location not found.")));

            var result = await _handler.Execute((driverId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver location not found.", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _googleApiServiceMock.Verify(x => x.GetCoordinatesFromAddress(command.DriverLocation), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenUpdateFails()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverCommand(true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", true, "El paraiso, caracas", true);

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Success(new Coordinates { Latitude = 10.0, Longitude = 10.0 }));
            _driverRepositoryMock.Setup(x => x.Update(driver)).ReturnsAsync(Result<Driver>.Failure(new DriverUpdateFailedException()));

            var result = await _handler.Execute((driverId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("The driver could not be updated correctly", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _driverRepositoryMock.Verify(x => x.Update(driver), Times.Once);
        }
    }
}