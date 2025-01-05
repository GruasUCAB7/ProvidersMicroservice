using ProvidersMS.Core.Domain.Events;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Providers.Domain.Events
{

    public class ProviderCreatedEvent(string dispatcherId, string name, ProviderCreated context) : DomainEvent<object>(dispatcherId, name, context) { }

    public class ProviderCreated(
        string id, 
        string rif, 
        string type, 
        List<CraneId> fleetOfCranes,
        List<DriverId> drivers
    )
    {
        public readonly string Id = id;
        public readonly string Rif = rif;
        public readonly string Type = type;
        public readonly List<CraneId> FleetOfCranes = fleetOfCranes;
        public readonly List<DriverId> Drivers = drivers;

        static public ProviderCreatedEvent CreateEvent(ProviderId Id, ProviderRif Rif, ProviderType type, List<CraneId> FleetOfCranes, List<DriverId> Drivers)
        {
            return new ProviderCreatedEvent(
                Id.GetValue(),
                typeof(ProviderCreated).Name,
                new ProviderCreated(
                    Id.GetValue(),
                    Rif.GetValue(),
                    type.GetValue(),
                    FleetOfCranes,
                    Drivers
                )
            );
        }
    }
}
