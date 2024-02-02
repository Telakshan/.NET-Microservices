namespace Ordering.API.Configuration;

public class AppSettings
{
    public ConnectionString ConnectionString { get; set; } = null!;
}

public class ConnectionString
{
    public string OrderingConnectionString { get; set; } = null!;
}

public class EmailSettings
{
    public string ApiKey { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string FromName { get; set; } = null!;
}
