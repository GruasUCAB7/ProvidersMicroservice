using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Application.Queries.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Application.Exceptions;

namespace ProvidersMS.src.Cranes.Application.Queries.GetAll
{
    public class GetAllCranesQueryHandler(ICraneRepository craneRepository) : IService<GetAllCranesQuery, GetCraneResponse[]>
    {
        private readonly ICraneRepository _craneRepository = craneRepository;
        public async Task<Result<GetCraneResponse[]>> Execute(GetAllCranesQuery data)
        {
            var crane = await _craneRepository.GetAll(data);
            if (crane == null)
            {
                return Result<GetCraneResponse[]>.Failure(new CraneNotFoundException());
            }

            var response = crane.Select(crane => new GetCraneResponse(
                crane.GetId(),
                crane.GetBrand(),
                crane.GetModel(),
                crane.GetPlate(),
                crane.GetCraneType(),
                crane.GetYear(),
                crane.GetIsActive()
                )
            ).ToArray();

            return Result<GetCraneResponse[]>.Success(response);
        }
    }
}
