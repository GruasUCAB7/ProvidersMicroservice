using Moq;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Queries.GetById;
using ProvidersMS.src.Providers.Application.Queries.GetById.Types;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Providers.Application.Queries.GetById
{
    public class GetProviderByIdQueryHandlerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;

        public GetProviderByIdQueryHandlerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
        }

        [Fact]
        public async Task GetProviderByIdSuccess()
        {
            var query = new GetProviderByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var handler = new GetProviderByIdQueryHandler(_providerRepositoryMock.Object);
            var provider = Provider.CreateProvider(
                new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                        new ProviderRif("V000000000"),
                        new ProviderType("Interno"),
                        new List<CraneId>(),
                        new List<DriverId>()
            );

            _providerRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Provider>.Of(provider));

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetProviderByIdWhenProviderNotFound()
        {
            var query = new GetProviderByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var handler = new GetProviderByIdQueryHandler(_providerRepositoryMock.Object);

            _providerRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Provider>.Empty());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Provider not found", result.ErrorMessage);
        }
    }
}
