using ProvidersMS.Core.Application.IdGenerator;
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
        IdGenerator<string> idGenerator
    ) : IService<CreateDriverCommand, CreateDriverResponse>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly ICraneRepository _craneRepository = craneRepository;
        private readonly IdGenerator<string> _idGenerator = idGenerator;

        public async Task<Result<CreateDriverResponse>> Execute(CreateDriverCommand data)
        {
            var isDriverExist = await _driverRepository.ExistByDNI(data.DNI);
            if (isDriverExist) 
            { 
                return Result<CreateDriverResponse>.Failure(new DriverAlreadyExistException(data.DNI));
            }

            var isCraneExist = await _craneRepository.GetById(data.CraneAssigned);
            if (isCraneExist.Unwrap().GetId() != data.CraneAssigned)
            {
                throw new CraneNotFoundException();
            }

            var isCraneIsActive = await _craneRepository.IsActiveCrane(data.CraneAssigned);
            if (isCraneIsActive == false)
            {
                throw new CraneNotAvailableException();
            }

            var id = _idGenerator.Generate();
            var driver = Driver.CreateDriver(
                new DriverId(id),
                new DriverDNI(data.DNI),
                new DriverIsActiveLicensed(data.IsActiveLicensed),
                new List<string>(),
                new CraneId(data.CraneAssigned)
            );
            await _driverRepository.Save(driver);

            return Result<CreateDriverResponse>.Success(new CreateDriverResponse(id));
        }
    }
}
