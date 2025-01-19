namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class ProviderAlreadyExistException : ApplicationException
    {
        public ProviderAlreadyExistException(string rif)
            : base($"Provider with plate {rif} already exist")
        {
        }
    }
}
