using FluentValidation;
using FluentValidation.Results;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using Xunit;
using ProvidersMS.src.Cranes.Infrastructure.Controllers;

namespace ProvidersMS.Tests.Crane.Infrastructure
{
    public class CraneControllerTests
    {
        private readonly Mock<ICraneRepository> _craneRepoMock = new Mock<ICraneRepository>();
        private readonly Mock<IdGenerator<string>> _idGeneratorMock = new Mock<IdGenerator<string>>();
        private readonly Mock<IValidator<CreateCraneCommand>> _validatorCreateMock = new Mock<IValidator<CreateCraneCommand>>();
        private readonly Mock<IValidator<UpdateCraneCommand>> _validatorUpdateMock = new Mock<IValidator<UpdateCraneCommand>>();
        private readonly Mock<ILoggerContract> _loggerMock = new Mock<ILoggerContract>();
        private readonly CraneController _controllerMock;

        public CraneControllerTests()
        {
            _controllerMock = new CraneController(_craneRepoMock.Object, _idGeneratorMock.Object, _validatorCreateMock.Object, _validatorUpdateMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturnCreated()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Heavy", 2012);
            var craneId = new CreateCraneResponse("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            _craneRepoMock.Setup(x => x.ExistByPlate(command.Plate)).ReturnsAsync(false);

            _idGeneratorMock.Setup(x => x.Generate()).Returns(craneId.Id);

            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            var result = await _controllerMock.CreateCrane(command) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            _loggerMock.Verify(x => x.Log(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturnBadRequest_WhenValidationFails()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Heavy", 2012);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Brand", "Brand is required") });
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controllerMock.CreateCrane(command) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(new List<string> { "Brand is required" }, result.Value);
        }

        //[Fact]
        //public async Task CreateCrane_ShouldReturnConflict_WhenCreationFails()
        //{
        //    var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Heavy", 2012);
        //    _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
        //    _craneRepoMock.Setup(x => x.Save(It.IsAny<src.Cranes.Domain.Crane>())).ReturnsAsync(Result<src.Cranes.Domain.Crane>.Failure(new Exception("Creation failed")));

        //    var result = await _controllerMock.CreateCrane(command) as ObjectResult;

        //    Assert.NotNull(result);
        //    Assert.Equal(409, result.StatusCode);
        //    Assert.Equal("Creation failed", result.Value);
        //}

        //[Fact]
        //public async Task CreateCrane_ShouldReturnInternalServerError_WhenExceptionOccurs()
        //{
        //    var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Heavy", 2012);
        //    _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
        //    _craneRepoMock.Setup(x => x.Save(It.IsAny<src.Cranes.Domain.Crane>())).ThrowsAsync(new Exception("Unexpected error"));

        //    var result = await _controllerMock.CreateCrane(command) as ObjectResult;

        //    Assert.NotNull(result);
        //    Assert.Equal(500, result!.StatusCode);
        //    Assert.Equal("Unexpected error", result.Value);
        //}
    }
    
}
