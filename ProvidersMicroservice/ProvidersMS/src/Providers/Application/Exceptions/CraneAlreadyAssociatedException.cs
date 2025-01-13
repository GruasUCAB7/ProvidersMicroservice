namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class CraneAlreadyAssociatedException(string craneId) : ApplicationException($"Crane with id: {craneId} already associated with another Provider.")
    {
    }
}
