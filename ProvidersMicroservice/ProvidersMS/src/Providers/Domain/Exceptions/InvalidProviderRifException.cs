using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.Exceptions
{
    public class InvalidProviderRifException : DomainException
    {
        public InvalidProviderRifException() : base("Invalid provider Rif")
        {
        }
    }
}
