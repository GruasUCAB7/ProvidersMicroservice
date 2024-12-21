using ProvidersMS.Core.Domain.Aggregates;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Providers.Domain.Events;
using ProvidersMS.src.Providers.Domain.Exceptions;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Providers.Domain
{
    public class Provider(ProviderId id) : AggregateRoot<ProviderId>(id)
    {
        private ProviderId _id = id;
        private ProviderRif _rif;
        private ProviderType _type;
        private List<CraneId> _fleetOfCranes;
        //private List<Driver> _drivers;

        public string GetId() => _id.GetValue();
        public string GetRif() => _rif.GetValue();
        public string GetType() => _type.GetValue();
        public List<string> GetFleetOfCranes() => _fleetOfCranes.Select(c => c.GetValue()).ToList();

        public static Provider CreateProvider(ProviderId id, ProviderRif rif, ProviderType type, List<CraneId> fleetOfCranes)
        {
            var provider = new Provider(id);
            provider.Apply(ProviderCreated.CreateEvent(id, rif, type, fleetOfCranes));
            return provider;
        }

        public void OnProviderCreatedEvent(ProviderCreated context)
        {
            _id = new ProviderId(context.Id);
            _rif = new ProviderRif(context.Rif);
            _type = Enum.Parse<ProviderType>(context.Type);
            _fleetOfCranes = context.FleetOfCranes.Select(id => new CraneId(id)).ToList();
        }

        public override void ValidateState()
        {
            if (_id == null || _rif == null)
            {
                throw new InvalidProviderException();
            }
        }
    }
}
