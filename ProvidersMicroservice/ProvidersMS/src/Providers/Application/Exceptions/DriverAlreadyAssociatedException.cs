namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class DriverAlreadyAssociatedException(string driverId) : ApplicationException($"Driver with id: {driverId} already associated with another Provider.")
    {
    }
}
