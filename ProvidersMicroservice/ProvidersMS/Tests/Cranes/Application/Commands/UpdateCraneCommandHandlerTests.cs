using Moq;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Exceptions;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using Xunit;

namespace ProvidersMS.Tests.Cranes.Application.Commands
{
    public class UpdateCraneCommandHandlerTests
    {
        private readonly Mock<ICraneRepository> _craneRepositoryMock;

        public UpdateCraneCommandHandlerTests()
        {
            _craneRepositoryMock = new Mock<ICraneRepository>();
        }

        [Fact]
        public async Task ShouldUpdateCraneSucces()
        {
            var craneId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateCraneCommand(true);
            var handler = new UpdateCraneCommandHandler(_craneRepositoryMock.Object);

            var crane = Crane.CreateCrane(
                new CraneId(craneId),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(craneId)).ReturnsAsync(Optional<Crane>.Of(crane));
            _craneRepositoryMock.Setup(x => x.Update(crane)).ReturnsAsync(Result<Crane>.Success(crane));

            var result = await handler.Execute((craneId, command));
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(craneId, result.Unwrap().Id);
            Assert.Equal("Ford", result.Unwrap().Brand);
            Assert.Equal("Tritón", result.Unwrap().Model);
            Assert.Equal("AC123CD", result.Unwrap().Plate);
            Assert.Equal("Mediana", result.Unwrap().CraneSize);
            Assert.Equal(2012, result.Unwrap().Year);
            Assert.True(result.Unwrap().IsActive);
        }

        [Fact]
        public async Task ShouldFailToUpdateCraneWhenCraneNotFound()
        {
            var craneId = "1";
            var command = new UpdateCraneCommand(true);
            var handler = new UpdateCraneCommandHandler(_craneRepositoryMock.Object);

            _craneRepositoryMock.Setup(x => x.GetById(craneId)).ReturnsAsync(Optional<Crane>.Empty());

            var result = await handler.Execute((craneId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Crane not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ShouldFailToUpdateCraneWhenUpdateFails()
        {
            var craneId = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d";
            var command = new UpdateCraneCommand(true);
            var handler = new UpdateCraneCommandHandler(_craneRepositoryMock.Object);

            var crane = Crane.CreateCrane(
                new CraneId(craneId),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            _craneRepositoryMock.Setup(x => x.GetById(craneId)).ReturnsAsync(Optional<Crane>.Of(crane));
            _craneRepositoryMock.Setup(x => x.Update(crane)).ReturnsAsync(Result<Crane>.Failure(new CraneUpdateFailedException()));

            var result = await handler.Execute((craneId, command));

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal("The crane could not be updated correctly", result.ErrorMessage);
        }
    }
}
