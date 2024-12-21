namespace ProvidersMS.src.Drivers.Application.Types
{
    public record GetDriverResponse
    (
        string Id,
        string DNI,
        bool IsActiveLicensed,
        List<string> ImagesDocuments,
        string CraneAssigned,
        bool IsAvailable
    );
}
