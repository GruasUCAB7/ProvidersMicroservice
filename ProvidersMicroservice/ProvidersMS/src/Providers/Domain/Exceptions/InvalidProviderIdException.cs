using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.Exceptions
{
    public class InvalidProviderIdException : DomainException
    {
        public InvalidProviderIdException() : base("Invalid provider ID")
        {
        }
    }
}
