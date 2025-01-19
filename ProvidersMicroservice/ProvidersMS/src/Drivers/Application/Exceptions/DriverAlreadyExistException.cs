namespace ProvidersMS.src.Drivers.Application.Exceptions
{
    public class DriverAlreadyExistException : Exception
    {
        public DriverAlreadyExistException(string dni)
            : base($"Driver with dni {dni} already exist")
        {
        }
    }
}
