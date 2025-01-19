using ProvidersMS.Core.Domain.Events;
using ProvidersMS.src.Cranes.Domain.ValueObjects;

namespace ProvidersMS.src.Cranes.Domain.Events
{
    public class CraneUpdatedEvent(string dispatcherId, string name, CraneUpdated context) : DomainEvent<object>(dispatcherId, name, context) { }
    
    public class CraneUpdated(string id, bool isActive)
    {
        public readonly string Id = id;
        public readonly bool IsActive = isActive;

        static public CraneUpdatedEvent CreateEvent(CraneId craneId, bool isActive)
        {
            return new CraneUpdatedEvent(
                craneId.GetValue(),
                typeof(CraneUpdated).Name,
                new CraneUpdated(
                    craneId.GetValue(),
                    isActive
                )
            );
        }
    }
}
