using AutoFixture;
using Catalog.API.Configuration;
using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Catalog.API.Tests;

public class CatalogTests
{
    private readonly ProductRepository _productRepository;
    private readonly Mock<CatalogContext> _catalogContextMock;
    private readonly IOptions<MongoConfig> _mongoConfig;
    private readonly Mock<IMongoCollection<Product>> _productsCollection;
    private readonly Product expectedResult;
    private readonly Fixture _fixture;
    public CatalogTests()
    {
        _mongoConfig = Options.Create(
            new MongoConfig
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "ProductDb",
                CollectionString = "Products"
            });

        _productsCollection = new Mock<IMongoCollection<Product>>();

        _productsCollection.Object.InsertManyAsync(CatalogContextSeed.GetPreconfiguredProducts());

        _catalogContextMock = new Mock<CatalogContext>(_mongoConfig);

        _productRepository = new ProductRepository(_catalogContextMock.Object);

        expectedResult = _catalogContextMock.Object.Products.Find(p => p.Id == "602d2149e773f2a3990b47f8")
            .FirstOrDefaultAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        _fixture = new Fixture();
    }

    [Fact]
    public async void GetProductsByCategoryTest()
    {
        var result = await _productRepository.GetProductByCategory("Home Kitchen");

        result.FirstOrDefault(x => x.Category == "Home Kitchen").Should().NotBeNull();
    }

    [Fact]
    public async void GetProductByIdTest()
    {
        var id = "602d2149e773f2a3990b47f8";

        var expectedResult = await _catalogContextMock.Object.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

        var result = await _productRepository.GetProduct(id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void GetProductByNameTest()
    {
        var result = await _productRepository.GetProductByName("Xiaomi Mi 9");

        var product = result.FirstOrDefault(x => x.Name == expectedResult.Name);

        product.Should().NotBeNull();
        product.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void CreateProductTest()
    {
        var product = _fixture.Build<Product>()
            .With(x => x.Id, ObjectId.GenerateNewId().ToString()).Create();

        await _productRepository.CreateProduct(product);

        var result = await _productRepository.GetProduct(product.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async void UpdateProductTest()
    {
        var product = _fixture.Build<Product>()
            .With(x => x.Id, ObjectId.GenerateNewId().ToString()).Create();

        await _productRepository.CreateProduct(product);

        product.Name = "Test";

        var updated = await _productRepository.UpdateProduct(product);

        var result = await _productRepository.GetProduct(product.Id);

        updated.Should().BeTrue();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(product);
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async void DeleteProductTest()
    {
        var product = _fixture.Build<Product>()
            .With(x => x.Id, ObjectId.GenerateNewId().ToString()).Create();

        await _productRepository.CreateProduct(product);

        var deleted = await _productRepository.DeleteProduct(product.Id);

        var deletedProduct = await _productRepository.GetProduct(product.Id);

        deleted.Should().BeTrue();
        deletedProduct.Should().BeNull();
    }
}