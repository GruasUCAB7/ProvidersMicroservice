using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Exceptions;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Commands.CreateDriver.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Application.Commands.CreateDriver
{
    public class CreateDriverCommandHandler(
         IDriverRepository driverRepository,
         ICraneRepository craneRepository,
         IGoogleApiService googleApiService
    ) : IService<CreateDriverCommand, CreateDriverResponse>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly ICraneRepository _craneRepository = craneRepository;
        private readonly IGoogleApiService _googleApiService = googleApiService;

        public async Task<Result<CreateDriverResponse>> Execute(CreateDriverCommand data)
        {
            var isDriverExist = await _driverRepository.ExistByDNI(data.DNI);
            if (isDriverExist)
            {
                return Result<CreateDriverResponse>.Failure(new DriverAlreadyExistException(data.DNI));
            }

            var isCraneExist = await _craneRepository.GetById(data.CraneAssigned);
            if (!isCraneExist.HasValue)
            {
                return Result<CreateDriverResponse>.Failure(new CraneNotFoundException());
            }

            var isCraneIsActive = await _craneRepository.IsActiveCrane(data.CraneAssigned);
            if (isCraneIsActive == false)
            {
                return Result<CreateDriverResponse>.Failure(new CraneNotAvailableException());
            }

            var isDriverWithSameCraneExist = await _driverRepository.IsCraneAssociatedWithAnotherDriver(data.CraneAssigned);
            if (isDriverWithSameCraneExist)
            {
                return Result<CreateDriverResponse>.Failure(new CraneAssociatedWithDriverException());
            }

            var driverLocationResult = await _googleApiService.GetCoordinatesFromAddress(data.DriverLocation);
            if (!driverLocationResult.IsSuccessful)
            {
                return Result<CreateDriverResponse>.Failure(new CoordinatesNotFoundException(driverLocationResult.ErrorMessage));
            }

            var driver = Driver.CreateDriver(
                new DriverId(data.UserId),
                new DriverDNI(data.DNI),
                new DriverIsActiveLicensed(data.IsActiveLicensed),
                new List<string>(),
                new CraneId(data.CraneAssigned),
                new DriverLocation(driverLocationResult.Unwrap().Latitude, driverLocationResult.Unwrap().Longitude)
            );
            await _driverRepository.Save(driver);

            return Result<CreateDriverResponse>.Success(new CreateDriverResponse(data.UserId));
        }
    }
}
