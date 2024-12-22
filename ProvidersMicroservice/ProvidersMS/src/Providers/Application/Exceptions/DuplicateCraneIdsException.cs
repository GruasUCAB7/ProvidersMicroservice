namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class DuplicateCraneIdsException : ApplicationException
    {
        public DuplicateCraneIdsException(): base("Duplicate crane IDs found") { }
    }
}
