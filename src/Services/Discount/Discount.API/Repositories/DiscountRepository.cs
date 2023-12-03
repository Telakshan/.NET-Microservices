using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly IConfiguration _configuration;
    public DiscountRepository(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        var affected = await connection.ExecuteAsync
            ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", 
            new { ProductName = coupon.ProductName,  Description = coupon.Description, Amount = coupon.Amount });

        //if (affected == 0) return false; else return true;
        return affected != 0;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        var affected = await connection.ExecuteAsync
            ("DELETE FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

        return affected != 0;
    }

    public async Task<Coupon> GetDiscount(string productName)
    {
        using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
            ("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName});

        if(coupon == null)
        {
            return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount"};
        }

        return coupon;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        var affected = await connection.ExecuteAsync
            ("UPDATE Coupon set ProductName = @ProductName , Description = @Description, Amount = @Amount WHERE Id = @Id", 
            new { Id = coupon.Id, ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

        return affected != 0;

    }

    public async Task<IEnumerable<Coupon>> GetAllDiscounts()
    {
        using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        var coupons = new List<Coupon>();

        connection.Open();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT Id, ProductName, Description, Amount  FROM Coupon", connection))
        {
            NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            { 
                var Id = Int32.Parse(reader[0].ToString() ?? string.Empty);
                var ProductName = reader.GetString(1);
                var Description = reader.GetString(2);
                var Amount = Int32.Parse(reader[3].ToString() ?? string.Empty);
                coupons.Add(new Coupon { Id = Id, ProductName = ProductName, Description = Description, Amount = Amount });

            }
        }

        if (coupons == null)
        {
          return Enumerable.Empty<Coupon>();
        }

        connection.Close();

        return coupons;
    }
}
