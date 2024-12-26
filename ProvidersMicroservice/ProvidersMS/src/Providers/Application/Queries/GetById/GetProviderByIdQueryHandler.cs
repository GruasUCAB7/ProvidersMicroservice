using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Queries.GetById.Types;
using ProvidersMS.src.Providers.Application.Queries.Types;
using ProvidersMS.src.Providers.Application.Repositories;

namespace ProvidersMS.src.Providers.Application.Queries.GetById
{
    public class GetProviderByIdQueryHandler(IProviderRepository providerRepository) : IService<GetProviderByIdQuery, GetProviderResponse>
    {
        private readonly IProviderRepository _providerRepository = providerRepository;
        public async Task<Result<GetProviderResponse>> Execute(GetProviderByIdQuery data)
        {
            var providerOptional = await _providerRepository.GetById(data.Id);
            if (!providerOptional.HasValue)
            {
                return Result<GetProviderResponse>.Failure(new ProviderNotFoundException());
            }

            var provider = providerOptional.Unwrap();
            var response = new GetProviderResponse(
                provider.GetId(),
                provider.GetRif(),
                provider.GetProviderType(),
                provider.GetFleetOfCranes(),
                provider.GetDrivers(),
                provider.GetIsActive()
            );

            return Result<GetProviderResponse>.Success(response);
        }
    }
}
