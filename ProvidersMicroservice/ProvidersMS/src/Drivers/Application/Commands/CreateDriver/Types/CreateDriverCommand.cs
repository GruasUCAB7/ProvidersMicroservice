namespace ProvidersMS.src.Drivers.Application.Commands.CreateDriver.Types
{
    public record CreateDriverCommand
    (
        string DNI,
        bool IsActiveLicensed,
        string CraneAssigned
    );
}
