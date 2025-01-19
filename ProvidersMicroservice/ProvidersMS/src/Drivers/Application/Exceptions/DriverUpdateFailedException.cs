namespace ProvidersMS.src.Drivers.Application.Exceptions
{
    public class DriverUpdateFailedException : ApplicationException
    {
        public DriverUpdateFailedException() : base("The driver could not be updated correctly") { }
    }
}
