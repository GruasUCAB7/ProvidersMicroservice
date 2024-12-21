using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Drivers.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class DriverId : IValueObject<DriverId>
    {
        private string Id { get; }

        public DriverId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidDriverIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(DriverId other)
        {
            return Id == other.Id;
        }
    }
}
