using ProvidersMS.Core.Domain.Events;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;

namespace ProvidersMS.src.Drivers.Domain.Events
{

    public class DriverCreatedEvent(string dispatcherId, string name, DriverCreated context) : DomainEvent<object>(dispatcherId, name, context) { }

    public class DriverCreated(string id, string dni, bool isActiveLicensed, List<string> imagesDocuments, string craneAssigned)
    {
        public readonly string Id = id;
        public readonly string DNI = dni;
        public readonly bool IsActiveLicensed = isActiveLicensed;
        public readonly List<string> ImagesDocuments = imagesDocuments;
        public readonly string CraneAssigned = craneAssigned;

        static public DriverCreatedEvent CreateEvent(DriverId Id, DriverDNI DNI, DriverIsActiveLicensed IsActiveLicensed, List<string> ImagesDocuments, CraneId craneAssigned)
        {
            return new DriverCreatedEvent(
                Id.GetValue(),
                typeof(DriverCreated).Name,
                new DriverCreated(
                    Id.GetValue(),
                    DNI.GetValue(),
                    IsActiveLicensed.GetValue(),
                    ImagesDocuments,
                    craneAssigned.GetValue()
                )
            );
        }
    }
}
