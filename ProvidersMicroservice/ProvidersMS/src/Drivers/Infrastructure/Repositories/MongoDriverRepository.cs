using MongoDB.Bson;
using MongoDB.Driver;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
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
                    new CraneId(d.GetValue("craneAssigned").AsString),
                    new DriverLocation(
                        d.GetValue("driverLocation").AsBsonDocument.GetValue("latitude").AsDouble,
                        d.GetValue("driverLocation").AsBsonDocument.GetValue("longitude").AsDouble
                    )
                );
                driver.SetIsAvailable(d.GetValue("isAvailable").AsBoolean);
                driver.SetIsActive(d.GetValue("isActive").AsBoolean);


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
                new CraneId(driverDocument.GetValue("craneAssigned").AsString),
                new DriverLocation(
                        driverDocument.GetValue("driverLocation").AsBsonDocument.GetValue("latitude").AsDouble,
                        driverDocument.GetValue("driverLocation").AsBsonDocument.GetValue("longitude").AsDouble
                    )
            );

            driver.SetIsAvailable(driverDocument.GetValue("isAvailable").AsBoolean);
            driver.SetIsActive(driverDocument.GetValue("isActive").AsBoolean);

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
                DriverLocation = new MongoCoordinates
                {
                    Latitude = driver.GetDriverLocationLatitude(),
                    Longitude = driver.GetDriverLocationLongitude()
                },
                IsActive = driver.GetIsActive(),
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
                {"driverLocation", new BsonDocument
                    {
                        {"latitude", mongoDriver.DriverLocation.Latitude},
                        {"longitude", mongoDriver.DriverLocation.Longitude}
                    }
                },
                {"isActive", mongoDriver.IsActive},
                {"createdDate", mongoDriver.CreatedDate},
                {"updatedDate", mongoDriver.UpdatedDate}
            };

            await _driverCollection.InsertOneAsync(bsonDocument);

            var savedDriver = Driver.CreateDriver(
                new DriverId(mongoDriver.Id),
                new DriverDNI(mongoDriver.DNI),
                new DriverIsActiveLicensed(mongoDriver.IsActiveLicensed),
                new List<string>(),
                new CraneId(mongoDriver.CraneAssigned),
                new DriverLocation(mongoDriver.DriverLocation.Latitude, mongoDriver.DriverLocation.Longitude)
            );
            return Result<Driver>.Success(savedDriver);
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

            if (driver.GetIsActive() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("isActive", driver.GetIsActive()));
            }

            if (driver.GetDriverLocationLatitude() != 0 || driver.GetDriverLocationLongitude() != 0)
            {
                var driverLocation = new BsonDocument
                {
                    { "latitude", driver.GetDriverLocationLatitude() },
                    { "longitude", driver.GetDriverLocationLongitude() }
                };
                updateDefinitions.Add(updateDefinitionBuilder.Set("driverLocation", driverLocation));
            }

            var imagesDocuments = driver.GetImagesDocuments();
            if (imagesDocuments != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("imageDocuments", new BsonArray(imagesDocuments)));
            }

            updateDefinitions.Add(updateDefinitionBuilder.Set("updatedDate", DateTime.UtcNow));

            var update = updateDefinitionBuilder.Combine(updateDefinitions);
            var updateResult = await _driverCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<Driver>.Failure(new Exception("Failed to update driver"));
            }

            return Result<Driver>.Success(driver);
        }

        public async Task UpdateDriverLocation(string driverId, double latitude, double longitude)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", driverId);
            var update = Builders<BsonDocument>.Update
                .Set("driverLocation.latitude", latitude)
                .Set("driverLocation.longitude", longitude)
                .Set("updatedDate", DateTime.UtcNow);

            await _driverCollection.UpdateOneAsync(filter, update);
        }

        public async Task ValidateUpdateTimeDriver()
        {
            var drivers = await _driverCollection.Find(Builders<BsonDocument>.Filter.Empty).ToListAsync();

            foreach (var driverDocument in drivers)
            {
                var updatedDate = driverDocument.GetValue("updatedDate").ToUniversalTime();
                var timeDifference = DateTime.UtcNow - updatedDate;

                if (timeDifference.TotalMinutes > 10)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", driverDocument.GetValue("_id").AsString);
                    var update = Builders<BsonDocument>.Update.Set("isActive", false);
                    await _driverCollection.UpdateOneAsync(filter, update);
                }
            }
        }

        public async Task<bool> IsCraneAssociatedWithAnotherDriver(string craneId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("craneAssigned", craneId);
            var driver = await _driverCollection.Find(filter).FirstOrDefaultAsync();
            return driver != null;
        }
    }
}