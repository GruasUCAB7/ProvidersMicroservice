namespace ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types
{
    public record UpdateDriverCommand
    (
        bool? IsActiveLicensed,
        string? CraneAssigned,
        bool? IsAvailable,
        string? DriverLocation,
        bool? IsActive,
        List<string> ImagesDocuments
    );
}
