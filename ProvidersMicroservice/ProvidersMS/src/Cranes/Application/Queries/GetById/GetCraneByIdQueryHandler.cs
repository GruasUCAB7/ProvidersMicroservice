using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Queries.GetById.Types;
using ProvidersMS.src.Cranes.Application.Queries.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Application.Exceptions;

namespace ProvidersMS.src.Cranes.Application.Queries.GetById
{
    public class GetCraneByIdQueryHandler(ICraneRepository craneRepository) : IService<GetCraneByIdQuery, GetCraneResponse>
    {
        private readonly ICraneRepository _craneRepository = craneRepository;
        public async Task<Result<GetCraneResponse>> Execute(GetCraneByIdQuery data)
        {
            var craneOptional = await _craneRepository.GetById(data.Id);
            if (!craneOptional.HasValue)
            {
                return Result<GetCraneResponse>.Failure(new CraneNotFoundException());
            }

            var crane = craneOptional.Unwrap();
            Console.WriteLine(crane);
            var response = new GetCraneResponse(
                crane.GetId(),
                crane.GetBrand(),
                crane.GetModel(),
                crane.GetPlate(),
                crane.GetCraneType(),
                crane.GetYear(),
                crane.GetIsActive()
            );

            return Result<GetCraneResponse>.Success(response);  
        }
    }
}
