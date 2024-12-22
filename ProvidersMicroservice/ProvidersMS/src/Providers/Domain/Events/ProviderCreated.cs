using ProvidersMS.Core.Domain.Events;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Providers.Domain.Events
{

    public class ProviderCreatedEvent(string dispatcherId, string name, ProviderCreated context) : DomainEvent<object>(dispatcherId, name, context) { }

    public class ProviderCreated(string id, string rif, string type, string[] fleetOfCranes, string[] drivers)
    {
        public readonly string Id = id;
        public readonly string Rif = rif;
        public readonly string Type = type;
        public readonly string[] FleetOfCranes = fleetOfCranes;
        public readonly string[] Drivers = drivers;

        static public ProviderCreatedEvent CreateEvent(ProviderId Id, ProviderRif Rif, ProviderType type, List<CraneId> FleetOfCranes, List<DriverId> Drivers)
        {
            return new ProviderCreatedEvent(
                Id.GetValue(),
                typeof(ProviderCreated).Name,
                new ProviderCreated(
                    Id.GetValue(),
                    Rif.GetValue(),
                    type.GetValue(),
                    FleetOfCranes.Select(c => c.GetValue()).ToArray(),
                    Drivers.Select(d => d.GetValue()).ToArray()
                )
            );
        }
    }
}
