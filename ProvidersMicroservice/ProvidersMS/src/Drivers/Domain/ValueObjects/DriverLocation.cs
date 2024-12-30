using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class DriverLocation : IValueObject<DriverLocation>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public DriverLocation(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                throw new InvalidDriverLocationException("Latitude must be between -90 and 90 degrees.");
            }

            if (longitude < -180 || longitude > 180)
            {
                throw new InvalidDriverLocationException("Longitude must be between -180 and 180 degrees.");
            }

            Latitude = latitude;
            Longitude = longitude;
        }

        public double GetLatitude()
        {
            return Latitude;
        }

        public double GetLongitude()
        {
            return Longitude;
        }

        public bool Equals(DriverLocation other)
        {
            return Latitude == other.Latitude && Longitude == other.Longitude;
        }
    }
}
