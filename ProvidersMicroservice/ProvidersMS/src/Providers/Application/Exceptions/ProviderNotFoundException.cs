namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class ProviderNotFoundException : ApplicationException
    {
        public ProviderNotFoundException() : base("Provider not found") { }
    }
}
