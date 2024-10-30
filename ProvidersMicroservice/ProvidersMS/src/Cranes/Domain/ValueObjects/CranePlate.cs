﻿using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.Core.Utils.RegExps;
using ProvidersMS.src.Cranes.Domain.Exceptions;
using ProvidersMS.src.Providers.Domain.Exceptions;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CranePlate : IValueObject<CranePlate>
    {
        private string Plate { get; }

        public CranePlate(string plate) 
        {
            if (!PlateRegex.IsMatch(plate))
            {
                throw new InvalidCranePlateException();
            }
            Plate = plate;
        }

        public string GetValue()
        {
            return Plate;
        }

        public bool Equals(CranePlate other)
        {
            return Plate == other.Plate;
        }

        public override string ToString()
        {
            return Plate;
        }
    }
}
