using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProvidersMS.src.Providers.Infrastructure.Models
{
    public class MongoProvider
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("rif")]
        public required string Rif { get; set; }

        [BsonElement("providerType")]
        public required string ProviderType { get; set; }

        [BsonElement("fleetOfCranes")]
        public required List<string> FleetOfCranes { get; set; }

        [BsonElement("drivers")]
        public required List<string> Drivers { get; set; }

        [BsonElement("isActive")]
        public required bool IsActive { get; set; }

        [BsonElement("createdDate")]
        public required DateTime CreatedDate { get; set; }

        [BsonElement("updatedDate")]
        public required DateTime UpdatedDate { get; set; }
    }
}
