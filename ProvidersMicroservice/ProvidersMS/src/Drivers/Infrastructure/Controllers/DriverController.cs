﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.src.Drivers.Application.Commands.CreateDriver;
using ProvidersMS.src.Drivers.Application.Commands.CreateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;
using ProvidersMS.src.Drivers.Infrastructure.Dtos;
using ProvidersMS.src.Drivers.Application.Queries.GetById.Types;
using ProvidersMS.src.Drivers.Application.Queries.GetById;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Queries.GetAll;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.src.Drivers.Application.Commands.DisconnectDriver;
using RestSharp;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation;
using Microsoft.AspNetCore.Authorization;
using ProvidersMS.src.Drivers.Application.Exceptions;

namespace ProvidersMS.src.Drivers.Infrastructure.Controllers
{
    [Route("provider/driver")]
    [ApiController]
    public class DriverController(
        IDriverRepository driverRepo,
        ICraneRepository craneRepo,
        IImageStorage imageStorage,
        IdGenerator<string> idGenerator,
        IValidator<CreateDriverWithImagesCommand> validatorCreate,
        IValidator<UpdateDriverCommand> validatorUpdate,
        IValidator<UpdateDriverLocationCommand> validatorUpdateLocation,
        IGoogleApiService googleApiService,
        IRestClient restClient,
        ILoggerContract logger) : ControllerBase
    {
        private readonly IDriverRepository _driverRepo = driverRepo;
        private readonly ICraneRepository _craneRepo = craneRepo;
        private readonly IImageStorage _imageStorage = imageStorage;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateDriverWithImagesCommand> _validatorCreate = validatorCreate;
        private readonly IValidator<UpdateDriverCommand> _validatorUpdate = validatorUpdate;
        private readonly IValidator<UpdateDriverLocationCommand> _validatorUpdateLocation = validatorUpdateLocation;
        private readonly IGoogleApiService _googleApiService = googleApiService;
        private readonly IRestClient _restClient = restClient;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        [Authorize(Roles = "Admin, Provider")]
        public async Task<IActionResult> CreateDriver([FromForm] CreateDriverWithImagesCommand data, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(400, "Invalid or missing Authorization header");
                }

                var userExist = new RestRequest($"https://localhost:4051/user/{data.UserId}", Method.Get);
                userExist.AddHeader("Authorization", token);
                var response = await _restClient.ExecuteAsync(userExist);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Failed to get user id. Content: {response.Content}");
                }

                var validate = _validatorCreate.Validate(data);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateDriverWithImagesCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var createDriverCommand = new CreateDriverCommand(data.UserId, data.DNI, data.IsActiveLicensed, data.CraneAssigned, data.DriverLocation);
                var createDriverService = new CreateDriverCommandHandler(_driverRepo, _craneRepo, _googleApiService);
                var createDriverResult = await createDriverService.Execute(createDriverCommand);
                if (createDriverResult.IsFailure)
                {
                    if (createDriverResult.ErrorMessage.Contains(nameof(CraneNotFoundException)))
                    {
                        return StatusCode(400, createDriverResult.ErrorMessage);
                    }
                    return StatusCode(500, createDriverResult.ErrorMessage);
                }

                var uploadImagesCommand = new UploadImageCommand
                (
                    data.LicenseImage,
                    data.DNIImage,
                    data.RoadMedicalCertificateImage,
                    data.CivilLiabilityImage
                );
                var uploadImagesService = new UploadImagesDocumentsCommandHandler(_imageStorage);
                var uploadImageResult = await uploadImagesService.Execute(uploadImagesCommand);
                if (uploadImageResult.IsFailure)
                {
                    return StatusCode(400, uploadImageResult.ErrorMessage);
                }

                var driverId = createDriverResult.Unwrap().Id;
                var imageUrls = uploadImageResult.Unwrap().Urls;

                var updateImagesCommand = new UpdateDriverImagesCommand(
                    driverId,
                    imageUrls
                );
                var updateImagesService = new UpdateImagesDocumentsCommandHandler(_driverRepo);
                var updateImagesResult = await updateImagesService.Execute(updateImagesCommand);
                if (updateImagesResult.IsFailure)
                {
                    return StatusCode(400, updateImagesResult.ErrorMessage);
                }

                _logger.Log("Driver created successfully: {DriverId}", driverId);
                return StatusCode(201, new { id = driverId });
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while creating the driver.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Operator")]
        public async Task<IActionResult> GetAllDrivers([FromQuery] GetAllDriversQuery data)
        {
            try
            {
                var query = new GetAllDriversQuery(data.PerPage, data.Page, data.IsActiveLicensed, data.IsAvailable);
                var handler = new GetAllDriversQueryHandler(_driverRepo);
                var result = await handler.Execute(query);

                _logger.Log("List of drivers: {DriverIds}", string.Join(", ", result.Unwrap().Select(c => c.Id)));
                return StatusCode(200, result.Unwrap());
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get list of drivers", ex.Message);
                return StatusCode(200, Array.Empty<GetDriverResponse>());
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, Provider, Driver")]
        public async Task<IActionResult> GetDriverById(string id)
        {
            try
            {
                var query = new GetDriverByIdQuery(id);
                var handler = new GetDriverByIdQueryHandler(_driverRepo);
                var result = await handler.Execute(query);

                var driver = result.Unwrap();

                if (driver == null || driver.Id != id)
                {
                    _logger.Error("Driver not found: {DriverId}", id);
                    return NotFound();
                }

                _logger.Log("Driver found: {DriverId}", id);
                return StatusCode(200, driver);
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get driver by id", ex.Message);
                return StatusCode(500, "Driver not found");
            }
        }

        [Authorize(Roles = "Admin, Operator, Provider, Driver")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDriver([FromBody] UpdateDriverCommand data, string id)
        {
            try
            {
                var command = new UpdateDriverCommand(data.IsActiveLicensed, data.CraneAssigned, data.IsAvailable, data.DriverLocation, data.IsActive);

                var validate = _validatorUpdate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateDriverCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateDriverCommandHandler(_driverRepo, _googleApiService);
                var result = await handler.Execute((id, data));
                if (result.IsSuccessful)
                {
                    var user = result.Unwrap();
                    _logger.Log("Driver updated: {DriverId}", id);
                    return Ok(user);
                }
                else
                {
                    _logger.Error("Failed to update driver: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the driver.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/updateLocation")]
        [Authorize(Roles = "Admin, Operator, Provider, Driver")]
        public async Task<IActionResult> UpdateDriverLocation([FromBody] UpdateDriverLocationCommand data, string id)
        {
            try
            {
                var command = new UpdateDriverLocationCommand(data.Latitude, data.Longitude);

                var validate = _validatorUpdateLocation.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateDriverLocationCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateDriverLocationCommandHandler(_driverRepo);
                var result = await handler.Execute((id, data));
                if (result.IsSuccessful)
                {
                    var user = result.Unwrap();
                    _logger.Log("Driver updated: {DriverId}", id);
                    return Ok(user);
                }
                else
                {
                    _logger.Error("Failed to update driver location: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the driver.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("disconnect")]
        public async Task<IActionResult> ValidateUpdateTimeDriver()
        {
            try
            {
                var handler = new DisconnectDriverCommandHandler(_driverRepo);
                var result = await handler.Execute();
                if (result)
                {
                    _logger.Log("Drivers updated");
                    return Ok();
                }
                else
                {
                    _logger.Error("Failed to update drivers");
                    return StatusCode(409, "Failed to update drivers");
                }

            } catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the drivers.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
