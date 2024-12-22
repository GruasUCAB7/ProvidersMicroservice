namespace ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types
{
    public record CreateProviderCommand(
        string Rif,
        string ProviderType,
        List<string>? FleetOfCranes,
        List<string>? Drivers
    );
}
