using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.Exceptions
{
    public class InvalidDriverException : DomainException
    {
        public InvalidDriverException() : base("Invalid driver")
        {
        }
    }
}
