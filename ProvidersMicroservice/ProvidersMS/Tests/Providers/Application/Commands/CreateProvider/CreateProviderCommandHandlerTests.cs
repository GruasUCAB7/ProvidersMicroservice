using Moq;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Providers.Application.Commands.CreateProvider
{
    public class CreateProviderCommandHandlerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;
        private readonly Mock<ICraneRepository> _craneRepositoryMock;
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly Mock<IdGenerator<string>> _idGeneratorMock;
        private readonly CreateProviderCommandHandler _handler;

        public CreateProviderCommandHandlerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
            _craneRepositoryMock = new Mock<ICraneRepository>();
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _idGeneratorMock = new Mock<IdGenerator<string>>();
            _handler = new CreateProviderCommandHandler(_providerRepositoryMock.Object, _craneRepositoryMock.Object, _driverRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldCreateProviderSuccess()
        {
            var command = new CreateProviderCommand("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "V000000000", "Interno", new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" });

            _providerRepositoryMock.Setup(x => x.ExistByRif(command.Rif)).ReturnsAsync(false);

            var provider = Provider.CreateProvider(
                new ProviderId(command.UserId),
                new ProviderRif(command.Rif),
                new ProviderType(command.ProviderType),
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

            _craneRepositoryMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepositoryMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepositoryMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<Driver>.Of(driver));
            _providerRepositoryMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ReturnsAsync(false);
            _providerRepositoryMock.Setup(x => x.Save(provider)).ReturnsAsync(Result<Provider>.Success(provider));

            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task ShouldFailToCreateProviderWhenProviderAlreadyExists()
        {
            var command = new CreateProviderCommand("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d", "V000000000", "Interno", new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" });

            _providerRepositoryMock.Setup(x => x.ExistByRif(command.Rif)).ReturnsAsync(true);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal($"Provider with plate V000000000 already exist", result.ErrorMessage);
        }
    }
}