using Xunit;
using Moq;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetAll;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables;
using ProvidersMS.src.Providers.Application.Models;


namespace ProvidersMS.Tests.Providers.Application.Queries.GetDriversAvailables
{
    public class GetDriversAvailablesQueryHandlerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;

        public GetDriversAvailablesQueryHandlerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
        }

        [Fact]
        public async Task GetAvailableDriversSuccess()
        {
            var query = new GetAvailableDriversQuery(1, 5, "");
            var handler = new GetAvailableDriversQueryHandler(_providerRepositoryMock.Object);
            var availableDrivers = new List<Driver>
            {
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f6r"),
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f3w", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f7k"),
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f6j", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f9p")
            };


            _providerRepositoryMock.Setup(x => x.GetAvailableDrivers(query)).ReturnsAsync(availableDrivers);
            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(3, result.Unwrap().Length);
        }

        [Fact]
        public async Task GetAllProvidersFailureWhenNoProvidersFound()
        {
            var query = new GetAvailableDriversQuery(1, 5, "");
            var handler = new GetAvailableDriversQueryHandler(_providerRepositoryMock.Object);

            _providerRepositoryMock.Setup(x => x.GetAvailableDrivers(query)).ReturnsAsync(new List<Driver>());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Unwrap());
        }
    }
}
