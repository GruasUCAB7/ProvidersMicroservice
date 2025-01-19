namespace ProvidersMS.src.Drivers.Application.Commands.UploadImagesDocuments.Types
{
    public record UploadImageCommand(
        IFormFile LicenseImage,
        IFormFile DNIImage,
        IFormFile RoadMedicalCertificateImage,
        IFormFile CivilLiabilityImage
    );
}
