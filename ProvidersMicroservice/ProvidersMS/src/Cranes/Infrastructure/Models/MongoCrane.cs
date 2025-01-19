using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProvidersMS.src.Cranes.Infrastructure.Models
{
    public class MongoCrane
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        
        [BsonElement("brand")]
        public required string Brand { get; set; }
        
        [BsonElement("model")]
        public required string Model { get; set; }
        
        [BsonElement("plate")]
        public required string Plate { get; set; }
        
        [BsonElement("craneSize")]
        public required string CraneSize { get; set; }
        
        [BsonElement("year")]
        public required int Year { get; set; }

        [BsonElement("isActive")]
        public required bool IsActive { get; set; }

        [BsonElement("createdDate")]
        public required DateTime CreatedDate { get; set; }

        [BsonElement("updatedDate")]
        public required DateTime UpdatedDate { get; set; }
    }
}
