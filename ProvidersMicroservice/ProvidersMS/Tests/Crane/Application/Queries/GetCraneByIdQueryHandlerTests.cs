﻿using Moq;
using ProvidersMS.src.Cranes.Application.Queries.GetById;
using ProvidersMS.src.Cranes.Application.Queries.GetById.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Crane.Application.Queries
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
            var crane = src.Cranes.Domain.Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                CraneSizeType.Mediana,
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Core.Utils.Optional.Optional<src.Cranes.Domain.Crane>.Of(crane));

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e", result.Unwrap().Id);
            Assert.Equal("Ford", result.Unwrap().Brand);
            Assert.Equal("Tritón", result.Unwrap().Model);
            Assert.Equal("AC123CD", result.Unwrap().Plate);
            Assert.Equal(CraneSizeType.Mediana.GetValue(), result.Unwrap().CraneType);
            Assert.Equal(2012, result.Unwrap().Year);
        }

        [Fact]
        public async Task GetCraneByIdWhenCraneNotFound()
        {
            var query = new GetCraneByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f9f");
            var handler = new GetCraneByIdQueryHandler(_craneRepositoryMock.Object);

            _craneRepositoryMock.Setup(x => x.GetById(query.Id)).ReturnsAsync(Core.Utils.Optional.Optional<src.Cranes.Domain.Crane>.Empty());

            var result = await handler.Execute(query);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Crane not found", result.ErrorMessage);
        }
    }
}
