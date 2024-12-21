using ProvidersMS.Core.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.Exceptions
{
    public class InvalidDocumentIdException : DomainException
    {
        public InvalidDocumentIdException() : base("Invalid document ID")
        {
        }
    }
}
