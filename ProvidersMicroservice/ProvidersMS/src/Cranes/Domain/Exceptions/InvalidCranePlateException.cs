using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCranePlateException : DomainException
    {
        public InvalidCranePlateException() : base("Invalid crane plate. The plate must be in XX000XX format")
        {
        }
    }
}
