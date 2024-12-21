using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.Exceptions
{
    public class InvalidDriverDNIException : DomainException
    {
        public InvalidDriverDNIException() : base("Invalid driver DNI")
        {
        }
    }
}
