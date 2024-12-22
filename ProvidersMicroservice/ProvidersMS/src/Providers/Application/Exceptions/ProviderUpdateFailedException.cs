namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class ProviderUpdateFailedException : ApplicationException
    {
        public ProviderUpdateFailedException() : base("The provider could not be updated correctly") { }
    }
}
