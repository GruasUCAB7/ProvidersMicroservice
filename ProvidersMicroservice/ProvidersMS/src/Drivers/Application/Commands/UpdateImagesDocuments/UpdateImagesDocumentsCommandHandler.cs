using Microsoft.AspNetCore.Mvc;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments;
using ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;

namespace ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments
{
    public class UpdateImagesDocumentsCommandHandler(
        IDriverRepository driverRepository,
        UploadImagesDocumentsCommandHandler uploadImagesDocuments
        ) : IService<UpdateDriverImagesCommand, EmptyResult>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly UploadImagesDocumentsCommandHandler _uploadImagesDocuments = uploadImagesDocuments;

        public async Task<Result<EmptyResult>> Execute(UpdateDriverImagesCommand data)
        {
            var driverOptional = await _driverRepository.GetById(data.DriverId);
            if (!driverOptional.HasValue)
            {
                return Result<EmptyResult>.Failure(new DriverNotFoundException());
            }

            var driver = driverOptional.Unwrap();

            var saveImageCommand = new UploadImageCommand(data.LicenseImage, data.DNIImage, data.RoadMedicalCertificateImage, data.CivilLiabilityImage);
            var saveImageResult = await _uploadImagesDocuments.Execute(saveImageCommand);
            if (saveImageResult.IsFailure)
            {
                return Result<EmptyResult>.Failure(new UnsavedImagesException());
            }

            var imageUrls = saveImageResult.Unwrap().Urls;
            driver.GetImagesDocuments().AddRange(imageUrls);
            var updateResult = await _driverRepository.UpdateDriverImages(driver);
            if (updateResult.IsFailure) 
            { 
                return Result<EmptyResult>.Failure(new UnsavedImagesException());
            }

            return Result<EmptyResult>.Success(new EmptyResult());
        }
    }
}
