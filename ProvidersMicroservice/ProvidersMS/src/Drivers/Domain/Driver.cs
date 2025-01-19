using ProvidersMS.Core.Domain.Aggregates;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.Events;
using ProvidersMS.src.Drivers.Domain.Exceptions;
using ProvidersMS.src.Drivers.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Domain
{
    public class Driver(DriverId id) : AggregateRoot<DriverId>(id)
    {
        private DriverId _id = id;
        private DriverDNI _dni;
        private DriverIsActiveLicensed _isActiveLicensed;
        private List<string> _imagesDocuments;
        private CraneId _craneAssigned;
        private bool _isAvailable = true;
        private DriverLocation _location;
        private bool _isActive = true; 

        public string GetId() => _id.GetValue();
        public string GetDNI() => _dni.GetValue();
        public bool GetIsActiveLicensed() => _isActiveLicensed.GetValue();
        public List<string> GetImagesDocuments() => _imagesDocuments;
        public string GetCraneAssigned() => _craneAssigned.GetValue();
        public bool GetIsAvailable() => _isAvailable;
        public bool GetIsActive() => _isActive;
        public double GetDriverLocationLatitude() => _location.GetLatitude();
        public double GetDriverLocationLongitude() => _location.GetLongitude();
        public void SetIsActiveLicensed(bool isActiveLicensed) => _isActiveLicensed = new DriverIsActiveLicensed(isActiveLicensed);
        public void SetCraneAssigned(CraneId craneAssigned) => _craneAssigned = craneAssigned;
        public bool SetIsAvailable(bool isAvailable) => _isAvailable = isAvailable;
        public bool SetIsActive(bool isActive) => _isActive = isActive;
        public void SetImagesDocuments(List<string> imagesDocuments) => _imagesDocuments = imagesDocuments;
        public void SetDriverLocation(double latitude, double longitude)
        {
            var newLocation = new DriverLocation(latitude, longitude);
            _location = newLocation;
        }


        public static Driver CreateDriver(DriverId id, DriverDNI dni, DriverIsActiveLicensed isActiveLicensed, List<string> imagesDocuments, CraneId craneAssigned, DriverLocation driverLocation)
        {
            var provider = new Driver(id);
            provider.Apply(DriverCreated.CreateEvent(id, dni, isActiveLicensed, imagesDocuments, craneAssigned, driverLocation));
            return provider;
        }

        public void OnDriverCreatedEvent(DriverCreated context)
        {
            _id = new DriverId(context.Id);
            _dni = new DriverDNI(context.DNI);
            _isActiveLicensed = new DriverIsActiveLicensed(context.IsActiveLicensed);
            _imagesDocuments = new List<string>(context.ImagesDocuments);
            _craneAssigned = new CraneId(context.CraneAssigned);
            _location = new DriverLocation(context.DriverLocation.Latitude, context.DriverLocation.Longitude);
        }

        public override void ValidateState()
        {
            if (_id == null || _craneAssigned == null || _dni == null || _isActiveLicensed == null || _imagesDocuments == null || _location == null)
            {
                throw new InvalidDriverException();
            }
        }
    }
}
