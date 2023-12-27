using Bogus;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        if (!orderContext.Orders.Any()) 
        {
            var testOrders = GetPreconfiguredOrder();
            orderContext.Orders.AddRange(testOrders);
            await orderContext.SaveChangesAsync();
            logger.LogInformation("Seed database associated with context {DbContextname}", typeof(OrderContext).Name);
        }
    }

    private static IEnumerable<Order> GetPreconfiguredOrder()
    {
        var faker = new Faker();
        var creditCardName = faker.PickRandom<CreditCardName>();

        var testOrders = new Faker<Order>()
            .RuleFor(o => o.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
            .RuleFor(o => o.FirstName, f => f.Name.FirstName())
            .RuleFor(o => o.LastName, f => f.Name.LastName())
            .RuleFor(o => o.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(o => o.AddressLine, f => f.Address.StreetAddress())
            .RuleFor(o => o.Country, f => f.Address.Country())
            .RuleFor(o => o.State, f => f.Address.State())
            .RuleFor(o => o.ZipCode, f => f.Address.ZipCode())
            .RuleFor(u => u.CardName, f => f.PickRandom<CreditCardName>().ToString())
            .RuleFor(o => o.CardNumber, f => f.Finance.CreditCardNumber())
            .RuleFor(o => o.Expiration, f => f.Date.Between(new DateTime(2024, 1, 1),
                                new DateTime(2030, 1, 1)))
            .RuleFor(o => o.CVV, f => f.Finance.CreditCardCvv())
            .RuleFor(o => o.PaymentMethod, f => (int)f.PickRandom<PaymentMethod>());

        return testOrders.Generate(30);
    }
}
