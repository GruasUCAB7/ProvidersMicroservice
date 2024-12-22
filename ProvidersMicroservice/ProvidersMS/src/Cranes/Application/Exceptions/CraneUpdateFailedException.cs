namespace ProvidersMS.src.Cranes.Application.Exceptions
{
    public class CraneUpdateFailedException : ApplicationException
    {
        public CraneUpdateFailedException(): base("The crane could not be updated correctly") { }
    }
}
