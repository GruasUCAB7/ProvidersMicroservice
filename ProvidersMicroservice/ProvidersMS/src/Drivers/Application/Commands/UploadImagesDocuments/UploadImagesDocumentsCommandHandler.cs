using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Images.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments.Types;
using ProvidersMS.src.Drivers.Domain.Entities.ImagesDocuments;

namespace ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments
{
    public class UploadImagesDocumentsCommandHandler(
        IdGenerator<string> idGenerator,
        IImageDocumentRepository imageRepository,
        ImageStorage imageStorage
    ) : IService<UploadImageCommand, UploadImageResponse>
    {
        private readonly IdGenerator<string> _idGenerator = idGenerator;
        private readonly IImageDocumentRepository _imageRepository = imageRepository;
        private readonly ImageStorage _imageStorage = imageStorage;

        public async Task<Result<UploadImageResponse>> Execute(UploadImageCommand data)
        {
            var imageUrls = new List<string>();

            if (data.LicenseImage != null)
            {
                var fileName = data.LicenseImage.FileName;
                var filePath = Path.Combine("uploads", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await data.LicenseImage.CopyToAsync(stream);
                }

                var storageResult = await _imageStorage.Save(filePath);
                if (storageResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                var imageId = _idGenerator.Generate();
                var imageSource = storageResult.Unwrap().Url;
                var image = ImageDocument.CreateImageDocument(imageId, imageSource);

                var saveResult = await _imageRepository.Save(image);
                if (saveResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                imageUrls.Add(imageSource);
                File.Delete(filePath);
            }

            if (data.DNIImage != null)
            {
                var fileName = data.DNIImage.FileName;
                var filePath = Path.Combine("uploads", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await data.DNIImage.CopyToAsync(stream);
                }

                var storageResult = await _imageStorage.Save(filePath);
                if (storageResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                var imageId = _idGenerator.Generate();
                var imageSource = storageResult.Unwrap().Url;
                var image = ImageDocument.CreateImageDocument(imageId, imageSource);

                var saveResult = await _imageRepository.Save(image);
                if (saveResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                imageUrls.Add(imageSource);
                File.Delete(filePath);
            }

            if (data.RoadMedicalCertificateImage != null)
            {
                var fileName = data.RoadMedicalCertificateImage.FileName;
                var filePath = Path.Combine("uploads", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await data.RoadMedicalCertificateImage.CopyToAsync(stream);
                }

                var storageResult = await _imageStorage.Save(filePath);
                if (storageResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                var imageId = _idGenerator.Generate();
                var imageSource = storageResult.Unwrap().Url;
                var image = ImageDocument.CreateImageDocument(imageId, imageSource);

                var saveResult = await _imageRepository.Save(image);
                if (saveResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                imageUrls.Add(imageSource);
                File.Delete(filePath);
            }

            if (data.CivilLiabilityImage != null)
            {
                var fileName = data.CivilLiabilityImage.FileName;
                var filePath = Path.Combine("uploads", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await data.CivilLiabilityImage.CopyToAsync(stream);
                }

                var storageResult = await _imageStorage.Save(filePath);
                if (storageResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }
                var imageId = _idGenerator.Generate();
                var imageSource = storageResult.Unwrap().Url;
                var image = ImageDocument.CreateImageDocument(imageId, imageSource);

                var saveResult = await _imageRepository.Save(image);
                if (saveResult.IsFailure)
                {
                    return Result<UploadImageResponse>.Failure(new UploadImageException());
                }

                imageUrls.Add(imageSource);
                File.Delete(filePath);
            }

            return Result<UploadImageResponse>.Success(new UploadImageResponse(imageUrls));
        }
    }
}
