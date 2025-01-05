using ProvidersMS.Core.Domain.Aggregates;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
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
        private List<DriverId> _drivers;
        private bool _isActive = true;

        public string GetId() => _id.GetValue();
        public string GetRif() => _rif.GetValue();
        public string GetProviderType() => _type.GetValue();
        public List<string> GetFleetOfCranes() => _fleetOfCranes.Select(c => c.GetValue()).ToList();
        public List<string> GetDrivers() => _drivers.Select(d => d.GetValue()).ToList();
        public bool GetIsActive() => _isActive;
        public bool SetIsActive(bool isActive) => _isActive = isActive;

        public void AddCranes(List<CraneId> craneIds)
        {
            foreach (var craneId in craneIds)
            {
                if (_fleetOfCranes.Contains(craneId))
                {
                    throw new Exception("Crane already exists in the fleet");
                }
                _fleetOfCranes.Add(craneId);
            }
        }

        public void AddDrivers(List<DriverId> driverIds)
        {
            foreach (var driverId in driverIds)
            {
                if (_drivers.Contains(driverId))
                {
                    throw new Exception("Driver already exists in the fleet");
                }
                _drivers.Add(driverId);
            }
        }

        public static Provider CreateProvider(
            ProviderId id, 
            ProviderRif rif, 
            ProviderType type, 
            List<CraneId> fleetOfCranes, 
            List<DriverId> drivers)
        {
            var provider = new Provider(id);
            provider.Apply(ProviderCreated.CreateEvent(id, rif, type, fleetOfCranes, drivers));
            return provider;
        }

        public void OnProviderCreatedEvent(ProviderCreated context)
        {
            _id = new ProviderId(context.Id);
            _rif = new ProviderRif(context.Rif);
            _type = new ProviderType(context.Type);
            _fleetOfCranes = new List<CraneId>(context.FleetOfCranes);
            _drivers = new List<DriverId>(context.Drivers);
        }

        public override void ValidateState()
        {
            if (_id == null || _rif == null || _type ==null)
            {
                throw new InvalidProviderException();
            }
        }
    }
}
