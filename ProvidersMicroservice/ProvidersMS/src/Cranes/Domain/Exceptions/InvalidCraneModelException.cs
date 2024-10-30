using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneModelException : DomainException
    {
        public InvalidCraneModelException() : base("Invalid crane model") 
        { }
    }
}
