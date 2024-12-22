using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Repositories;

namespace ProvidersMS.src.Providers.Application.Commands.UpdateProvider
{
    public class UpdateProviderCommandHandler(
        IProviderRepository providerRepository,
        ICraneRepository craneRepository,
        IDriverRepository driverRepository
    ) : IService<(string id, UpdateProviderCommand data), UpdateProviderResponse>
    {
        private readonly IProviderRepository _providerRepository = providerRepository;
        private readonly ICraneRepository _craneRepository = craneRepository;
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<UpdateProviderResponse>> Execute((string id, UpdateProviderCommand data) request)
        {
            var providerOptional = await _providerRepository.GetById(request.id);
            if (!providerOptional.HasValue)
            {
                return Result<UpdateProviderResponse>.Failure(new ProviderNotFoundException());
            }

            var provider = providerOptional.Unwrap();

            if (request.data.FleetOfCranes != null)
            {
                var craneIdsSet = new HashSet<string>();
                foreach (var craneId in request.data.FleetOfCranes)
                {
                    if (!craneIdsSet.Add(craneId))
                    {
                        throw new DuplicateCraneIdsException();
                    }

                    var crane = await _craneRepository.GetById(craneId);
                    if (!crane.HasValue)
                    {
                        throw new CranesNotFoundException();
                    }

                    var isCraneAssociated = await _providerRepository.IsCraneAssociatedWithAnotherProvider(craneId);
                    if (isCraneAssociated)
                    {
                        throw new CraneAlreadyAssociatedException(craneId);
                    }
                }

                var craneIds = request.data.FleetOfCranes.Select(c => new CraneId(c)).ToList();
                provider.AddCranes(craneIds);
            }

            if (request.data.Drivers != null)
            {
                var driverIdsSet = new HashSet<string>();
                foreach (var driverId in request.data.Drivers)
                {
                    if (!driverIdsSet.Add(driverId))
                    {
                        throw new DuplicateDriverIdsException();
                    }

                    var exists = await _driverRepository.GetById(driverId);
                    if (!exists.HasValue)
                    {
                        throw new DriversNotFoundException();
                    }

                    var isDriverAssociated = await _providerRepository.IsDriverAssociatedWithAnotherProvider(driverId);
                    if (isDriverAssociated)
                    {
                        throw new DriverAlreadyAssociatedException(driverId);
                    }
                }

                var driverIds = request.data.Drivers.Select(d => new DriverId(d)).ToList();
                provider.AddDrivers(driverIds);
            }

            var updateResult = await _providerRepository.Update(provider);
            if (updateResult.IsFailure)
            {
                return Result<UpdateProviderResponse>.Failure(new ProviderUpdateFailedException());
            }

            var response = new UpdateProviderResponse(
                provider.GetId(),
                provider.GetRif(),
                provider.GetProviderType(),
                provider.GetFleetOfCranes(),
                provider.GetDrivers()
            );

            return Result<UpdateProviderResponse>.Success(response);
        }
    }
}
