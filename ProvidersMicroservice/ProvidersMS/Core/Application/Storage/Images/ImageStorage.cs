using ProvidersMS.Core.Application.Storage.Images.Types;
using ProvidersMS.Core.Utils.Result;

namespace ProvidersMS.Core.Application.Storage.Images
{
    public interface ImageStorage
    {
        Task<Result<ImageSaved>> Save(string path);
        Task<Result<ImageSaved>> Delete(string url);
    }
}
