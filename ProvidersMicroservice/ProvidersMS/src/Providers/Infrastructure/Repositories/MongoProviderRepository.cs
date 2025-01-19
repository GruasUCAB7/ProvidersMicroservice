using MongoDB.Bson;
using MongoDB.Driver;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Utils.Result;
using ProvidersMS.src.Cranes.Domain.ValueObjects;
using ProvidersMS.src.Drivers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Application.Exceptions;
using ProvidersMS.src.Providers.Application.Models;
using ProvidersMS.src.Providers.Application.Queries.GetAll.Types;
using ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Domain;
using ProvidersMS.src.Providers.Domain.ValueObjects;
using ProvidersMS.src.Providers.Infrastructure.Models;

namespace ProvidersMS.src.Providers.Infrastructure.Repositories
{
    public class MongoProviderRepository(MongoDbService mongoDbService) : IProviderRepository
    {
        private readonly IMongoCollection<BsonDocument> _providerCollection = mongoDbService.GetProviderCollection();
        private readonly IMongoCollection<BsonDocument> _driverCollection = mongoDbService.GetDriverCollection();

        public async Task<bool> ExistByRif(string rif)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("rif", rif);
            var provider = await _providerCollection.Find(filter).FirstOrDefaultAsync();
            return provider != null;
        }

        public async Task<List<Provider>> GetAll(GetAllProvidersQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = data.IsActive?.ToLower() switch
            {
                "active" => filterBuilder.Eq("isActive", true),
                "inactive" => filterBuilder.Eq("isActive", false),
                _ => filterBuilder.Empty
            };

            var providerEntities = await _providerCollection
                .Find(filter)
                .Skip(data.PerPage * (data.Page - 1))
                .Limit(data.PerPage)
                .ToListAsync();

            var providers = providerEntities.Select(p =>
            {
                var provider = Provider.CreateProvider(
                    new ProviderId(p.GetValue("_id").AsString),
                    new ProviderRif(p.GetValue("rif").AsString),
                    new ProviderType(p.GetValue("providerType").AsString),
                    p.GetValue("fleetOfCranes").AsBsonArray.Select(c => new CraneId(c.AsString)).ToList(),
                    p.GetValue("drivers").AsBsonArray.Select(d => new DriverId(d.AsString)).ToList()
                );

                provider.SetIsActive(p.GetValue("isActive").AsBoolean);
                return provider;
            }).ToList();

            return providers;
        }

        public async Task<Core.Utils.Optional.Optional<Provider>> GetById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var providerDocument = await _providerCollection.Find(filter).FirstOrDefaultAsync();

            if (providerDocument is null)
            {
                return Core.Utils.Optional.Optional<Provider>.Empty();
            }

            var fleetOfCranes = providerDocument.GetValue("fleetOfCranes").AsBsonArray
                .Select(c => new CraneId(c.AsString)).ToList();

            var drivers = providerDocument.GetValue("drivers").AsBsonArray
                .Select(d => new DriverId(d.AsString)).ToList();

            var provider = Provider.CreateProvider(
                new ProviderId(providerDocument.GetValue("_id").AsString),
                new ProviderRif(providerDocument.GetValue("rif").AsString),
                new ProviderType(providerDocument.GetValue("providerType").AsString),
                fleetOfCranes,
                drivers
            );
            
            provider.SetIsActive(providerDocument.GetValue("isActive").AsBoolean);

            return Core.Utils.Optional.Optional<Provider>.Of(provider);
        }

        public async Task<Result<Provider>> Save(Provider provider)
        {
            var mongoProvider = new MongoProvider
            {
                Id = provider.GetId(),
                Rif = provider.GetRif(),
                ProviderType = provider.GetProviderType(),
                FleetOfCranes = new List<string>(),
                Drivers = new List<string>(),
                IsActive = provider.GetIsActive(),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var bsonDocument = new BsonDocument
            {
                {"_id", mongoProvider.Id},
                {"rif", mongoProvider.Rif},
                {"providerType", mongoProvider.ProviderType},
                {"fleetOfCranes", new BsonArray()},
                {"drivers", new BsonArray()},
                {"isActive", mongoProvider.IsActive},
                {"createdDate", mongoProvider.CreatedDate},
                {"updatedDate", mongoProvider.UpdatedDate}
            };

            await _providerCollection.InsertOneAsync(bsonDocument);

            var savedProvider = Provider.CreateProvider(
                new ProviderId(mongoProvider.Id),
                new ProviderRif(mongoProvider.Rif),
                new ProviderType(mongoProvider.ProviderType),
                new List<CraneId>(),
                new List<DriverId>()
            );
            return Result<Provider>.Success(savedProvider);
        }

        public async Task<Result<Provider>> Update(Provider provider)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", provider.GetId());

            var existingProviderDocument = await _providerCollection.Find(filter).FirstOrDefaultAsync();
            if (existingProviderDocument == null)
            {
                return Result<Provider>.Failure(new Exception("Provider not found"));
            }

            var updateDefinitionBuilder = Builders<BsonDocument>.Update;
            var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();

            if (provider.GetIsActive() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("isActive", provider.GetIsActive()));
            } 

            if (provider.GetFleetOfCranes() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("fleetOfCranes", new BsonArray(provider.GetFleetOfCranes().Select(c => c.ToString()))));
            }

            if (provider.GetDrivers() != null)
            {
                updateDefinitions.Add(updateDefinitionBuilder.Set("drivers", new BsonArray(provider.GetDrivers().Select(d => d.ToString()))));
            }

            updateDefinitions.Add(updateDefinitionBuilder.Set("updatedDate", DateTime.UtcNow));

            var update = updateDefinitionBuilder.Combine(updateDefinitions);

            var updateResult = await _providerCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return Result<Provider>.Failure(new ProviderUpdateFailedException());
            }

            return Result<Provider>.Success(provider);
        }

        public async Task<bool> IsCraneAssociatedWithAnotherProvider(string craneId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("fleetOfCranes", craneId);
            var provider = await _providerCollection.Find(filter).FirstOrDefaultAsync();
            return provider != null;
        }

        public async Task<bool> IsDriverAssociatedWithAnotherProvider(string driverId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("drivers", driverId);
            var provider = await _providerCollection.Find(filter).FirstOrDefaultAsync();
            return provider != null;
        }

        public async Task<List<Driver>> GetAvailableDrivers(GetAvailableDriversQuery data)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("isAvailable", true),
                filterBuilder.Eq("isActiveLicensed", true)
            );

            var driverEntities = await _driverCollection
                .Find(filter)
                .Skip(data.PerPage * (data.Page - 1))
                .Limit(data.PerPage)
                .ToListAsync();

            var drivers = new List<Driver>();
            foreach (var d in driverEntities)
            {
                try
                {
                    var driver = new Driver(
                        d.GetValue("_id").AsString, 
                        d.GetValue("craneAssigned").AsString
                    );
                    drivers.Add(driver);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Error parsing driver document: {0}", ex.Message);
                }
            }

            var internalProviderDrivers = new List<Driver>();
            var externalProviderDrivers = new List<Driver>();

            foreach (var driver in drivers)
            {
                var providerFilter = Builders<BsonDocument>.Filter.Eq("fleetOfCranes", driver.CraneAssigned);
                var providerDocument = await _providerCollection.Find(providerFilter).FirstOrDefaultAsync();

                if (providerDocument != null)
                {
                    var providerType = new ProviderType(providerDocument.GetValue("providerType").AsString);
                    if (providerType.Type == "Interno")
                    {
                        internalProviderDrivers.Add(driver);
                    }
                    else
                    {
                        externalProviderDrivers.Add(driver);
                    }
                }
            }

            var sortedDrivers = internalProviderDrivers.Concat(externalProviderDrivers).ToList();

            return sortedDrivers;
        }
    }
}
