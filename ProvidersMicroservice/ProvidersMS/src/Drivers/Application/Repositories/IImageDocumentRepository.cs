using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Domain.Entities.ImagesDocuments;

namespace ProvidersMS.src.Drivers.Application.Repositories
{
    public interface IImageDocumentRepository
    {
        Task<Result<List<ImageDocument>>> Save(ImageDocument imageDocument);
        Task<Core.Utils.Optional.Optional<ImageDocument>> GetByUrl(string url);
        Task Delete(string id);
    }
}
