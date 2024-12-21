using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.Core.Application.Storage.Images.Types;
using ProvidersMS.Core.Utils.Result;

namespace ProvidersMS.Core.Infrastructure.Storage
{
    public class CloudinaryImageStorage(Cloudinary cloudinary) : ImageStorage
    {
        private readonly Cloudinary _cloudinary = cloudinary;

        public async Task<Result<ImageSaved>> Save(string path)
        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return Result<ImageSaved>.Success(new ImageSaved { Url = uploadResult.SecureUrl.ToString() });
            }
            catch (Exception ex)
            {
                return Result<ImageSaved>.Failure(ex);
            }
        }

        public async Task<Result<ImageSaved>> Delete(string url)
        {
            try
            {
                var deletionParams = new DeletionParams(url)
                {
                    ResourceType = ResourceType.Image
                };

                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                if (deletionResult.Result == "ok")
                {
                    return Result<ImageSaved>.Success(new ImageSaved { Url = url });
                }
                else
                {
                    return Result<ImageSaved>.Failure(new Exception("Failed to delete image"));
                }
            }
            catch (Exception ex)
            {
                return Result<ImageSaved>.Failure(ex);
            }
        }
    }
}
