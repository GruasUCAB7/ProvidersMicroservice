using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Providers.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.ValueObjects
{

    public class ProviderType : IValueObject<ProviderType>
    {
        public static readonly string Interno = "Interno";
        public static readonly string Externo = "Externo";

        public string Type { get; }

        public ProviderType(string type)
        {
            if (type != Interno
                && type != Externo)
            {
                throw new InvalidProviderTypeException($"Invalid provider types: {type}. Allowed values are: {Interno}, {Externo}.");
            }
            Type = type;
        }

        public string GetValue()
        {
            return Type;
        }

        public bool Equals(ProviderType other)
        {
            return Type == other.Type;
        }
    }
}
