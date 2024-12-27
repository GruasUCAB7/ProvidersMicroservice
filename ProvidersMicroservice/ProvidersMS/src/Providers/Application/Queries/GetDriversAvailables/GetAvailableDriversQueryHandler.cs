using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Application.Repositories;

namespace ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables
{
    public class GetAvailableDriversQueryHandler(IProviderRepository providerRepository) : IService<GetAvailableDriversQuery, GetAvailableDriversResponse[]>
    {
        private readonly IProviderRepository _providerRepository = providerRepository;
        public async Task<Result<GetAvailableDriversResponse[]>> Execute(GetAvailableDriversQuery data)
        {
            var provider = await _providerRepository.GetAvailableDrivers(data);
            if (provider == null)
            {
                return Result<GetAvailableDriversResponse[]>.Failure(new DriversNotFoundException());
            }

            var response = provider.Select(provider => new GetAvailableDriversResponse(
                provider.Id,
                provider.CraneAssigned
                )
            ).ToArray();

            return Result<GetAvailableDriversResponse[]>.Success(response);
        }
    }
}
