namespace ProvidersMS.src.Providers.Application.Exceptions
{
    public class DriversNotFoundException : ApplicationException
    {
        public DriversNotFoundException()
            : base("Drivers not found.")
        {
        }
    }
}
