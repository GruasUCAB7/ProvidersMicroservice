namespace ProvidersMS.src.Cranes.Application.Types
{
    public record GetCraneResponse
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
