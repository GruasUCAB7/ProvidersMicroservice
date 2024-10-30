using ProvidersMS.src.Cranes.Domain.Exceptions;

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
