namespace ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types
{
    public record CreateCraneCommand(
        string Brand,
        string Model,
        string Plate,
        string CraneSize,
        int Year
    );
}
