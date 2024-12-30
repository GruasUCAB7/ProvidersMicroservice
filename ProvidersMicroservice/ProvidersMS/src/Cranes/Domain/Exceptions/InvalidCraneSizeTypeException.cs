using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneSizeException(string size) : DomainException(size)
    {
    }
}
