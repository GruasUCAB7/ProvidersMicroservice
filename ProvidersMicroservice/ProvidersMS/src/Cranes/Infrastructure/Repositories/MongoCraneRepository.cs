using MongoDB.Bson;
using MongoDB.Driver;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Application.Queries.GetAll.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Domain;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Cranes.Infrastructure.Models;

namespace ProvidersMS.src.Cranes.Infrastructure.Repositories
{
    public class MongoCraneRepository(MongoDbService mongoDbService) : ICraneRepository
    {
        private readonly IMongoCollection<BsonDocument> _craneCollection = mongoDbService.GetCraneCollection();

        public async Task<bool> ExistByPlate(string plate)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("plate", plate);
            var crane = await _craneCollection.Find(filter).FirstOrDefaultAsync();
            return crane != null;
        }

        public async Task<List<Crane>> GetAll(GetAllCranesQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = data.IsActive?.ToLower() switch
            {
                "active" => filterBuilder.Eq("isActive", true),
                "inactive" => filterBuilder.Eq("isActive", false),
                _ => filterBuilder.Empty
            };

            var craneEntities = await _craneCollection
                .Find(filter)
                .Skip(data.PerPage * (data.Page - 1))
                .Limit(data.PerPage)
                .ToListAsync();

            var cranes = craneEntities.Select(e =>
            {
                var crane = Crane.CreateCrane(
                    new CraneId(e.GetValue("_id").AsString),
                    new CraneBrand(e.GetValue("brand").AsString),
                    new CraneModel(e.GetValue("model").AsString),
                    new CranePlate(e.GetValue("plate").AsString),
                    Enum.Parse<CraneSizeType>(e.GetValue("craneType").AsString),
                    new CraneYear(e.GetValue("year").AsInt32)
                );

                crane.SetIsActive(e.GetValue("isActive").AsBoolean);
                return crane;
            }).ToList();

            return cranes;
        }

        public async Task<Core.Utils.Optional.Optional<Crane>> GetById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var craneDocument = await _craneCollection.Find(filter).FirstOrDefaultAsync();

            if (craneDocument is null)
            {
                return Core.Utils.Optional.Optional<Crane>.Empty();
            }
            
            var crane = Crane.CreateCrane(
                new CraneId(craneDocument.GetValue("_id").AsString),
                new CraneBrand(craneDocument.GetValue("brand").AsString),
                new CraneModel(craneDocument.GetValue("model").AsString),
                new CranePlate(craneDocument.GetValue("plate").AsString),
                Enum.Parse<CraneSizeType>(craneDocument.GetValue("craneType").AsString),
                new CraneYear(craneDocument.GetValue("year").AsInt32)
            );

            crane.SetIsActive(craneDocument.GetValue("isActive").AsBoolean);

            return Core.Utils.Optional.Optional<Crane>.Of(crane);
        }

        public async Task<Result<Crane>> Save(Crane crane)
        {
            var mongoCrane = new MongoCrane
            {
                Id = crane.GetId(),
                Brand = crane.GetBrand(),
                Model = crane.GetModel(),
                Plate = crane.GetPlate(),
                CraneType = crane.GetCraneType().ToString(),
                Year = crane.GetYear(),
                IsActive = crane.GetIsActive(),
                CreationDate = crane.GetCreationDate()
            };

            var bsonDocument = new BsonDocument
            {
                {"_id", mongoCrane.Id},
                {"brand", mongoCrane.Brand},
                {"model", mongoCrane.Model},
                {"plate", mongoCrane.Plate},
                {"craneType", mongoCrane.CraneType},
                {"year", mongoCrane.Year},
                {"isActive", mongoCrane.IsActive},
                {"dateCreate", mongoCrane.CreationDate}
            };

            await _craneCollection.InsertOneAsync(bsonDocument);

            var savedCrane = Crane.CreateCrane(
                new CraneId(mongoCrane.Id),
                new CraneBrand(mongoCrane.Brand),
                new CraneModel(mongoCrane.Model),
                new CranePlate(mongoCrane.Plate),
                Enum.Parse<CraneSizeType>(mongoCrane.CraneType),
                new CraneYear(mongoCrane.Year)
            );
            return Result<Crane>.Success(savedCrane);
        }

        public async Task<Result<Crane>> Update(Crane crane)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", crane.GetId());
            var update = Builders<BsonDocument>.Update
                .Set("isActive", crane.GetIsActive());

            var updateResult = await _craneCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<Crane>.Failure(new Exception("Failed to update crane"));
            }

            return Result<Crane>.Success(crane);
        }
    }
}
