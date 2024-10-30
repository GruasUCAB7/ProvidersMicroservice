using ProvidersMS.Core.Application.Dtos;
using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain;

namespace ProvidersMS.src.Cranes.Application.Repositories
{
    public interface ICraneRepository
    {
        Task<bool> ExistByPlate(string plate);
        Task<List<Crane>> GetAll(PaginationDto data);
        Task<Optional<Crane>> GetById(string id);
        Task<Result<Crane>> Save(Crane crane);
    }
}
