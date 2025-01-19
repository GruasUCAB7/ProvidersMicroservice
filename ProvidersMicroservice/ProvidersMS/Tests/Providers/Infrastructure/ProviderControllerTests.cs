using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Application.Models;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetById.Types;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Application.Queries.Types;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Infrastructure.Controllers;
using RestSharp;
using Xunit;

namespace ProvidersMS.Tests.Providers.Infrastructure
{
    public class ProviderControllerTests
    {
        private readonly Mock<IProviderRepository> _providerRepoMock = new Mock<IProviderRepository>();
        private readonly Mock<ICraneRepository> _craneRepoMock = new Mock<ICraneRepository>();
        private readonly Mock<IDriverRepository> _driverRepoMock = new Mock<IDriverRepository>();
        private readonly Mock<IValidator<CreateProviderCommand>> _validatorCreateMock = new Mock<IValidator<CreateProviderCommand>>();
        private readonly Mock<IValidator<UpdateProviderCommand>> _validatorUpdateMock = new Mock<IValidator<UpdateProviderCommand>>();
        private readonly Mock<IRestClient> _restClientMock = new Mock<IRestClient>();
        private readonly Mock<ILoggerContract> _loggerMock = new Mock<ILoggerContract>();
        private readonly ProviderController _controller;

        public ProviderControllerTests()
        {
            _controller = new ProviderController(
                _providerRepoMock.Object,
                _craneRepoMock.Object,
                _driverRepoMock.Object,
                _validatorCreateMock.Object,
                _validatorUpdateMock.Object,
                _restClientMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateProvider_ShouldReturnCreated()
        {
            var command = new CreateProviderCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "V000000000",
                "Interno",
                new List<string>(),
                new List<string>()
            );

            var provider = Provider.CreateProvider(
                new ProviderId(command.UserId),
                new ProviderRif(command.Rif),
                new ProviderType(command.ProviderType),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            var driver = src.Drivers.Domain.Driver.CreateDriver(
                new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"),
                new DriverDNI("00000000"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new DriverLocation(10.0, 10.0)
            );

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Provider\",\"email\":\"provider@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Provider\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _providerRepoMock.Setup(x => x.ExistByRif(provider.GetRif())).ReturnsAsync(false);
            _craneRepoMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepoMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepoMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<src.Drivers.Domain.Driver>.Of(driver));
            _providerRepoMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ReturnsAsync(false);
            _providerRepoMock.Setup(x => x.Save(provider)).ReturnsAsync(Result<Provider>.Success(provider));

            var result = await _controller.CreateProvider(command, token);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, actionResult.StatusCode);
            Assert.Equal(new { id = "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e" }, actionResult.Value);
        }

        [Fact]
        public async Task CreateProvider_ShouldReturn400_WhenValidationFails()
        {
            var command = new CreateProviderCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "",
                "Interno",
                new List<string>(),
                new List<string>()
            );

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Provider\",\"email\":\"provider@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Provider\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Rif", "Rif is required") });
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.CreateProvider(command, token) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "Rif is required" }, result?.Value);
        }

        [Fact]
        public async Task CreateProvider_ShouldReturn409_WhenProviderAlreadyExists()
        {
            var command = new CreateProviderCommand(
                "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e",
                "V000000000",
                "Interno",
                new List<string>(),
                new List<string>()
            );

            var provider = Provider.CreateProvider(
                new ProviderId(command.UserId),
                new ProviderRif(command.Rif),
                new ProviderType(command.ProviderType),
                new List<CraneId>(),
                new List<DriverId>()
            );

            var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeIeyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwM2NmNTVhYy1jYWZkLTRiZTUtODg1Yy1mNGZjZWMxODE0YzciLCJuYW1lIjoiSm95Y2UiLCJlbWFpbCI6Impld2FnbmVyLjIxQGVzdC51Y2FiLmVkdS52ZSIsInBob25lX251bWJlciI6Iis1OCA0MjQtMjcyMDUwMyIsImlzVGVtcG9yYXJ5UGFzc3dvcmQiOiJmYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiNTQ3MjNkMGMtYWNiNS00MTkyLThlODUtZmI1Y2IzNDg5ZWYzIiwiZXhwIjoxNzM2NzA4NjU0LCJpc3MiOiJHcu-_vWFzVUNBQiIsImF1ZCI6Ikdy77-9YXNVQ0FCVXNlcnMifQ.WElef3AbayZCx3nPMn0nxZES-TEGp3eVz0dGHTMpyeI";
            var restResponse = new RestResponse
            {
                Content = "{\"id\":\"e76d03f6-135f-4a64-a8d7-1a8fc88bafe8\",\"name\":\"Provider\",\"email\":\"provider@gmail.com\",\"phone\":\"+58 424-2720504\",\"userType\":\"Provider\",\"isActive\":true,\"department\":\"Conduccion\",\"isTemporaryPassword\":false,\"passwordExpirationDate\":\"2125-11-01T23:25:59\"}",
                StatusCode = System.Net.HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                IsSuccessStatusCode = true,
                ResponseUri = new Uri("https://localhost:4051/user/e76d03f6-135f-4a64-a8d7-1a8fc88bafe8"),
                StatusDescription = "OK"
            };

            _restClientMock.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);
            _validatorCreateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _providerRepoMock.Setup(x => x.ExistByRif(provider.GetRif())).ReturnsAsync(true);

            var result = await _controller.CreateProvider(command, token) as ObjectResult;

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAllProviders_ShouldReturn200_WhenProvidersAreRetrievedSuccessfully()
        {
            var query = new GetAllProvidersQuery(10, 1, "active");
            var providers = new List<Provider>
            {
                Provider.CreateProvider(new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e"), new ProviderRif("V000000000"), new ProviderType("Interno"), new List<CraneId>(), new List<DriverId>()),
                Provider.CreateProvider(new ProviderId("c697e2b8-87ef-4820-bcdb-9cdf39b9d344"), new ProviderRif("V000000001"), new ProviderType("Externo"), new List<CraneId>(), new List<DriverId>())
            };

            var providerResponses = providers.Select(p => new GetProviderResponse(p.GetId(), p.GetRif(), p.GetProviderType(), p.GetFleetOfCranes(), p.GetDrivers(), p.GetIsActive())).ToArray();

            _providerRepoMock.Setup(x => x.GetAll(query)).ReturnsAsync(providers);

            var result = await _controller.GetAllProviders(query);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetAvailableDrivers_ShouldReturn200_WhenDriversAreRetrievedSuccessfully()
        {
            var query = new GetAvailableDriversQuery(10, 1, "disponible");
            var availableDrivers = new List<Driver>
            {
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f6r"),
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f3w", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f7k"),
                new Driver("53c0d8fa-dbca-4d98-9fdf-1d1413e90f6j", "53c0d8fa-dbca-4d98-9fdf-1d1413e90f9p")
            };

            var driverResponses = availableDrivers.Select(d => new GetAvailableDriversResponse(d.Id, d.CraneAssigned)).ToArray();

            _providerRepoMock.Setup(x => x.GetAvailableDrivers(query)).ReturnsAsync(availableDrivers);

            var result = await _controller.GetAvailableDrivers(query);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetProviderById_ShouldReturn200_WhenProviderExist()
        {
            var providerId = new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var rif = new ProviderRif("V000000000");
            var type = new ProviderType("Interno");
            var fleetOfCranes = new List<CraneId>();
            var drivers = new List<DriverId>();
            var existingProvider = Provider.CreateProvider(providerId, rif, type, fleetOfCranes, drivers);

            var query = new GetProviderByIdQuery(providerId.GetValue());

            _providerRepoMock.Setup(r => r.GetById(providerId.GetValue())).ReturnsAsync(Optional<Provider>.Of(existingProvider));

            var result = await _controller.GetProviderById(providerId.GetValue());

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetProviderById_ShouldReturn500_WhenProviderNotFound()
        {
            var query = new GetProviderByIdQuery("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            _providerRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Provider>.Empty());

            var result = await _controller.GetProviderById(query.Id);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProvider_ShouldReturn200_WhenUpdateIsSuccessful()
        {
            var providerId = new ProviderId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");
            var rif = new ProviderRif("V000000000");
            var type = new ProviderType("Interno");
            var fleetOfCranes = new List<CraneId>();
            var drivers = new List<DriverId>();
            var existingProvider = Provider.CreateProvider(providerId, rif, type, fleetOfCranes, drivers);

            var crane = Crane.CreateCrane(
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new CraneBrand("Ford"),
                new CraneModel("Tritón"),
                new CranePlate("AC123CD"),
                new CraneSize("Mediana"),
                new CraneYear(2012)
            );

            var driver = src.Drivers.Domain.Driver.CreateDriver(
                new DriverId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r"),
                new DriverDNI("00000000"),
                new DriverIsActiveLicensed(true),
                new List<string>(),
                new CraneId("53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r"),
                new DriverLocation(10.0, 10.0)
            );

            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, false);

            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());

            _providerRepoMock.Setup(r => r.GetById(providerId.GetValue())).ReturnsAsync(Optional<Provider>.Of(existingProvider));
            _craneRepoMock.Setup(x => x.GetById(crane.GetId())).ReturnsAsync(Optional<Crane>.Of(crane));
            _providerRepoMock.Setup(x => x.IsCraneAssociatedWithAnotherProvider(crane.GetId())).ReturnsAsync(false);
            _driverRepoMock.Setup(x => x.GetById(driver.GetId())).ReturnsAsync(Optional<src.Drivers.Domain.Driver>.Of(driver));
            _providerRepoMock.Setup(x => x.IsDriverAssociatedWithAnotherProvider(driver.GetId())).ReturnsAsync(false);
            _providerRepoMock.Setup(r => r.Update(existingProvider)).ReturnsAsync(Result<Provider>.Success(existingProvider));

            var result = await _controller.UpdateProvider(command, providerId.GetValue());

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProvider_ShouldReturn400_WhenValidationFails()
        {
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, null);
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("IsActive", "IsActive is required") });
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(validationResult);

            var result = await _controller.UpdateProvider(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(new List<string> { "IsActive is required" }, result?.Value);
        }

        [Fact]
        public async Task UpdateProvider_ShouldReturn409_WhenProviderNotFound()
        {
            var command = new UpdateProviderCommand(new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f8r" }, new List<string>() { "53c0d8fa-dbca-4d98-9fdf-1d1413e90f2r" }, true);
            _validatorUpdateMock.Setup(x => x.Validate(command)).Returns(new ValidationResult());
            _providerRepoMock.Setup(r => r.GetById("53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e")).ReturnsAsync(Optional<Provider>.Empty());

            var result = await _controller.UpdateProvider(command, "53c0d8fa-dbca-4d98-9fdf-1d1413e90f0e");

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, actionResult.StatusCode);
        }
    }
}
