using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CraneBrand : IValueObject<CraneBrand>
    {
        private string Brand { get; }
        public CraneBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand) || brand.Length < 2)
            {
                throw new InvalidCraneBrandException();
            }
            Brand = brand;
        }

        public string GetValue()
        {
            return Brand;
        }

        public bool Equals(CraneBrand other)
        {
            return Brand == other.Brand;
        }
    }
}
