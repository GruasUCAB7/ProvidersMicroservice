using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneIdException : DomainException
    {
        public InvalidCraneIdException() : base("Invalid crane ID")
        {
        }
    }
}
