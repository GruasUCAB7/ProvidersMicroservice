using Xunit;
using Moq;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetAll;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;


namespace ProvidersMS.Tests.Providers.Application.Queries.GetAll
{
    public class GetAllProvidersQueryHandlerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;

        public GetAllProvidersQueryHandlerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
        }

        [Fact]
        public async Task GetAllProviderSuccess()
        {
            var query = new GetAllProvidersQuery(1, 5, "");
            var handler = new GetAllProvidersQueryHandler(_providerRepositoryMock.Object);
            var providers = new List<Provider>
                {
                    Provider.CreateProvider(
                        new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                        new ProviderRif("V000000000"),
                        new ProviderType("Interno"),
                        new List<CraneId>(),
                        new List<DriverId>()
                    ),
                    Provider.CreateProvider(
                        new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f6t"),
                        new ProviderRif("V000000002"),
                        new ProviderType("Externo"),
                        new List<CraneId>(),
                        new List<DriverId>()
                    )
                };


            _providerRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(providers);
            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.Unwrap().Length);
        }

        [Fact]
        public async Task GetAllProvidersFailureWhenNoProvidersFound()
        {
            var query = new GetAllProvidersQuery(1, 5, "");
            var handler = new GetAllProvidersQueryHandler(_providerRepositoryMock.Object);

            _providerRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(new List<Provider>());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Unwrap());
        }
    }
}
