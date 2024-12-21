using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Drivers.Domain.Exceptions;

namespace ProvidersMS.src.Drivers.Domain.ValueObjects
{
    public class ImageDocumentId : IValueObject<ImageDocumentId>
    {
        private string Id { get; }

        public ImageDocumentId(string id)
        {
            if (!UUIDRegExps.UUIDRegExp.IsMatch(id))
            {
                throw new InvalidDocumentIdException();
            }
            Id = id;
        }

        public string GetValue()
        {
            return Id;
        }

        public bool Equals(ImageDocumentId other)
        {
            return Id == other.Id;
        }
    }
}
