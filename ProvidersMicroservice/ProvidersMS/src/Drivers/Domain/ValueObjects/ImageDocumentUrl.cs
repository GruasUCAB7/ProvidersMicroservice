using ProvidersMS.Core.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class ImageDocumentUrl(string url) : IValueObject<ImageDocumentUrl>
    {
        private readonly string Url = url;

        public string GetValue()
        {
            return Url;
        }

        public bool Equals(ImageDocumentUrl other)
        {
            return Url == other.Url;
        }
    }
}
