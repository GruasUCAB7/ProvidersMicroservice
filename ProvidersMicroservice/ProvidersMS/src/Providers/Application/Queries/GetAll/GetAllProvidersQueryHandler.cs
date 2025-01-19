using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.Types;
using ProvidersMS.src.Providers.Application.Repositories;

namespace ProvidersMS.src.Providers.Application.Queries.GetAll
{
    public class GetAllProvidersQueryHandler(IProviderRepository providerRepository) : IService<GetAllProvidersQuery, GetProviderResponse[]>
    {
        private readonly IProviderRepository _providerRepository = providerRepository;
        public async Task<Result<GetProviderResponse[]>> Execute(GetAllProvidersQuery data)
        {
            var provider = await _providerRepository.GetAll(data);
            if (provider == null)
            {
                return Result<GetProviderResponse[]>.Failure(new ProviderNotFoundException());
            }

            var response = provider.Select(provider => new GetProviderResponse(
                provider.GetId(),
                provider.GetRif(),
                provider.GetProviderType(),
                provider.GetFleetOfCranes(),
                provider.GetDrivers(),
                provider.GetIsActive()
                )
            ).ToArray();

            return Result<GetProviderResponse[]>.Success(response);
        }
    }
}
