using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CraneYear : IValueObject<CraneYear>
    {
        private int Year { get; }

        public CraneYear(int year)
        {
            if (year < 1995 || year > DateTime.Now.Year)
            {
                throw new InvalidCraneYearException();
            }
            Year = year;
        }

        public int GetValue()
        {
            return Year;
        }

        public bool Equals(CraneYear other)
        {
            return Year == other.Year;
        }

        public override string ToString()
        {
            return Year.ToString();
        }
    }
}
