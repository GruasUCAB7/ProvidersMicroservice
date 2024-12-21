using ProvidersMS.Core.Domain.Entities;
using ProvidersMS.src.Drivers.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Domain.Entities.ImagesDocuments
{
    public class ImageDocument(ImageDocumentId id, ImageDocumentUrl url) : Entity<ImageDocumentId>(id)
    {
        public ImageDocumentId _id = id;
        public ImageDocumentUrl _imageUrl = url;

        public string GetId() => _id.GetValue();
        public string GetUrl() => _imageUrl.GetValue();

        public static ImageDocument CreateImageDocument(string id, string url)
        {
            return new ImageDocument(new ImageDocumentId(id), new ImageDocumentUrl(url));
        }
    }
}
