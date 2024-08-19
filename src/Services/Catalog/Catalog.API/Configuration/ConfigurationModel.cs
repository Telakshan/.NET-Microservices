using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Configuration;

public class MongoConfig
{
    [Required]
    public string ConnectionString { get; init; } = null!;
    [Required]
    public string DatabaseName { get; init; } = null!;
    [Required]
    public string CollectionName { get; init; } = null!;
}
