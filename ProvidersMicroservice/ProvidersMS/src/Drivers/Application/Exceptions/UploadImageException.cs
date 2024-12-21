namespace ProvidersMS.src.Images.Application.Exceptions
{
    public class UploadImageException : Exception
    {
        public UploadImageException()
            : base("There was an error saving the image.")
        {
        }
    }
}
