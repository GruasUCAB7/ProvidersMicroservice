using ProvidersMS.src.Drivers.Application.Types;

namespace ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types
{
    public record UpdateDriverResponse
    (
        string Id,
        string DNI,
        bool IsActiveLicensed,
        List<string> ImagesDocuments,
        string CraneAssigned,
        bool IsAvailable,
        CoordinatesDto DriverLocation
    );
}
