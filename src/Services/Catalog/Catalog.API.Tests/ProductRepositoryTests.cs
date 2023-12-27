using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Moq;
using Moq.EntityFrameworkCore;

namespace Catalog.API.Tests;

public class ProductRepositoryTests
{
    private readonly IProductRepository _productRepository;
    private readonly Mock<ICatalogContext> _catalogContextMock;
    private readonly Mock<IMongoCollection<Product>> _collectionMock;
    private readonly Mock<IMongoDatabase> _dbMock;
    public ProductRepositoryTests()
    {
        _catalogContextMock = new Mock<ICatalogContext>();
        _collectionMock = new Mock<IMongoCollection<Product>>();
        _dbMock = new Mock<IMongoDatabase>();
    }

    [Fact]
    public void GetProducts_WhenCalled()
    {
        /*_catalogContextMock.Setup<DbSet<Product>>(x => x.Products)
            .ReturnsDbSet<Product>(TestDataHelper.GetTestProducts());*/
    }
}