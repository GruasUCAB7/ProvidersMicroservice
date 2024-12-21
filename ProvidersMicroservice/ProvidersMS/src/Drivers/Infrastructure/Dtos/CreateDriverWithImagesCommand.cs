namespace ProvidersMS.src.Drivers.Infrastructure.Dtos
{
    public record CreateDriverWithImagesCommand(
        string DNI,
        bool IsActiveLicensed,
        string CraneAssigned,
        IFormFile LicenseImage,
        IFormFile DNIImage,
        IFormFile RoadMedicalCertificateImage,
        IFormFile CivilLiabilityImage
    );
}
