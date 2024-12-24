using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
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

namespace ProvidersMS.src.Drivers.Infrastructure.Controllers
{
    [Route("provider/driver")]
    [ApiController]
    public class DriverController(
        IDriverRepository driverRepo,
        ICraneRepository craneRepo,
        IImageDocumentRepository imageRepo,
        ImageStorage imageStorage,
        IdGenerator<string> idGenerator,
        IValidator<CreateDriverWithImagesCommand> validatorCreate,
        IValidator<UpdateDriverCommand> validatorUpdate,
        ILoggerContract logger) : ControllerBase
    {
        private readonly IDriverRepository _driverRepo = driverRepo;
        private readonly ICraneRepository _craneRepo = craneRepo;
        private readonly IImageDocumentRepository _imageRepo = imageRepo;
        private readonly ImageStorage _imageStorage = imageStorage;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateDriverWithImagesCommand> _validatorCreate = validatorCreate;
        private readonly IValidator<UpdateDriverCommand> _validatorUpdate = validatorUpdate;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromForm] CreateDriverWithImagesCommand data)
        {
            try
            {
                var validate = _validatorCreate.Validate(data);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateDriverWithImagesCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var createDriverCommand = new CreateDriverCommand(data.DNI, data.IsActiveLicensed, data.CraneAssigned);
                var createDriverService = new CreateDriverCommandHandler(_driverRepo, _craneRepo, _idGenerator);
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
                var uploadImagesService = new UploadImagesDocumentsCommandHandler(_idGenerator, _imageRepo, _imageStorage);
                var uploadImageResult = await uploadImagesService.Execute(uploadImagesCommand);
                if (uploadImageResult.IsFailure)
                {
                    return StatusCode(400, uploadImageResult.ErrorMessage);
                }

                var driverId = createDriverResult.Unwrap().Id;
                var updateImagesCommand = new UpdateDriverImagesCommand(
                    driverId,
                    data.LicenseImage,
                    data.DNIImage,
                    data.RoadMedicalCertificateImage,
                    data.CivilLiabilityImage
                );
                var updateImagesService = new UpdateImagesDocumentsCommandHandler(_driverRepo, uploadImagesService);
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
        public async Task<IActionResult> GetAllDrivers([FromQuery] GetAllDriversQuery data)
        {
            try
            {
                var query = new GetAllDriversQuery(data.PerPage, data.Page, data.IsActiveLicensed);
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDriver([FromBody] UpdateDriverCommand data, string id)
        {
            try
            {
                var command = new UpdateDriverCommand(data.IsActiveLicensed, data.CraneAssigned, data.IsAvailable);

                var validate = _validatorUpdate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateDriverCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateDriverCommandHandler(_driverRepo);
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
    }
}
