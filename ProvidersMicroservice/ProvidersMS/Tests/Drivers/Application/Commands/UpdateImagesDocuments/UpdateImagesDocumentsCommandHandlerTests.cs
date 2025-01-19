using Moq;
using Xunit;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;

namespace ProvidersMS.Tests.Drivers.Application.Commands.UpdateImagesDocuments
{
    public class UpdateImagesDocumentsCommandHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly UpdateImagesDocumentsCommandHandler _handler;

        public UpdateImagesDocumentsCommandHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _handler = new UpdateImagesDocumentsCommandHandler(_driverRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldUpdateDriverSuccess()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverImagesCommand(driverId, new List<string> { "image1", "image2" });

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _driverRepositoryMock.Setup(x => x.Update(driver)).ReturnsAsync(Result<Driver>.Success(driver));

            var result = await _handler.Execute(command);

            Assert.True(result.IsSuccessful);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _driverRepositoryMock.Verify(x => x.Update(driver), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenDriverNotFound()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverImagesCommand(driverId, new List<string> { "image1", "image2" });

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver not found.", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenUpdateFails()
        {
            var driverId = "7603741d-2d93-448c-b9ea-9ec3ba21cea8";
            var command = new UpdateDriverImagesCommand(driverId, new List<string> { "image1", "image2" });

            var driver = Driver.CreateDriver(
                new DriverId(driverId),
                new DriverDNI("29611513"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(driverId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _driverRepositoryMock.Setup(x => x.Update(driver)).ReturnsAsync(Result<Driver>.Failure(new UnsavedImagesException()));

            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("There was an error saving the images urls.", result.ErrorMessage);
            _driverRepositoryMock.Verify(x => x.GetById(driverId), Times.Once);
            _driverRepositoryMock.Verify(x => x.Update(driver), Times.Once);
        }
    }
}