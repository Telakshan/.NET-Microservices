namespace Catalog.API.Configuration;

public class MongoConfig
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionString { get; set; } = null!;
}
