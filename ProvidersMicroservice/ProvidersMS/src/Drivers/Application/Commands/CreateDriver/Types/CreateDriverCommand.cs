namespace ProvidersMS.src.Drivers.Application.Commands.CreateDriver.Types
{
    public record CreateDriverCommand
    (
        string UserId,
        string DNI,
        bool IsActiveLicensed,
        string CraneAssigned,
        string DriverLocation
    );
}
