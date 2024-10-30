using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneSizeTypeException : DomainException
    {
        public InvalidCraneSizeTypeException() : base("Invalid crane size type") 
        { }
    }
}
