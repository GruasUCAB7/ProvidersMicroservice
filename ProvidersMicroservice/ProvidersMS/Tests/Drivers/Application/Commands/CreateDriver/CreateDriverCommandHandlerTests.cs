using Moq;
using Xunit;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Commands.CreateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.CreateDriver;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using static ProvidersMS.Core.Infrastructure.GoogleMaps.GoogleApiService;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Exceptions;

namespace ProvidersMS.Tests.Drivers.Application.Commands.CreateDriver
{
    public class CreateDriverCommandHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly Mock<ICraneRepository> _craneRepositoryMock;
        private readonly Mock<IGoogleApiService> _googleApiServiceMock;
        private readonly CreateDriverCommandHandler _handler;

        public CreateDriverCommandHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _craneRepositoryMock = new Mock<ICraneRepository>();
            _googleApiServiceMock = new Mock<IGoogleApiService>();
            _handler = new CreateDriverCommandHandler(_driverRepositoryMock.Object, _craneRepositoryMock.Object, _googleApiServiceMock.Object);
        }

        [Fact]
        public async Task ShouldCreateDriverSuccess()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            _driverRepositoryMock.Setup(x => x.ExistByDNI(command.DNI)).ReturnsAsync(false);

            var craneOptional = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(command.CraneAssigned)).ReturnsAsync(Optional<Crane>.Of(craneOptional));
            _craneRepositoryMock.Setup(x => x.IsActiveCrane(command.CraneAssigned)).ReturnsAsync(true);
            _driverRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherDriver(command.CraneAssigned)).ReturnsAsync(false);
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Success(new Coordinates { Latitude = 10.0, Longitude = 10.0 }));

            var result = await _handler.Execute(command);

            var driver = Driver.CreateDriver(
                new DriverId(command.UserId),
                new DriverDNI(command.DNI),
                new DriverIsActiveLicensed(command.IsActiveLicensed),
                new List<string>(),
                new CraneId(command.CraneAssigned),
                new DriverLocation(10.0, 10.0)
            );

            _driverRepositoryMock.Setup(x => x.Save(driver)).ReturnsAsync(Result<Driver>.Success(driver));

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task ShouldFailToCreateDriverWhenDriverAlreadyExist()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            _driverRepositoryMock.Setup(x => x.ExistByDNI(command.DNI)).ReturnsAsync(true);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal($"Driver with dni 00000000 already exist", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToCreateDriverWhenCraneNotFound()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            _craneRepositoryMock.Setup(x => x.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d")).ReturnsAsync(Optional<Crane>.Empty());
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Crane not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToCreateDriverWhenCraneIsAssignedInOtherDriver()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            var craneOptional = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(command.CraneAssigned)).ReturnsAsync(Optional<Crane>.Of(craneOptional));
            _craneRepositoryMock.Setup(x => x.IsActiveCrane(command.CraneAssigned)).ReturnsAsync(true);
            _driverRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherDriver(command.CraneAssigned)).ReturnsAsync(true);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("This crane is already assigned to this driver.", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToCreateDriverWhenCraneIsNotActive()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            var craneOptional = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(command.CraneAssigned)).ReturnsAsync(Optional<Crane>.Of(craneOptional));
            _craneRepositoryMock.Setup(x => x.IsActiveCrane(command.CraneAssigned)).ReturnsAsync(false);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Crane not available", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToCreateDriverWhenLocationIsNotFound()
        {
            var command = new CreateDriverCommand("7603741d-2d93-448c-b9ea-9ec3ba21cea8", "00000000", true, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "El paraiso, caracas");

            var craneOptional = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(command.CraneAssigned)).ReturnsAsync(Optional<Crane>.Of(craneOptional));
            _craneRepositoryMock.Setup(x => x.IsActiveCrane(command.CraneAssigned)).ReturnsAsync(true);
            _driverRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherDriver(command.CraneAssigned)).ReturnsAsync(false);
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Failure(new CoordinatesNotFoundException("Driver location not found.")));
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver location not found.", result.ErrorMessage);
        }
    }
}