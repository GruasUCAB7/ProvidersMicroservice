using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Cranes.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CraneId : IValueObject<CraneId>
    {
        private string Id { get; }

        public CraneId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidCraneIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(CraneId other)
        {
            return Id == other.Id;
        }
    }
}
