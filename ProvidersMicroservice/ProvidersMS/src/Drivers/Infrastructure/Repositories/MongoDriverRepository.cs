using MongoDB.Bson;
using MongoDB.Driver;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Application.Exceptions;
using ProvidersMS.src.Drivers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Domain;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Infrastructure.Models;

namespace ProvidersMS.src.Drivers.Infrastructure.Repositories
{
    public class MongoDriverRepository(MongoDbService mongoDbService) : IDriverRepository
    {
        private readonly IMongoCollection<BsonDocument> _driverCollection = mongoDbService.GetDriverCollection();
        public async Task<bool> ExistByDNI(string dni)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("dni", dni);
            var driver = await _driverCollection.Find(filter).FirstOrDefaultAsync();
            return driver != null;
        }

        public async Task<List<Driver>> GetAll(GetAllDriversQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(data.IsActiveLicensed))
            {
                filter &= data.IsActiveLicensed.ToLower() switch
                {
                    "activo" => filterBuilder.Eq("isActiveLicensed", true),
                    "inactivo" => filterBuilder.Eq("isActiveLicensed", false),
                    _ => filter
                };
            }

            if (!string.IsNullOrEmpty(data.IsAvailable))
            {
                filter &= data.IsAvailable.ToLower() switch
                {
                    "disponible" => filterBuilder.Eq("isAvailable", true),
                    "no disponible" => filterBuilder.Eq("isAvailable", false),
                    _ => filter
                };
            }

            var driverEntities = await _driverCollection
                .Find(filter)
                .Skip(data.PerPage * (data.Page - 1))
                .Limit(data.PerPage)
                .ToListAsync();

            var drivers = driverEntities.Select(d =>
            {
                var driver = Driver.CreateDriver(
                    new DriverId(d.GetValue("_id").AsString),
                    new DriverDNI(d.GetValue("dni").AsString),
                    new DriverIsActiveLicensed(d.GetValue("isActiveLicensed").AsBoolean),
                    d.GetValue("imageDocuments").AsBsonArray.Select(i => i.AsString).ToList(),
                    new CraneId(d.GetValue("craneAssigned").AsString)
                );

                driver.SetIsAvailable(d.GetValue("isAvailable").AsBoolean);
                return driver;

            }).ToList();

            return drivers;
        }

        public async Task<Core.Utils.Optional.Optional<Driver>> GetById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var driverDocument = await _driverCollection.Find(filter).FirstOrDefaultAsync();
            if (driverDocument is null)
            {
                return Core.Utils.Optional.Optional<Driver>.Empty();
            }

            var driver = Driver.CreateDriver(
                new DriverId(driverDocument.GetValue("_id").AsString),
                new DriverDNI(driverDocument.GetValue("dni").AsString),
                new DriverIsActiveLicensed(driverDocument.GetValue("isActiveLicensed").AsBoolean),
                driverDocument.GetValue("imageDocuments").AsBsonArray.Select(i => i.AsString).ToList(),
                new CraneId(driverDocument.GetValue("craneAssigned").AsString)
            );

            driver.SetIsAvailable(driverDocument.GetValue("isAvailable").AsBoolean);
            return Core.Utils.Optional.Optional<Driver>.Of(driver);
        }

        public async Task<Result<Driver>> Save(Driver driver)
        {
            var mongoDriver = new MongoDriver
            {
                Id = driver.GetId(),
                DNI = driver.GetDNI(),
                IsActiveLicensed = driver.GetIsActiveLicensed(),
                ImageDocuments = driver.GetImagesDocuments(),
                CraneAssigned = driver.GetCraneAssigned(),
                IsAvailable = driver.GetIsAvailable(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var bsonDocument = new BsonDocument
            {
                {"_id", mongoDriver.Id},
                {"dni", mongoDriver.DNI},
                {"isActiveLicensed", mongoDriver.IsActiveLicensed},
                {"imageDocuments", new BsonArray(mongoDriver.ImageDocuments)},
                {"craneAssigned", mongoDriver.CraneAssigned},
                {"isAvailable", mongoDriver.IsAvailable},
                {"createdDate", mongoDriver.CreatedDate},
                {"updatedDate", mongoDriver.UpdatedDate}
            };

            await _driverCollection.InsertOneAsync(bsonDocument);

            var savedDriver = Driver.CreateDriver(
                new DriverId(mongoDriver.Id),
                new DriverDNI(mongoDriver.DNI),
                new DriverIsActiveLicensed(mongoDriver.IsActiveLicensed),
                new List<string>(),
                new CraneId(mongoDriver.CraneAssigned)
            );
            return Result<Driver>.Success(savedDriver);
        }

        public async Task<Result<object>> UpdateDriverImages(Driver driver)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", driver.GetId());
                var existingDriver = await _driverCollection.FindAsync(filter);
                var result = await existingDriver.FirstOrDefaultAsync();

                if (result == null)
                {
                    return Result<object>.Failure(new DriverNotFoundException());
                }

                var imagesDocuments = driver.GetImagesDocuments();
                var update = Builders<BsonDocument>.Update.Set("imageDocuments", new BsonArray(imagesDocuments));
                await _driverCollection.UpdateOneAsync(filter, update);

                return Result<object>.Success(imagesDocuments);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(ex);
            }
        }

        public async Task<Result<Driver>> Update(Driver driver)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", driver.GetId());
            var updateDefinitionBuilder = Builders<BsonDocument>.Update;
            var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();

            if (driver.GetIsActiveLicensed() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("isActiveLicensed", driver.GetIsActiveLicensed()));
            }   

            if (driver.GetCraneAssigned() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("craneAssigned", driver.GetCraneAssigned()));
            }

            if (driver.GetIsAvailable() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("isAvailable", driver.GetIsAvailable()));
            }

            var update = updateDefinitionBuilder.Combine(updateDefinitions);
            var updateResult = await _driverCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<Driver>.Failure(new Exception("Failed to update driver"));
            }

            return Result<Driver>.Success(driver);
        }
    }
}
