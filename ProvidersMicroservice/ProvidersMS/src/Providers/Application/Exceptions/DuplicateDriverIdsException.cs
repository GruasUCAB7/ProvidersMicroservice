namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class DuplicateDriverIdsException : ApplicationException
    {
        public DuplicateDriverIdsException(): base("Duplicate driver IDs found.") { }
    }
}
