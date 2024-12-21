using Xunit;
using Moq;
using ProvidersMS.src.Cranes.Application.Queries.GetAll;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain.ValueObjects;


namespace ProvidersMS.Tests.Crane.Application.Queries
{
    public class GetAllCranesQueryHandlerTests
    {
        private readonly Mock<ICraneRepository> _craneRepositoryMock;

        public GetAllCranesQueryHandlerTests()
        {
            _craneRepositoryMock = new Mock<ICraneRepository>();
        }

        [Fact]
        public async Task GetAllCranesSuccess()
        {
            var query = new GetAllCranesQuery(1, 5, "");
            var handler = new GetAllCranesQueryHandler(_craneRepositoryMock.Object);
            var cranes = new List<src.Cranes.Domain.Crane>
                {
                    src.Cranes.Domain.Crane.CreateCrane(
                        new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                        new CraneBrand("Ford"),
                        new CraneModel("Tritón"),
                        new CranePlate("AC123CD"),
                        CraneSizeType.Mediana,
                        new CraneYear(2012)
                    ),
                    src.Cranes.Domain.Crane.CreateCrane(
                        new CraneId("5d6ec69f-c63b-45a3-8fe6-72d10e1d06b5"),
                        new CraneBrand("Toyota"),
                        new CraneModel("Hilux"),
                        new CranePlate("BC234DE"),
                        CraneSizeType.Pesada,
                        new CraneYear(2015)
                    )
                };


            _craneRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(cranes);
            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.Unwrap().Length);
        }

        [Fact]
        public async Task GetAllCranesFailureWhenNoCranesFound()
        {
            var query = new GetAllCranesQuery(1, 5, "");
            var handler = new GetAllCranesQueryHandler(_craneRepositoryMock.Object);

            _craneRepositoryMock.Setup(x => x.GetAll(query)).ReturnsAsync(new List<src.Cranes.Domain.Crane>());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Empty(result.Unwrap());
        }
    }
}
