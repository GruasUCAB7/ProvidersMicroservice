namespace ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types
{
    public record UpdateProviderCommand
    (
        List<string>? FleetOfCranes,
        List<string>? Drivers,
        bool? IsActive
    );
}
