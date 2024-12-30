using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Exceptions;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Application.Commands.CreateCrane
{
    public class CreateCraneCommandHandler(
        ICraneRepository craneRepository,
        IdGenerator<string> idGenerator
    ) : IService<CreateCraneCommand, CreateCraneResponse>
    {
        private readonly ICraneRepository _craneRepository = craneRepository;
        private readonly IdGenerator<string> _idGenerator = idGenerator;

        public async Task<Result<CreateCraneResponse>> Execute(CreateCraneCommand data)
        {
            var isCraneExist = await _craneRepository.ExistByPlate(data.Plate);
            if (isCraneExist) 
            {
                return Result<CreateCraneResponse>.Failure(new CraneAlreadyExistException(data.Plate));
            }

            var id = _idGenerator.Generate();
            var crane = Crane.CreateCrane(
                new CraneId(id),
                new CraneBrand(data.Brand),
                new CraneModel(data.Model),
                new CranePlate(data.Plate),
                new CraneSize(data.CraneSize),
                new CraneYear(data.Year)
            );
            await _craneRepository.Save(crane);

            return Result<CreateCraneResponse>.Success(new CreateCraneResponse(id));
        }
    }
}
