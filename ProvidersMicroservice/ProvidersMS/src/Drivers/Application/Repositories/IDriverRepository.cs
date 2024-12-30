using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Drivers.Domain;

namespace ProvidersMS.src.Drivers.Application.Repositories
{
    public interface IDriverRepository
    {
        Task<bool> ExistByDNI(string dni);
        Task<List<Driver>> GetAll(GetAllDriversQuery data);
        Task<Optional<Driver>> GetById(string id);
        Task<Result<Driver>> Save(Driver driver);
        Task<Result<object>> UpdateDriverImages(Driver driver);
        Task<Result<Driver>> Update(Driver driver);
        Task ValidateUpdateTimeDriver();
    }
}
