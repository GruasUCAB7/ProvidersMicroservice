using Xunit;
using Moq;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Drivers.Application.Queries.GetAll;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.ValueObjects;


namespace ProvidersMS.Tests.Drivers.Application.Queries
{
    public class GetAllDriversQueryHandlerTests
    {
        private readonly Mock<IDriverRepository> _driverRepositoryMock;
        private readonly GetAllDriversQueryHandler _handler;

        public GetAllDriversQueryHandlerTests()
        {
            _driverRepositoryMock = new Mock<IDriverRepository>();
            _handler = new GetAllDriversQueryHandler(_driverRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllDriversSuccess()
        {
            var query = new GetAllDriversQuery(1, 5, "", "");
            var drivers = new List<Driver>
                {
                    Driver.CreateDriver(
                        new DriverId("5d6ec69f-c63b-45a3-8fe6-72d10e1d06b5"),
                        new DriverDNI("12345678"),
                        new DriverIsActiveLicensed(true),
                        new List<string>(),
                        new CraneId("5d6ec69f-c63b-45a3-8fe6-72d10e1d06b5"),
                        new DriverLocation(1.0, 1.0)
                    ),
                   Driver.CreateDriver(
                        new DriverId("5d6ec69f-c63b-45a3-8fe6-72d10e1d0674"),
                        new DriverDNI("12345678"),
                        new DriverIsActiveLicensed(true),
                        new List<string>(),
                        new CraneId("5d6ec69f-c63b-45a3-8fe6-72d10e1d0614"),
                        new DriverLocation(1.0, 1.0)
                    )
                };


            _driverRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(drivers);
            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.Unwrap().Length);
        }

        [Fact]
        public async Task GetAllDriversFailureWhenNoDriversFound()
        {
            var query = new GetAllDriversQuery(1, 5, "", "");

            _driverRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(new List<Driver>());

            var result = await _handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Unwrap());
        }
    }
}
