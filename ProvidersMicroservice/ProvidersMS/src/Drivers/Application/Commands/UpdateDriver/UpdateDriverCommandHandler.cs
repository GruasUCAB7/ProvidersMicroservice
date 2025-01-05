using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;

namespace ProvidersMS.src.Drivers.Application.Commands.UpdateDriver
{
    public class UpdateDriverCommandHandler(IDriverRepository driverRepository, IGoogleApiService googleApiService) : IService<(string id, UpdateDriverCommand data), GetDriverResponse>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly IGoogleApiService _googleApiService = googleApiService;

        public async Task<Result<GetDriverResponse>> Execute((string id, UpdateDriverCommand data) request)
        {
            var driverOptional = await _driverRepository.GetById(request.id);
            if (!driverOptional.HasValue)
            {
                return Result<GetDriverResponse>.Failure(new DriverNotFoundException());
            }

            var driver = driverOptional.Unwrap();

            if (request.data.IsActiveLicensed.HasValue)
            {
                driver.SetIsActiveLicensed(request.data.IsActiveLicensed.Value);
            }

            if (!string.IsNullOrEmpty(request.data.CraneAssigned))
            {
                var craneId = new CraneId(request.data.CraneAssigned);
                driver.SetCraneAssigned(craneId);
            }

            if (request.data.IsAvailable.HasValue)
            {
                driver.SetIsAvailable(request.data.IsAvailable.Value);
            }

            if (!string.IsNullOrEmpty(request.data.DriverLocation))
            {
                var location = await _googleApiService.GetCoordinatesFromAddress(request.data.DriverLocation);
                if (location == null)
                {
                    return Result<GetDriverResponse>.Failure(new CoordinatesNotFoundException("Driver location not found."));
                }
            }

            if (request.data.IsActive.HasValue)
            {
                driver.SetIsActive(request.data.IsActive.Value);
            }

            var updateResult = await _driverRepository.Update(driver);
            if (updateResult.IsFailure)
            {
                return Result<GetDriverResponse>.Failure(new DriverUpdateFailedException());
            }

            var response = new GetDriverResponse(
                driver.GetId(),
                driver.GetDNI(),
                driver.GetIsActiveLicensed(),
                driver.GetImagesDocuments(),
                driver.GetCraneAssigned(),
                driver.GetIsAvailable(),
                new CoordinatesDto(driver.GetDriverLocationLatitude(), driver.GetDriverLocationLongitude()),
                driver.GetIsActive()
            );

            return Result<GetDriverResponse>.Success(response);
        }
    }
}
