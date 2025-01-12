using Moq;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Queries.GetById;
using ProvidersMS.src.Drivers.Application.Queries.GetById.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Drivers.Application.Queries
{
    public class GetDriverByIdQueryHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly GetDriverByIdQueryHandler _handler;

        public GetDriverByIdQueryHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _handler = new GetDriverByIdQueryHandler(_driverRepositoryMock.Object);
        }

        [Fact]
        public async Task GetDriverByIdSuccess()
        {
            var query = new GetDriverByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var driver = Driver.CreateDriver(
                    new DriverId("5d6ec69f-c63b-45a3-8fe6-72d10e1d06b5"),
                    new DriverDNI("12345678"),
                    new DriverIsActiveLicensed(true),
                    new List<string>(),
                    new CraneId("5d6ec69f-c63b-45a3-8fe6-72d10e1d06b5"),
                    new DriverLocation(1.0, 1.0)
            );

            _driverRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Driver>.Of(driver));

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetDriverByIdWhenDriverNotFound()
        {
            var query = new GetDriverByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _driverRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Driver not found.", result.ErrorMessage);
        }
    }
}
