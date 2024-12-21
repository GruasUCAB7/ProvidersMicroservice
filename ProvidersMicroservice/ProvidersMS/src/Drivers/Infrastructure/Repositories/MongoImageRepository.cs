using ProvidersMS.Core.Utils.Result;
using ProvidersMS.Core.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Infrastructure.Models;
using ProvidersMS.src.Drivers.Domain.Entities.ImagesDocuments;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Images.Application.Exceptions;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain;

namespace ProvidersMS.src.Drivers.Infrastructure.Repositories
{
    public class MongoImageDocumentRepository(MongoDbService mongoDbService) : IImageDocumentRepository
    {
        private readonly IMongoCollection<BsonDocument> _imageCollection = mongoDbService.GetImageCollection();

        public async Task<Result<List<ImageDocument>>> Save(ImageDocument imageDocument)
        {
            var mongoImage = new MongoImage
            {
                Id = imageDocument.GetId(),
                Src = imageDocument.GetUrl(),
                CreatedDate = DateTime.UtcNow
            };

            var bsonDocument = new BsonDocument
                {
                    { "_id", imageDocument.GetId() },
                    { "src", imageDocument.GetUrl() },
                    { "createdDate", mongoImage.CreatedDate}
                };

            await _imageCollection.InsertOneAsync(bsonDocument);

            return Result<List<ImageDocument>>.Success(new List<ImageDocument> { imageDocument });
        }

        public async Task<Core.Utils.Optional.Optional<ImageDocument>> GetByUrl(string url)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("src", url);
            var imageBson = await _imageCollection.Find(filter).FirstOrDefaultAsync();
            if (imageBson is null)
            {
                return Core.Utils.Optional.Optional<ImageDocument>.Empty();
            }

            var imageDocument = new ImageDocument(
                new ImageDocumentId(imageBson.GetValue("_id").AsString),
                new ImageDocumentUrl(imageBson.GetValue("src").AsString)
            );

            return Core.Utils.Optional.Optional<ImageDocument>.Of(imageDocument);
        }

        public async Task Delete(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await _imageCollection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                throw new UploadImageException();
            }

        }
    }
}
