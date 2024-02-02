using Catalog.API.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Catalog.API.Configuration;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{
    private readonly IOptions<MongoConfig> _mongoConfig;
    public CatalogContext(IOptions<MongoConfig> mongoConfig)
    {
        _mongoConfig = mongoConfig;

        var client = new MongoClient(_mongoConfig.Value.ConnectionString);

        var database = client.GetDatabase(_mongoConfig.Value.DatabaseName);

        Products = database.GetCollection<Product>(_mongoConfig.Value.CollectionString);

        CatalogContextSeed.SeedData(Products);
    }

    public IMongoCollection<Product> Products { get; }
}
