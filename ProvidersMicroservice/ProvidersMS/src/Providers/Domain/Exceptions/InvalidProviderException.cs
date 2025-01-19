using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.Exceptions
{
    public class InvalidProviderException : DomainException
    {
        public InvalidProviderException() : base("Invalid provider")
        {
        }
    }
}
