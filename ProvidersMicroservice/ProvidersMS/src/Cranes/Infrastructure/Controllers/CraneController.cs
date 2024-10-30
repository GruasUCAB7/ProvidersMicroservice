using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Queries.GetAll;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Application.Queries.Types;
using ProvidersMS.src.Cranes.Application.Repositories;

namespace ProvidersMS.src.Cranes.Infrastructure.Controllers
{
    [Route("api/crane")]
    [ApiController]
    public class CraneController(
        ICraneRepository craneRepo,
        IdGenerator<string> idGenerator,
        IValidator<CreateCraneCommand> validatorCreate,
        ILoggerContract logger) : ControllerBase
    {
        private readonly ICraneRepository _craneRepo = craneRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateCraneCommand> _validator = validatorCreate;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateCrane([FromBody] CreateCraneCommand data)
        {
            try
            {
                var command = new CreateCraneCommand(data.Brand, data.Model, data.Plate, data.CraneType, data.Year);

                var validate = _validator.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for CreateCraneCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new CreateCraneCommandHandler(_craneRepo, _idGenerator);
                var result = await handler.Execute(command);

                if (result.IsSuccessful)
                {
                    _logger.Log("Crane created successfully: {CraneId}", result.Unwrap().Id);
                    return StatusCode(201, new { id = result.Unwrap().Id });
                }
                else
                {
                    _logger.Error("Failed to create crane: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while creating the crane.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCranes([FromQuery] GetAllCranesQuery data)
        {
            try
            {
                var query = new GetAllCranesQuery(data.Page, data.PerPage, data.IsActive);
                var handler = new GetAllCranesQueryHandler(_craneRepo);
                var result = await handler.Execute(query);

                _logger.Log("List of cranes: {CraneIds}", string.Join(", ", result.Unwrap().Select(c => c.Id)));
                return StatusCode(200, result.Unwrap());
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get list of cranes", ex.Message);
                return StatusCode(200, Array.Empty<GetCraneResponse>());
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCraneById(string id)
        //{
        //    try
        //    {
        //        var query = new GetCraneByIdQuery(id);
        //        var handler = new GetCraneByIdQueryHandler(_craneRepo);
        //        var result = await handler.Execute(query);

        //        var crane = result.Unwrap();
        //        if (crane == null || crane.Id != id)
        //        {
        //            _logger.Error("Crane not found: {CraneId}", id);
        //            return NotFound();
        //        }

        //        _logger.Log("Crane found: {CraneId}", id);
        //        return StatusCode(200, crane);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Exception("Failed to get crane by id", ex.Message);
        //        return StatusCode(500, "Crane not found");
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateCrane([FromBody] UpdateCraneCommand data, string id)
        //{
        //    try
        //    {
        //        var command = new UpdateCraneCommand(id, data.Brand, data.Model, data.Plate, data.CraneType, data.Year);
        //        var handler = new UpdateCraneCommandHandler(_craneRepo);
        //        var executionResult = await handler.Execute(command);
        //        var crane = executionResult.Unwrap();
        //        _logger.Log("Crane updated: {CraneId}", id);
        //        return Ok(crane);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Exception("An error occurred while updating the crane.", ex.Message);
        //        var response = new ApiResponse<UpdateCraneResponse>(null, 500, new List<string> { ex.Message });
        //        return StatusCode(500, response);
        //    }
        //}
    }
}
