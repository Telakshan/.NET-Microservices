using Catalog.API.Configuration;
using Catalog.API.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Tests;

public class CatalogContextTests
{
    private readonly IOptions<MongoConfig> _mongoConfig;
    DatabaseNamespace databaseNamespace = new DatabaseNamespace("ProductDb");
    public CatalogContextTests()
    {
        _mongoConfig = Options.Create(
            new MongoConfig {
                ConnectionString = "mongodb://localhost:27017", 
                DatabaseName = databaseNamespace.ToString(), 
                CollectionName = "Products"
            });
    }

    [Fact]
    public void CreateCatalogContextTest()
    {
        var result = new CatalogContext(_mongoConfig);

        result.Should().NotBeNull().And.BeAssignableTo<ICatalogContext>();
        result.Products.Database.Should().NotBeNull();
        result.Products.Database.DatabaseNamespace.Should().Be(databaseNamespace);

        //GetPreconfiguredProducts method add 6 documents
        result.Products.CountDocuments(p => true).Should().Be(6L);
    }
}
