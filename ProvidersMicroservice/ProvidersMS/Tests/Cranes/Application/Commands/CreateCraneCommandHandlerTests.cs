using Moq;
using Xunit;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Domain;

namespace ProvidersMS.Tests.Cranes.Application.Commands
{
    public class CreateCraneCommandHandlerTests
    {
        private readonly Mock<ICraneRepository> _craneRepositoryMock;
        private readonly Mock<IdGenerator<string>> _idGeneratorMock;
        private readonly CreateCraneCommandHandler _handler;

        public CreateCraneCommandHandlerTests()
        {
            _craneRepositoryMock = new Mock<ICraneRepository>();
            _idGeneratorMock = new Mock<IdGenerator<string>>();
            _handler = new CreateCraneCommandHandler(_craneRepositoryMock.Object, _idGeneratorMock.Object);
        }

        [Fact]
        public async Task ShouldCreateCraneSuccess()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AC123CD", "Mediana", 2012);

            _craneRepositoryMock.Setup(x => x.ExistByPlate(command.Plate)).ReturnsAsync(false);
            _idGeneratorMock.Setup(x => x.Generate()).Returns("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d");
            var result = await _handler.Execute(command);

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0d"),
                new CraneBrand(command.Brand),
                new CraneModel(command.Model),
                new CranePlate(command.Plate),
                new CraneSize("Mediana"),
                new CraneYear(command.Year)
            );

            _craneRepositoryMock.Setup(x =>
                x.Save(crane))
                .ReturnsAsync(Result<src.Cranes.Domain.Crane>.Success(crane));

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task ShouldFailToCreateCraneWhenCraneAlreadyExists()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AC123CD", "Mediana", 2012);

            _craneRepositoryMock.Setup(x => x.ExistByPlate(command.Plate)).ReturnsAsync(true);
            var result = await _handler.Execute(command);

            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal($"Crane with plate AC123CD already exist", result.ErrorMessage);
        }
    }
}