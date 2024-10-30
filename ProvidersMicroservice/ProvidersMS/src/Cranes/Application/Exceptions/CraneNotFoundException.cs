namespace ProvidersMS.src.Cranes.Application.Exceptions
{
    public class CraneNotFoundException : ApplicationException
    {
        public CraneNotFoundException(): base("Crane not found") { }
    }
}
