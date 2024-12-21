using ProvidersMS.src.Providers.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.ValueObjects
{
    public static class ProviderTypeValue
    {
        public static string GetValue(this ProviderType providerType)
        {
            if (providerType == ProviderType.Interno || providerType == ProviderType.Externo)
            {
                return providerType.ToString();
            }
            throw new InvalidProviderTypeException();
        }
    }
}
