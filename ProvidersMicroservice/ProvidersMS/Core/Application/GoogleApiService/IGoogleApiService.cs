using ProvidersMS.Core.Utils.Result;
using static ProvidersMS.Core.Infrastructure.GoogleMaps.GoogleApiService;

namespace ProvidersMS.Core.Application.GoogleApiService
{
    public interface IGoogleApiService
    {
        Task<Result<Coordinates>> GetCoordinatesFromAddress(string address);
    }
}
