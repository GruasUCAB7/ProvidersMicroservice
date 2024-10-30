using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneException : DomainException
    {
        public InvalidCraneException() : base("Invalid crane")
        {
        }
    }
}
