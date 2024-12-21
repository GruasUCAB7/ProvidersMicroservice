using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Providers.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.ValueObjects
{
    public class ProviderId : IValueObject<ProviderId>
    {
        private string Id { get; }

        public ProviderId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidProviderIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(ProviderId other)
        {
            return Id == other.Id;
        }
    }
}
