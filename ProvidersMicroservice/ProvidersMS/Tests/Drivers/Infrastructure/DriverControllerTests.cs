using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.Core.Application.Storage.Images.Types;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Drivers.Application.Queries.GetById.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Infrastructure.Controllers;
using ProvidersMS.src.Drivers.Infrastructure.Dtos;
using RestSharp;
using System.Text;
using Xunit;
using static ProvidersMS.Core.Infrastructure.GoogleMaps.GoogleApiService;

namespace ProvidersMS.Tests.Drivers.Infrastructure
{
    public class DriverControllerTests
    {
        private readonly Mock<IDriverRepository> _driverRepoMock = new Mock<IDriverRepository>();
        private readonly Mock<ICraneRepository> _craneRepoMock = new Mock<ICraneRepository>();
        private readonly Mock<IImageStorage> _imageStorageMock = new Mock<IImageStorage>();
        private readonly Mock<IdGenerator<string>> _idGeneratorMock = new Mock<IdGenerator<string>>();
        private readonly Mock<IValidator<CreateDriverWithImagesCommand>> _validatorCreateMock = new Mock<IValidator<CreateDriverWithImagesCommand>>();
        private readonly Mock<IValidator<UpdateDriverCommand>> _validatorUpdateMock = new Mock<IValidator<UpdateDriverCommand>>();
        private readonly Mock<IValidator<UpdateDriverLocationCommand>> _validatorUpdateLocationMock = new Mock<IValidator<UpdateDriverLocationCommand>>();
        private readonly Mock<IGoogleApiService> _googleApiServiceMock = new Mock<IGoogleApiService>();
        private readonly Mock<IRestClient> _restClientMock = new Mock<IRestClient>();
        private readonly Mock<ILoggerContract> _loggerMock = new Mock<ILoggerContract>();
        private readonly DriverController _controller;

        public DriverControllerTests()
        {
            _controller = new DriverController(
                _driverRepoMock.Object,
                _craneRepoMock.Object,
                _imageStorageMock.Object,
                _idGeneratorMock.Object,
                _validatorCreateMock.Object,
                _validatorUpdateMock.Object,
                _validatorUpdateLocationMock.Object,
                _googleApiServiceMock.Object,
                _restClientMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateDriver_ShouldReturnCreated()
        {
            var command = new CreateDriverWithImagesCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "12345678",
                true,
                "ac174604-3e44-4f57-bf7d-ef97f44b5a57",
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "license", "license.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "dni", "dni.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "road_medical_certificate", "road_medical_certificate.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "civil_liability", "civil_liability.jpg"),
                "El Paraiso, Caracas, Venezuela"
            );

            var driver = Driver.CreateDriver(
                new DriverId(command.UserId),
                new DriverDNI(command.DNI),
                new DriverIsActiveLicensed(command.IsActiveLicensed),
                new List<string>(),
                new CraneId(command.CraneAssigned),
                new DriverLocation(10.0, 10.0)
            );

            var craneId = new CraneId("ac174604-3e44-4f57-bf7d-ef97f44b5a57");
            var brand = new CraneBrand("Ford");
            var model = new CraneModel("Tritón");
            var plate = new CranePlate("AB123CD");
            var size = new CraneSize("Pesada");
            var year = new CraneYear(2012);
            var existingCrane = Crane.CreateCrane(craneId, brand, model, plate, size, year);

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Driver\",\"email\":\"driver@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Driver\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _craneRepoMock.Setup(r => r.GetById(craneId.GetValue())).ReturnsAsync(Optional<Crane>.Of(existingCrane));
            _craneRepoMock.Setup(x => x.IsActiveCrane(command.CraneAssigned)).ReturnsAsync(true);
            _driverRepoMock.Setup(x => x.IsCraneAssociatedWithAnotherDriver(command.CraneAssigned)).ReturnsAsync(false);
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Success(new Coordinates { Latitude = 10.0, Longitude = 10.0 }));
            _driverRepoMock.Setup(x => x.Save(driver)).ReturnsAsync(Result<Driver>.Success(driver));
            _imageStorageMock.Setup(x => x.Save(It.IsAny<string>())).ReturnsAsync(Result<ImageSaved>.Success(new ImageSaved { Url = "http://example.com/image.jpg" }));
            _driverRepoMock.Setup(x => x.GetById(command.UserId)).ReturnsAsync(Optional<Driver>.Of(driver));
            _driverRepoMock.Setup(x => x.Update(driver)).ReturnsAsync(Result<Driver>.Success(driver));

            var result = await _controller.CreateDriver(command, token);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, actionResult.StatusCode);
            Assert.Equal(new { id = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e" }, actionResult.Value);
        }

        [Fact]
        public async Task CreateCrane_ShouldReturn400_WhenValidationFails()
        {
            var command = new CreateDriverWithImagesCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "",
                true,
                "ac174604-3e44-4f57-bf7d-ef97f44b5a57",
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "license", "license.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "dni", "dni.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "road_medical_certificate", "road_medical_certificate.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "civil_liability", "civil_liability.jpg"),
                "El Paraiso, Caracas, Venezuela"
            );

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Driver\",\"email\":\"driver@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Driver\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("DNI", "DNI is required") });
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.CreateDriver(command, token) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "DNI is required" }, result?.Value);
        }

        [Fact]
        public async Task CreateDriver_ShouldReturn500_WhenDriverAlreadyExists()
        {
            var command = new CreateDriverWithImagesCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "12345678",
                true,
                "ac174604-3e44-4f57-bf7d-ef97f44b5a57",
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "license", "license.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "dni", "dni.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "road_medical_certificate", "road_medical_certificate.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy content")), 0, 12, "civil_liability", "civil_liability.jpg"),
                "El Paraiso, Caracas, Venezuela"
            );

            var driver = Driver.CreateDriver(
                new DriverId(command.UserId),
                new DriverDNI(command.DNI),
                new DriverIsActiveLicensed(command.IsActiveLicensed),
                new List<string>(),
                new CraneId(command.CraneAssigned),
                new DriverLocation(10.0, 10.0)
            );

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Driver\",\"email\":\"driver@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Driver\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _driverRepoMock.Setup(x => x.ExistByDNI(command.DNI)).ReturnsAsync(true);

            var result = await _controller.CreateDriver(command, token) as ObjectResult;

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAllDrivers_ShouldReturn200_WhenDriversAreRetrievedSuccessfully()
        {
            var query = new GetAllDriversQuery(10, 1, "", "");
            var drivers = new List<Driver>
            {
                Driver.CreateDriver(
                    new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                    new DriverDNI("00000000"),
                    new DriverIsActiveLicensed(true),
                    new List<string>(),
                    new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344"),
                    new DriverLocation(10.0, 10.0)
                ),
                Driver.CreateDriver(
                    new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"),
                    new DriverDNI("00000000"),
                    new DriverIsActiveLicensed(true),
                    new List<string>(),
                    new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344"),
                    new DriverLocation(10.0, 10.0)
                )
            };
            var driverResponses = drivers.Select(d => new GetDriverResponse(d.GetId(), d.GetDNI(), d.GetIsActiveLicensed(), d.GetImagesDocuments(), d.GetCraneAssigned(), d.GetIsAvailable(), new CoordinatesDto(d.GetDriverLocationLatitude(), d.GetDriverLocationLongitude()), d.GetIsActive())).ToArray();
            _driverRepoMock.Setup(x => x.GetAll(query)).ReturnsAsync(drivers);
            var result = await _controller.GetAllDrivers(query);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);

            var responseValue = Assert.IsType<GetDriverResponse[]>(actionResult.Value);
            Assert.Equal(driverResponses, responseValue);
        }

        [Fact]
        public async Task GetDriverById_ShouldReturn200_WhenDriverExist()
        {
            var driverId = new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var DNI = new DriverDNI("00000000");
            var activeLicensed = new DriverIsActiveLicensed(true);
            var images = new List<string>();
            var craneAssigned = new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344");
            var location = new DriverLocation(10.0, 10.0);         
            var existingDriver = Driver.CreateDriver(driverId, DNI, activeLicensed, images, craneAssigned, location);

            var query = new GetDriverByIdQuery(driverId.GetValue());

            _driverRepoMock.Setup(r => r.GetById(driverId.GetValue())).ReturnsAsync(Optional<Driver>.Of(existingDriver));

            var result = await _controller.GetDriverById(driverId.GetValue());

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetDriverById_ShouldReturn500_WhenDriverNotFound()
        {
            var query = new GetDriverByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _driverRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _controller.GetDriverById(query.Id);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDriver_ShouldReturn200_WhenUpdateIsSuccessful()
        {
            var driverId = new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var DNI = new DriverDNI("00000000");
            var activeLicensed = new DriverIsActiveLicensed(true);
            var images = new List<string>();
            var craneAssigned = new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344");
            var location = new DriverLocation(10.0, 10.0);
            var existingDriver = Driver.CreateDriver(driverId, DNI, activeLicensed, images, craneAssigned, location);

            var command = new UpdateDriverCommand(true, "c697e2b8-87ef-4820-bcdb-9cdf39b9d354", false, "el paraiso, caracas", true);

            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            _driverRepoMock.Setup(r => r.GetById(driverId.GetValue())).ReturnsAsync(Optional<Driver>.Of(existingDriver));
            _googleApiServiceMock.Setup(x => x.GetCoordinatesFromAddress(command.DriverLocation)).ReturnsAsync(Result<Coordinates>.Success(new Coordinates { Latitude = 10.0, Longitude = 10.0 }));
            _driverRepoMock.Setup(r => r.Update(existingDriver)).ReturnsAsync(Result<Driver>.Success(existingDriver));

            var result = await _controller.UpdateDriver(command, driverId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDriver_ShouldReturn400_WhenValidationFails()
        {
            var command = new UpdateDriverCommand(true, "c697e2b8-87ef-4820-bcdb-9cdf39b9d354", false, "el paraiso, caracas", null);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("IsActive", "IsActive is required") });
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.UpdateDriver(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "IsActive is required" }, result?.Value);
        }

        [Fact]
        public async Task UpdateDriver_ShouldReturn409_WhenDriverNotFound()
        {
            var command = new UpdateDriverCommand(true, "c697e2b8-87ef-4820-bcdb-9cdf39b9d354", false, "el paraiso, caracas", true);
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _driverRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _controller.UpdateDriver(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDriverLocation_ShouldReturn200_WhenUpdateIsSuccessful()
        {
            var driverId = new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var DNI = new DriverDNI("00000000");
            var activeLicensed = new DriverIsActiveLicensed(true);
            var images = new List<string>();
            var craneAssigned = new CraneId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344");
            var location = new DriverLocation(10.0, 10.0);
            var existingDriver = Driver.CreateDriver(driverId, DNI, activeLicensed, images, craneAssigned, location);

            var command = new UpdateDriverLocationCommand(11.0, 11.0);

            _validatorUpdateLocationMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _driverRepoMock.Setup(r => r.GetById(driverId.GetValue())).ReturnsAsync(Optional<Driver>.Of(existingDriver));
            _driverRepoMock.Setup(r => r.UpdateDriverLocation(driverId.GetValue(), command.Latitude, command.Longitude)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateDriverLocation(command, driverId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDriverLocation_ShouldReturn409_WhenDriverNotFound()
        {
            var command = new UpdateDriverLocationCommand(11.0, 11.0);
            _validatorUpdateLocationMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _driverRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Driver>.Empty());

            var result = await _controller.UpdateDriverLocation(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDriverLocation_ShouldReturn400_WhenValidationFails()
        {
            var command = new UpdateDriverLocationCommand(-89, 11.0);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Latitude", "Latitude must be greater than -90.") });
            _validatorUpdateLocationMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.UpdateDriverLocation(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "Latitude must be greater than -90." }, result?.Value);
        }

        [Fact]
        public async Task ValidateUpdateTimeDriver_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            _driverRepoMock.Setup(r => r.ValidateUpdateTimeDriver()).Returns(Task.CompletedTask);

            var result = await _controller.ValidateUpdateTimeDriver();

            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }
    }
}
