using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProvidersMS.src.Drivers.Infrastructure.Models
{
    public class MongoImage
    {
        [BsonId]
        [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("src")]
        public required string Src { get; set; }

        [BsonElement("createdDate")]
        public required DateTime CreatedDate { get; set; }
    }
}
