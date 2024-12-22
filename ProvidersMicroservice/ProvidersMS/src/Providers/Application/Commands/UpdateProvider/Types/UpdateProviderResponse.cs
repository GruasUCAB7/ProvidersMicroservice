namespace ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types
{
    public record UpdateProviderResponse
    (
        string Id,
        string Rif,
        string ProviderType,
        List<string> FleetOfCranes,
        List<string> Drivers
    );
}
