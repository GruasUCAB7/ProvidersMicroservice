using ProvidersMS.Core.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Domain.Exceptions;

namespace ProvidersMS.src.Cranes.Domain.ValueObjects
{
    public class CraneModel : IValueObject<CraneModel>
    {
        private string Model { get; }
        public CraneModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model) || model.Length < 2)
            {
                throw new InvalidCraneModelException();
            }
            Model = model;
        }

        public string GetValue()
        {
            return Model;
        }

        public bool Equals(CraneModel other)
        {
            return Model == other.Model;
        }
    }
}
