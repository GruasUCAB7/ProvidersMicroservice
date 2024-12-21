using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.Exceptions
{
    public class InvalidProviderTypeException : DomainException
    {
        public InvalidProviderTypeException() : base("Invalid provider type")
        {
        }
    }
}
