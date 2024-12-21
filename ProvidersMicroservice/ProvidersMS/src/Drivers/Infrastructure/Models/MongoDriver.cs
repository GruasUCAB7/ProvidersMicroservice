using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProvidersMS.src.Drivers.Infrastructure.Models
{
    public class MongoDriver
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("dni")]
        public required string DNI { get; set; }

        [BsonElement("isActiveLicensed")]
        public required bool IsActiveLicensed { get; set; }

        [BsonElement("imageDocuments")]
        public required List<string> ImageDocuments { get; set; }

        [BsonElement("craneAssigned")]
        public required string CraneAssigned { get; set; }

        [BsonElement("isAvailable")]
        public required bool IsAvailable { get; set; }

        [BsonElement("createdDate")]
        public required DateTime CreatedDate { get; set; }

        [BsonElement("updatedDate")]
        public required DateTime UpdatedDate { get; set; }
    }
}
