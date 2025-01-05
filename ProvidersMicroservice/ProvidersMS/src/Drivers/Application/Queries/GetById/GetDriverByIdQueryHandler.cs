using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Queries.GetById.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;

namespace ProvidersMS.src.Drivers.Application.Queries.GetById
{
    public class GetDriverByIdQueryHandler(IDriverRepository driverRepository) : IService<GetDriverByIdQuery, GetDriverResponse>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<GetDriverResponse>> Execute(GetDriverByIdQuery data)
        {
            var driverOptional = await _driverRepository.GetById(data.Id);
            if (!driverOptional.HasValue)
            {
                return Result<GetDriverResponse>.Failure(new DriverNotFoundException());
            }

            var driver = driverOptional.Unwrap();
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
