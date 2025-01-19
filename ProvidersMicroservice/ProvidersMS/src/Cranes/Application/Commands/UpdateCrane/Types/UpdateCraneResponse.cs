namespace ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types
{
    public record UpdateCraneResponse
    (
        string Id,
        string Brand,
        string Model,
        string Plate,
        string CraneSize,
        int Year,
        bool IsActive
    );
}
