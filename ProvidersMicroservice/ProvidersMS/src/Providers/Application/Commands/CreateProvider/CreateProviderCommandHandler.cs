using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Providers.Application.Commands.CreateProvider
{
    public class CreateProviderCommandHandler(
        IProviderRepository providerRepository,
        ICraneRepository craneRepository,
        IDriverRepository driverRepository
    ) : IService<CreateProviderCommand, CreateProviderResponse>
    {
        private readonly IProviderRepository _providerRepository = providerRepository;
        private readonly ICraneRepository _craneRepository = craneRepository;
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<CreateProviderResponse>> Execute(CreateProviderCommand data)
        {
            var isProviderExist = await _providerRepository.ExistByRif(data.Rif);
            if (isProviderExist)
            {
                return Result<CreateProviderResponse>.Failure(new ProviderAlreadyExistException(data.Rif));
            }

            if (data.FleetOfCranes != null)
            {
                var craneIdsSet = new HashSet<string>();
                foreach (var craneId in data.FleetOfCranes)
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
            }

            if (data.Drivers != null)
            {
                var driverIdsSet = new HashSet<string>();
                foreach (var driverId in data.Drivers)
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
                        throw new CraneAlreadyAssociatedException(driverId);
                    }
                }
            }

            var fleetOfCranes = data.FleetOfCranes?.Select(crane => new CraneId(crane)).ToList();
            var drivers = data.Drivers?.Select(driver => new DriverId(driver)).ToList();

            var provider = Provider.CreateProvider(
                new ProviderId(data.UserId),
                new ProviderRif(data.Rif),
                new ProviderType(data.ProviderType),
                fleetOfCranes,
                drivers
            );
            await _providerRepository.Save(provider);

            return Result<CreateProviderResponse>.Success(new CreateProviderResponse(data.UserId));
        }
    }
}
