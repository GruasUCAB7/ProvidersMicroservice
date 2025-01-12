namespace ProvidersMS.src.Drivers.Application.Exceptions
{
    public class CraneAssociatedWithDriverException : Exception
    {
        public CraneAssociatedWithDriverException()
            : base("This crane is already assigned to this driver.")
        {
        }
    }
}
