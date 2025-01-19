using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.Exceptions
{
    public class InvalidDriverIdException : DomainException
    {
        public InvalidDriverIdException() : base("Invalid driver ID")
        {
        }
    }
}
