using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.Exceptions
{
    public class InvalidDriverLocationException(string message) : DomainException(message)
    {
    }
}
