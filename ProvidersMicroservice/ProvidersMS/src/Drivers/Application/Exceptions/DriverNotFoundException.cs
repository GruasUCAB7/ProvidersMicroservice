namespace ProvidersMS.src.Drivers.Application.Exceptions
{
    public class DriverNotFoundException : Exception
    {
        public DriverNotFoundException()
            : base("Driver not found.")
        {
        }
    }
}
