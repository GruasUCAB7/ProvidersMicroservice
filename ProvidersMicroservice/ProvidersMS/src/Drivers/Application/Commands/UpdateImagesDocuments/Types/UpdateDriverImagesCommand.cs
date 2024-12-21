namespace ProvidersMS.src.Drivers.Application.Commands.UpdateImagesDocuments.Types
{
    public record UpdateDriverImagesCommand(
        string DriverId, 
        IFormFile LicenseImage,
        IFormFile DNIImage,
        IFormFile RoadMedicalCertificateImage,
        IFormFile CivilLiabilityImage);

}
