using Moq;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.src.Cranes.Application.Queries.GetById;
using ProvidersMS.src.Cranes.Application.Queries.GetById.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Cranes.Application.Queries
{
    public class GetCraneByIdQueryHandlerTests
    {
        private readonly Mock<ICraneRepository> _craneRepositoryMock;

        public GetCraneByIdQueryHandlerTests()
        {
            _craneRepositoryMock = new Mock<ICraneRepository>();
        }

        [Fact]
        public async Task GetCraneByIdSuccess()
        {
            var query = new GetCraneByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var handler = new GetCraneByIdQueryHandler(_craneRepositoryMock.Object);
            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Crane>.Of(crane));

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetCraneByIdWhenCraneNotFound()
        {
            var query = new GetCraneByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f9f");
            var handler = new GetCraneByIdQueryHandler(_craneRepositoryMock.Object);

            _craneRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Optional<Crane>.Empty());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Crane not found", result.ErrorMessage);
        }
    }
}
