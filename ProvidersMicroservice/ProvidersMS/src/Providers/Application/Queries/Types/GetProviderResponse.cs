namespace ProvidersMS.src.Providers.Application.Queries.Types
{
    public record GetProviderResponse
    (
        string Id,
        string Rif,
        string ProviderType,
        List<string> FleetOfCranes,
        List<string> Drivers
    );
}
