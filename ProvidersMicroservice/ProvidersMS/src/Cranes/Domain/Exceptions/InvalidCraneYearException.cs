using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.Exceptions
{
    public class InvalidCraneYearException : DomainException
    {
        public InvalidCraneYearException() : base("Invalid crane year. The year should be from 1995 onwards") 
        { 
        }
    }
}
