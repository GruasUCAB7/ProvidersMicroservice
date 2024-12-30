using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Types;

namespace ProvidersMS.src.Drivers.Application.Queries.GetAll
{
    public class GetAllDriversQueryHandler(IDriverRepository driverRepository) : IService<GetAllDriversQuery, GetDriverResponse[]>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<GetDriverResponse[]>> Execute(GetAllDriversQuery data)
        {
            var driver = await _driverRepository.GetAll(data);
            if (driver == null)
            {
                return Result<GetDriverResponse[]>.Failure(new DriverNotFoundException());
            }

            var response = driver.Select(driver => new GetDriverResponse(
                driver.GetId(),
                driver.GetDNI(),
                driver.GetIsActiveLicensed(),
                driver.GetImagesDocuments(),
                driver.GetCraneAssigned(),
            driver.GetIsAvailable(),
            new CoordinatesDto(driver.GetDriverLocationLatitude(), driver.GetDriverLocationLongitude())
            )
            ).ToArray();

            return Result<GetDriverResponse[]>.Success(response);
        }
    }
}
