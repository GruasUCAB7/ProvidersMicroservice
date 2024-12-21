namespace ProvidersMS.src.Drivers.Application.Exceptions
{
    public class UnsavedImagesException : Exception
    {
        public UnsavedImagesException()
            : base("There was an error saving the images urls.")
        {
        }
    }
}
