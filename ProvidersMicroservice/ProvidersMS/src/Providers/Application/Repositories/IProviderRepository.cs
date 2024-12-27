using ProvidersMS.Core.Utils.Optional;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Providers.Application.Models;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Domain;

namespace ProvidersMS.src.Providers.Application.Repositories
{
    public interface IProviderRepository
    {
        Task<bool> ExistByRif(string rif);
        Task<List<Provider>> GetAll(GetAllProvidersQuery data);
        Task<Optional<Provider>> GetById(string id);
        Task<Result<Provider>> Save(Provider provider);
        Task<Result<Provider>> Update(Provider provider);
        Task<bool> IsCraneAssociatedWithAnotherProvider(string craneId);
        Task<bool> IsDriverAssociatedWithAnotherProvider(string driverId);
        Task<List<Driver>> GetAvailableDrivers(GetAvailableDriversQuery data);
    }
}
