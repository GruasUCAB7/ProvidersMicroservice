using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Application.Exceptions;

namespace ProvidersMS.src.Cranes.Application.Commands.UpdateCrane
{
    public class UpdateCraneCommandHandler(ICraneRepository craneRepository) : IService<(string id, UpdateCraneCommand data), UpdateCraneResponse>
    {
        private readonly ICraneRepository _craneRepository = craneRepository;

        public async Task<Result<UpdateCraneResponse>> Execute((string id, UpdateCraneCommand data) request)
        {
            var craneOptional = await _craneRepository.GetById(request.id);
            if (!craneOptional.HasValue)
            {
                return Result<UpdateCraneResponse>.Failure(new CraneNotFoundException());
            }

            var crane = craneOptional.Unwrap();

            if (request.data.IsActive.HasValue) 
            { 
                crane.SetIsActive(request.data.IsActive.Value); 
            }

            var updateResult = await _craneRepository.Update(crane);
            if (updateResult.IsFailure)
            {
                return Result<UpdateCraneResponse>.Failure(new CraneUpdateFailedException());
            }

            var response = new UpdateCraneResponse(
                crane.GetId(),
                crane.GetBrand(),
                crane.GetModel(),
                crane.GetPlate(),
                crane.GetCraneSize(),
                crane.GetYear(),
                crane.GetIsActive()
            );

            return Result<UpdateCraneResponse>.Success(response);
        }
    }
}
