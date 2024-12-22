namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class CranesNotFoundException : ApplicationException
    {
        public CranesNotFoundException(): base("Cranes not found") { }
    }
}
