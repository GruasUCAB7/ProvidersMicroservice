using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Providers.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.ValueObjects
{
    public class ProviderRif : IValueObject<ProviderRif>
    {
        private string Rif { get; }

        public ProviderRif(string rif)
        {
            if (!RifRegex.IsMatch(rif))
            {
                throw new InvalidProviderRifException();
            }
            Rif = rif;
        }

        public string GetValue()
        {
            return Rif;
        }

        public bool Equals(ProviderRif other)
        {
            return Rif == other.Rif;
        }

        public override string ToString()
        {
            return Rif;
        }
    }
}
