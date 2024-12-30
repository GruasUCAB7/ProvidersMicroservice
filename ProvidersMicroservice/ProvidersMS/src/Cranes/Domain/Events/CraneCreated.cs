using ProvidersMS.Core.Domain.Events;
using ProvidersMS.src.Cranes.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Domain.Events
{

    public class CraneCreatedEvent(string dispatcherId, string name, CraneCreated context) : DomainEvent<object>(dispatcherId, name, context) { }

    public class CraneCreated(string id, string brand, string model, string plate, string craneSize, int year)
    {
        public readonly string Id = id;
        public readonly string Brand = brand;
        public readonly string Model = model;
        public readonly string Plate = plate;
        public readonly string CraneSize = craneSize;
        public readonly int Year = year;

        static public CraneCreatedEvent CreateEvent(CraneId craneId, CraneBrand craneBrand, CraneModel craneModel, CranePlate cranePlate, CraneSize craneSize, CraneYear craneYear)
        {
            return new CraneCreatedEvent(
                craneId.GetValue(),
                typeof(CraneCreated).Name,
                new CraneCreated(
                    craneId.GetValue(),
                    craneBrand.GetValue(),
                    craneModel.GetValue(),
                    cranePlate.GetValue(),
                    craneSize.GetValue(),
                    craneYear.GetValue()
                )
            );
        }
    }
}
