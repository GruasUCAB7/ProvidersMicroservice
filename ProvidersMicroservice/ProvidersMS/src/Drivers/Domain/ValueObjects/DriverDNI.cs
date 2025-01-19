using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Drivers.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class DriverDNI : IValueObject<DriverDNI>
    {
        private string DNI { get; }

        public DriverDNI(string dni)
        {
            if (!DNIRegex.IsMatch(dni))
            {
                throw new InvalidDriverDNIException();
            }
            DNI = dni;
        }

        public string GetValue()
        {
            return DNI;
        }

        public bool Equals(DriverDNI other)
        {
            return DNI == other.DNI;
        }
    }
}
