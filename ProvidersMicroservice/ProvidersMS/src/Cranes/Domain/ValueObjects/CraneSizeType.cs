using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CraneSize : IValueObject<CraneSize>
    {
        public static readonly string Ligera = "Ligera";
        public static readonly string Mediana = "Mediana";
        public static readonly string Pesada = "Pesada";

        public string Size { get; }

        public CraneSize(string size)
        {
            if (size != Ligera
                && size != Mediana
                && size != Pesada)
            {
                throw new InvalidCraneSizeException($"Invalid crane size: {size}. Allowed values are: {Ligera}, {Mediana}, {Pesada}.");
            }
            Size = size;
        }

        public string GetValue()
        {
            return Size;
        }

        public bool Equals(CraneSize other)
        {
            return Size == other.Size;
        }
    }
}
