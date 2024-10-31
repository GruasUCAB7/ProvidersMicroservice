namespace ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types
{
    public record UpdateCraneResponse
    (
        string Id,
        string Brand,
        string Model,
        string Plate,
        string CraneType,
        int Year,
        bool IsActive
    );
}
