using Microsoft.AspNetCore.Mvc;
using ProvidersMS.Core.Application.Services;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments.Types;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Repositories;

namespace ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments
{
    public class UpdateImagesDocumentsCommandHandler(
        IDriverRepository driverRepository
        ) : IService<UpdateDriverImagesCommand, EmptyResult>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<Result<EmptyResult>> Execute(UpdateDriverImagesCommand data)
        {
            var driverOptional = await _driverRepository.GetById(data.DriverId);
            if (!driverOptional.HasValue)
            {
                return Result<EmptyResult>.Failure(new DriverNotFoundException());
            }

            var driver = driverOptional.Unwrap();
            var imagesList = driver.GetImagesDocuments();
            if (data.ImagesUrl != null)
            {
                imagesList.AddRange(data.ImagesUrl);
            }

            driver.SetImagesDocuments(imagesList);

            var updateResult = await _driverRepository.Update(driver);
            if (updateResult.IsFailure)
            {
                return Result<EmptyResult>.Failure(new UnsavedImagesException());
            }

            return Result<EmptyResult>.Success(new EmptyResult());
        }
    }
}
