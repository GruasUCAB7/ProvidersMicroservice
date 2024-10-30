namespace ProvidersMS.src.Cranes.Application.Queries.Types
{
    public record GetCraneResponse
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
