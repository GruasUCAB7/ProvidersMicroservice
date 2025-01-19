namespace ProvidersMS.src.Cranes.Application.Exceptions
{
    public class CraneNotAvailableException : ApplicationException
    {
        public CraneNotAvailableException(): base("Crane not available") { }
    }
}
