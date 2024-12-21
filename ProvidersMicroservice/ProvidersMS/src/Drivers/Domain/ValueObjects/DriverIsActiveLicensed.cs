using ProvidersMS.Core.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class DriverIsActiveLicensed(bool activeLicensed) : IValueObject<DriverIsActiveLicensed>
    {
        private readonly bool ActiveLicensed = activeLicensed;

        public bool GetValue()
        {
            return ActiveLicensed;
        }

        public bool Equals(DriverIsActiveLicensed other) 
        {
            return ActiveLicensed == other.ActiveLicensed;
        }
    }
}
