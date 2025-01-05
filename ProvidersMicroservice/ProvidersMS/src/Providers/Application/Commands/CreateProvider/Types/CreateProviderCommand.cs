namespace ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types
{
    public record CreateProviderCommand(
        string UserId,
        string Rif,
        string ProviderType,
        List<string>? FleetOfCranes,
        List<string>? Drivers,
        string TokenJWT
    );
}
