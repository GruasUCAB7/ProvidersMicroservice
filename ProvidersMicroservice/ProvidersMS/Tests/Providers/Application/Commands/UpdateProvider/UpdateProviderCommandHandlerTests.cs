using Moq;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Providers.Application.Commands.UpdateProvider
{
    public class UpdateProviderCommandHandlerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;
        private readonly Mock<ICraneRepository> _craneRepositoryMock;
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly UpdateProviderCommandHandler _handler;

        public UpdateProviderCommandHandlerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
            _craneRepositoryMock = new Mock<ICraneRepository>();
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _handler = new UpdateProviderCommandHandler(_providerRepositoryMock.Object, _craneRepositoryMock.Object, _driverRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldUpdateDriverSucces()
        {
            var providerId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);

            var provider = Provider.CreateProvider(
                new ProviderId(providerId),
                new ProviderRif("V000000000"),
                new ProviderType("Interno"),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            var driver = Driver.CreateDriver(
                new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"),
                new DriverDNI("00000000"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new DriverLocation(10.0, 10.0)
            );

            _providerRepositoryMock.Setup(x => x.GetById(providerId)).ReturnsAsync(Optional<Provider>.Of(provider));
            _craneRepositoryMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepositoryMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<Driver>.Of(driver));
            _providerRepositoryMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ReturnsAsync(false);
            _providerRepositoryMock.Setup(x => x.Update(provider)).ReturnsAsync(Result<Provider>.Success(provider));

            var result = await _handler.Execute((providerId, command));

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(providerId, result.Unwrap().Id);
            Assert.Equal("V000000000", result.Unwrap().Rif);
            Assert.Equal("Interno", result.Unwrap().ProviderType);
            Assert.Equal(["53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"], result.Unwrap().FleetOfCranes);
            Assert.Equal(["53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"], result.Unwrap().Drivers);
        }

        [Fact]
        public async Task ShouldFailToUpdateProviderWhenProviderNotFound()
        {
            var providerId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);

            _providerRepositoryMock.Setup(x => x.GetById(providerId)).ReturnsAsync(Optional<Provider>.Empty());

            var result = await _handler.Execute((providerId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Provider not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToUpdateProviderWhenUpdateFails()
        {
            var providerId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);

            var provider = Provider.CreateProvider(
                new ProviderId(providerId),
                new ProviderRif("V000000000"),
                new ProviderType("Interno"),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            var driver = Driver.CreateDriver(
                new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"),
                new DriverDNI("00000000"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new DriverLocation(10.0, 10.0)
            );

            _providerRepositoryMock.Setup(x => x.GetById(providerId)).ReturnsAsync(Optional<Provider>.Of(provider));
            _craneRepositoryMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepositoryMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<Driver>.Of(driver));
            _providerRepositoryMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ReturnsAsync(false);
            _providerRepositoryMock.Setup(x => x.Update(provider)).ReturnsAsync(Result<Provider>.Failure(new ProviderUpdateFailedException()));

            var result = await _handler.Execute((providerId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("The provider could not be updated correctly", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToUpdateProviderWhenCraneIsAssignedInOtherProvider()
        {
            var providerId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);

            var provider = Provider.CreateProvider(
                new ProviderId(providerId),
                new ProviderRif("V000000000"),
                new ProviderType("Interno"),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _providerRepositoryMock.Setup(x => x.GetById(providerId)).ReturnsAsync(Optional<Provider>.Of(provider));
            _craneRepositoryMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ThrowsAsync(new CraneAlreadyAssociatedException(crane.GetId()));

            var exception = await Assert.ThrowsAsync<CraneAlreadyAssociatedException>(() => _handler.Execute((providerId, command)));

            Assert.Equal("Crane with id: 53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r already associated with another Provider.", exception.Message);
        }

        [Fact]
        public async Task ShouldFailToUpdateProviderWhenDriverIsAssignedInOtherProvider()
        {
            var providerId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);

            var provider = Provider.CreateProvider(
                new ProviderId(providerId),
                new ProviderRif("V000000000"),
                new ProviderType("Interno"),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            var driver = Driver.CreateDriver(
                new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"),
                new DriverDNI("00000000"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new DriverLocation(10.0, 10.0)
            );

            _providerRepositoryMock.Setup(x => x.GetById(providerId)).ReturnsAsync(Optional<Provider>.Of(provider));
            _craneRepositoryMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepositoryMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<Driver>.Of(driver));
            _providerRepositoryMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ThrowsAsync(new DriverAlreadyAssociatedException(driver.GetId()));

            var exception = await Assert.ThrowsAsync<DriverAlreadyAssociatedException>(() => _handler.Execute((providerId, command)));

            Assert.Equal("Driver with id: 53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r already associated with another Provider.", exception.Message);
        }

    }
}
