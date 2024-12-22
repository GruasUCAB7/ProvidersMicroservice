namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class DriverAlreadyAssociatedException : ApplicationException
    {
        public DriverAlreadyAssociatedException(string driverId) : base($"Driver with id: {driverId} already associated with another Provider.")
        {
        }
    }
}
