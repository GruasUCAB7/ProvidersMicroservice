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
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Application.Types;
using ProvidersMS.src.Cranes.Application.Queries.GetById.Types;
using ProvidersMS.src.Cranes.Domain;

namespace ProvidersMS.Tests.Cranes.Infrastructure
{
    public class CraneControllerTests
    {
        private readonly Mock<ICraneRepository> _craneRepoMock = new Mock<ICraneRepository>();
        private readonly Mock<IdGenerator<string>> _idGeneratorMock = new Mock<IdGenerator<string>>();
        private readonly Mock<IValidator<CreateCraneCommand>> _validatorCreateMock = new Mock<IValidator<CreateCraneCommand>>();
        private readonly Mock<IValidator<UpdateCraneCommand>> _validatorUpdateMock = new Mock<IValidator<UpdateCraneCommand>>();
        private readonly Mock<ILoggerContract> _loggerMock = new Mock<ILoggerContract>();
        private readonly CraneController _controller;

        public CraneControllerTests()
        {
            _controller = new CraneController(_craneRepoMock.Object, _idGeneratorMock.Object, _validatorCreateMock.Object, _validatorUpdateMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturnCreated()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Pesada", 2012);
            var craneId = new CreateCraneResponse("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _idGeneratorMock.Setup(x => x.Generate()).Returns(craneId.Id);

            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            var result = await _controller.CreateCrane(command);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, actionResult.StatusCode);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturn400_WhenValidationFails()
        {
            var command = new CreateCraneCommand("", "Tritón", "AB123CD", "Pesada", 2012);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Brand", "Brand is required") });
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.CreateCrane(command) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "Brand is required" }, result?.Value);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturn409_WhenCraneAlreadyExists()
        {
            var command = new CreateCraneCommand("Ford", "Tritón", "AB123CD", "Pesada", 2012);
            var craneId = new CreateCraneResponse("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _craneRepoMock.Setup(x => x.ExistByPlate(command.Plate)).ReturnsAsync(true);

            var result = await _controller.CreateCrane(command);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAllCranes_ShouldReturn200_WhenCranesAreRetrievedSuccessfully()
        {
            var query = new GetAllCranesQuery(10, 1, "active");
            var cranes = new List<Crane>
            {
                Crane.CreateCrane(new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"), new CraneBrand("Ford"), new CraneModel("Tritón"), new CranePlate("AB123CD"), new CraneSize("Pesada"), new CraneYear(2012)),
                Crane.CreateCrane(new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344"), new CraneBrand("Ford"), new CraneModel("Tritón"), new CranePlate("WT753NJ"), new CraneSize("Ligera"), new CraneYear(2004))
            };

            var craneResponses = cranes.Select(c => new GetCraneResponse(c.GetId(), c.GetBrand(), c.GetModel(), c.GetPlate(), c.GetCraneSize(), c.GetYear(), c.GetIsActive())).ToArray();

            _craneRepoMock.Setup(x => x.GetAll(query)).ReturnsAsync(cranes);

            var result = await _controller.GetAllCranes(query);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);

            var responseValue = Assert.IsType<GetCraneResponse[]>(actionResult.Value);
            Assert.Equal(craneResponses, responseValue);
        }

        [Fact]
        public async Task GetCraneById_ShouldReturn200_WhenCraneExist()
        {
            var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var brand = new CraneBrand("Ford");
            var model = new CraneModel("Tritón");
            var plate = new CranePlate("AB123CD");
            var size = new CraneSize("Pesada");
            var year = new CraneYear(2012);
            var existingCrane = Crane.CreateCrane(craneId, brand, model, plate, size, year);

            var query = new GetCraneByIdQuery(craneId.GetValue());

            _craneRepoMock.Setup(r => r.GetById(craneId.GetValue())).ReturnsAsync(Optional<Crane>.Of(existingCrane));

            var result = await _controller.GetCraneById(craneId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetCraneById_ShouldReturn409_WhenCraneNotFound()
        {
            var query = new GetCraneByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _craneRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Crane>.Empty());

            var result = await _controller.GetCraneById(query.Id);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCrane_ShouldReturn200_WhenUpdateIsSuccessful()
        {
            var craneId = new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var brand = new CraneBrand("Ford");
            var model = new CraneModel("Tritón");
            var plate = new CranePlate("AB123CD");
            var size = new CraneSize("Pesada");
            var year = new CraneYear(2012);
            var existingCrane = Crane.CreateCrane(craneId, brand, model, plate, size, year);

            var command = new UpdateCraneCommand(true);

            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            _craneRepoMock.Setup(r => r.GetById(craneId.GetValue())).ReturnsAsync(Optional<Crane>.Of(existingCrane));
            _craneRepoMock.Setup(r => r.Update(existingCrane)).ReturnsAsync(Result<Crane>.Success(existingCrane));

            var result = await _controller.UpdateCrane(command, craneId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCrane_ShouldReturn400_WhenValidationFails()
        {
            var command = new UpdateCraneCommand(true);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("IsActive", "IsActive is required") });
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.UpdateCrane(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "IsActive is required" }, result?.Value);
        }

        [Fact]
        public async Task UpdateCrane_ShouldReturn409_WhenCraneNotFound()
        {
            var command = new UpdateCraneCommand(true);
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _craneRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Crane>.Empty());

            var result = await _controller.UpdateCrane(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }
    }
}
