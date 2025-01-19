using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;

namespace ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation
{
    public class UpdateDriverLocationCommandHandler(IDriverRepository driverRepository) : IService<(string id, UpdateDriverLocationCommand data), GetDriverResponse>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<GetDriverResponse>> Execute((string id, UpdateDriverLocationCommand data) request)
        {
            var driverOptional = await _driverRepository.GetById(request.id);
            if (!driverOptional.HasValue)
            {
                return Result<GetDriverResponse>.Failure(new DriverNotFoundException());
            }

            var driver = driverOptional.Unwrap();

            await _driverRepository.UpdateDriverLocation(request.id, request.data.Latitude, request.data.Longitude);

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
