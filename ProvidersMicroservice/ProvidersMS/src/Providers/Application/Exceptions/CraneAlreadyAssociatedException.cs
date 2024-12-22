namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class CraneAlreadyAssociatedException : ApplicationException
    {
        public CraneAlreadyAssociatedException(string craneId) : base($"Crane with id: {craneId} already associated with another Provider.")
        {
        }
    }
}
