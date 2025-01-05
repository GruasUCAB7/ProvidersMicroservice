using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Providers.Domain.Exceptions
{
    public class InvalidProviderTypeException(string type) : DomainException(type)
    {
    }
}
