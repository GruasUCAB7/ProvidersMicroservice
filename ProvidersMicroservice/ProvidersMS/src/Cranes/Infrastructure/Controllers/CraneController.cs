﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Queries.GetAll;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Application.Queries.GetById;
using ProvidersMS.src.Cranes.Application.Queries.GetById.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Application.Types;

namespace ProvidersMS.src.Cranes.Infrastructure.Controllers
{
    [Route("provider/crane")]
    [ApiController]
    public class CraneController(
        ICraneRepository craneRepo,
        IdGenerator<string> idGenerator,
        IValidator<CreateCraneCommand> validatorCreate,
        IValidator<UpdateCraneCommand> validatorUpdate,
        ILoggerContract logger) : ControllerBase
    {
        private readonly ICraneRepository _craneRepo = craneRepo;
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IValidator<CreateCraneCommand> _validatorCreate = validatorCreate;
        private readonly IValidator<UpdateCraneCommand> _validatorUpdate = validatorUpdate;
        private readonly ILoggerContract _logger = logger;

        [HttpPost]
        [Authorize(Roles = "Admin, Provider")]
        public async Task<IActionResult> CreateCrane([FromBody] CreateCraneCommand data)
        {
            try
            {
                var command = new CreateCraneCommand(data.Brand, data.Model, data.Plate, data.CraneSize, data.Year);

                var validate = _validatorCreate.Validate(command);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCranes([FromQuery] GetAllCranesQuery data)
        {
            try
            {
                var query = new GetAllCranesQuery(data.PerPage, data.Page, data.IsActive);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Operator, Provider")]
        public async Task<IActionResult> GetCraneById(string id)
        {
            try
            {
                var query = new GetCraneByIdQuery(id);
                var handler = new GetCraneByIdQueryHandler(_craneRepo);
                var result = await handler.Execute(query);

                if (result.IsSuccessful)
                {
                    var crane = result.Unwrap();
                    _logger.Log("Crane found: {CraneId}", id);
                    return Ok(crane);
                }
                else
                {
                    _logger.Error("Failed to get crane: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("Failed to get crane by id", ex.Message);
                return StatusCode(500, "Crane not found");
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Provider")]
        public async Task<IActionResult> UpdateCrane([FromBody] UpdateCraneCommand data, string id)
        {
            try
            {
                var command = new UpdateCraneCommand(data.IsActive);
                
                var validate = _validatorUpdate.Validate(command);
                if (!validate.IsValid)
                {
                    var errors = validate.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.Error($"Validation failed for UpdateCraneCommand: {string.Join(", ", errors)}");
                    return StatusCode(400, errors);
                }

                var handler = new UpdateCraneCommandHandler(_craneRepo);
                var result = await handler.Execute((id, data));
                if (result.IsSuccessful)
                {
                    var crane = result.Unwrap();
                    _logger.Log("Crane updated: {CraneId}", id);
                    return Ok(crane);
                }
                else
                {
                    _logger.Error("Failed to update crane: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(409, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Exception("An error occurred while updating the crane.", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
