using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Application.Queries.GetAll;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetById;
using ProvidersMS.src.Providers.Application.Queries.GetById.Types;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Application.Queries.Types;
using ProvidersMS.src.Providers.Application.Repositories;


namespace ProvidersMS.src.Providers.Infrastructure.Controllers
{
    [Route("provider/provider")]
    [ApiController]
    public class ProviderController(
        IProviderRepository providerRepo,
        ICraneRepository craneRepo,
        IDriverRepository driverRepo,
        IdGenerator<string> idGenerator,
        IValidator<CreateProviderCommand> validatorCreate,
        IValidator<UpdateProviderCommand> validatorUpdate,
        ILoggerContract logger) : ControllerBase
    {
        private readonly IProviderRepository _providerRepo = providerRepo;
        private readonly ICraneRepository _craneRepo = craneRepo;
        private readonly IDriverRepository _driverRepo = driverRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateProviderCommand> _validatorCreate = validatorCreate;
        private readonly IValidator<UpdateProviderCommand> _validatorUpdate = validatorUpdate;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateProvider([FromBody] CreateProviderCommand data)
        {
            try
            {
                var command = new CreateProviderCommand(data.Rif, data.ProviderType, data.FleetOfCranes, data.Drivers);

                var validate = _validatorCreate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateProviderCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new CreateProviderCommandHandler(_providerRepo, _craneRepo, _driverRepo, _idGenerator);
                var result = await handler.Execute(command);

                if (result.IsSuccessful)
                {
                    _logger.Log("Provider created successfully: {ProviderId}", result.Unwrap().Id);
                    return StatusCode(201, new { id = result.Unwrap().Id });
                }
                else
                {
                    _logger.Error("Failed to create provider: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while creating the provider.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProviders([FromQuery] GetAllProvidersQuery data)
        {
            try
            {
                var query = new GetAllProvidersQuery(data.PerPage, data.Page, data.IsActive);
                var handler = new GetAllProvidersQueryHandler(_providerRepo);
                var result = await handler.Execute(query);

                _logger.Log("List of providers: {ProvidersIds}", string.Join(", ", result.Unwrap().Select(c => c.Id)));
                return StatusCode(200, result.Unwrap());
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get list of providers", ex.Message);
                return StatusCode(200, Array.Empty<GetProviderResponse>());
            }
        }

        [HttpGet("availables")]
        public async Task<IActionResult> GetAvailableDrivers([FromQuery] GetAvailableDriversQuery data)
        {
            try
            {
                var query = new GetAvailableDriversQuery(data.PerPage, data.Page, data.IsAvailable);
                var handler = new GetAvailableDriversQueryHandler(_providerRepo);
                var result = await handler.Execute(query);

                _logger.Log("List of available drivers:", string.Join(", ", result.Unwrap().Select(c => c.CraneAssigned)));
                return StatusCode(200, result.Unwrap());
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get available drivers", ex.Message);
                return StatusCode(200, Array.Empty<GetAvailableDriversResponse>());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProviderById(string id)
        {
            try
            {
                var query = new GetProviderByIdQuery(id);
                var handler = new GetProviderByIdQueryHandler(_providerRepo);
                var result = await handler.Execute(query);

                var crane = result.Unwrap();

                _logger.Log("Provider found: {ProviderId}", id);
                return StatusCode(200, crane);
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get provider by id", ex.Message);
                return StatusCode(500, "Provider not found");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProvider([FromBody] UpdateProviderCommand data, string id)
        {
            try
            {
                var command = new UpdateProviderCommand(data.FleetOfCranes, data.Drivers, data.IsActive);

                var validate = _validatorUpdate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateProviderCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateProviderCommandHandler(_providerRepo, _craneRepo, _driverRepo);
                var result = await handler.Execute((id, data));
                if (result.IsSuccessful)
                {
                    var crane = result.Unwrap();
                    _logger.Log("Provider updated: {ProviderId}", id);
                    return Ok(crane);
                }
                else
                {
                    _logger.Error("Failed to update provider: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the provider.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
