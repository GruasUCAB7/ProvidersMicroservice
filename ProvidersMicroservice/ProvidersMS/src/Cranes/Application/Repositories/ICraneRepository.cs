using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Domain;

namespace ProvidersMS.src.Cranes.Application.Repositories
{
    public interface ICraneRepository
    {
        Task<bool> ExistByPlate(string plate);
        Task<List<Crane>> GetAll(GetAllCranesQuery data);
        Task<Optional<Crane>> GetById(string id);
        Task<Result<Crane>> Save(Crane crane);
        Task<Result<Crane>> Update(Crane crane);
        Task<bool> IsActiveCrane(string craneId);
    }
}
