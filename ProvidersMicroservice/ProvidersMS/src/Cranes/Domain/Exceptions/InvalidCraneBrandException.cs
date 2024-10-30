using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneBrandException : DomainException
    {
        public InvalidCraneBrandException() : base("Invalid crane brand") 
        { 
        }
    }
}
