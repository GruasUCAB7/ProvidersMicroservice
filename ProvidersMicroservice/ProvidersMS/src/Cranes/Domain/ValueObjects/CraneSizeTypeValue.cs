using ProvidersMS.src.Cranes.Domain.Exceptions;
using ProvidersMS.src.Providers.Domain.Exceptions;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public static class CraneSizeTypeValue
    {
        public static string GetValue(this CraneSizeType craneSizeType)
        {
            if (craneSizeType == CraneSizeType.Light || craneSizeType == CraneSizeType.Medium || craneSizeType == CraneSizeType.Heavy)
            {
                return craneSizeType.ToString();
            }
            throw new InvalidCraneSizeTypeException();
        }
    }
}
